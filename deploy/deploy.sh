#!/bin/bash
# Deployment Script for HsaManageFahrzeug on Hetzner Cloud
# This script deploys the application with health checks and rollback capability

set -e

# Color output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

log_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

log_warn() {
    echo -e "${YELLOW}[WARN]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

log_step() {
    echo -e "${BLUE}[STEP]${NC} $1"
}

# Configuration
APP_DIR="/opt/hsamanafahrzeug"
COMPOSE_FILE="$APP_DIR/docker-compose.prod.yml"
ENV_FILE="$APP_DIR/.env"
BACKUP_DIR="$APP_DIR/backups"
LOG_DIR="$APP_DIR/logs"

# Check if .env file exists
if [ ! -f "$ENV_FILE" ]; then
    log_error ".env file not found at $ENV_FILE"
    log_error "Please create .env file from .env.example"
    exit 1
fi

# Load environment variables
source "$ENV_FILE"

# Create backup of current state
log_step "Creating backup of current deployment..."
BACKUP_NAME="backup-$(date +%Y%m%d-%H%M%S)"
mkdir -p "$BACKUP_DIR/$BACKUP_NAME"

# Backup database if container is running
if docker ps | grep -q hsamanafahrzeug-postgres; then
    log_info "Backing up database..."
    docker exec hsamanafahrzeug-postgres pg_dump -U "$POSTGRES_USER" "$POSTGRES_DB" > "$BACKUP_DIR/$BACKUP_NAME/database.sql"
    log_info "Database backed up to $BACKUP_DIR/$BACKUP_NAME/database.sql"
fi

# Backup current docker-compose state
if [ -f "$COMPOSE_FILE" ]; then
    cp "$COMPOSE_FILE" "$BACKUP_DIR/$BACKUP_NAME/"
    log_info "Docker Compose configuration backed up"
fi

# Function to check service health
check_health() {
    local service=$1
    local max_attempts=30
    local attempt=1
    
    log_info "Checking health of $service..."
    
    while [ $attempt -le $max_attempts ]; do
        if docker ps | grep -q "hsamanafahrzeug-$service" && \
           docker inspect --format='{{.State.Health.Status}}' "hsamanafahrzeug-$service" 2>/dev/null | grep -q "healthy"; then
            log_info "$service is healthy!"
            return 0
        fi
        
        echo -n "."
        sleep 2
        attempt=$((attempt + 1))
    done
    
    log_error "$service failed health check after $max_attempts attempts"
    return 1
}

# Function to rollback deployment
rollback() {
    log_error "Deployment failed! Rolling back..."
    
    # Restore from backup
    if [ -f "$BACKUP_DIR/$BACKUP_NAME/docker-compose.prod.yml" ]; then
        cp "$BACKUP_DIR/$BACKUP_NAME/docker-compose.prod.yml" "$COMPOSE_FILE"
    fi
    
    # Restart with old configuration
    cd "$APP_DIR"
    docker compose -f "$COMPOSE_FILE" down
    docker compose -f "$COMPOSE_FILE" up -d
    
    log_error "Rollback completed. Please check the logs and fix the issues."
    exit 1
}

# Main deployment process
log_info "=========================================="
log_info "Starting deployment of HsaManageFahrzeug"
log_info "=========================================="

cd "$APP_DIR"

# Step 1: Pull latest images (if using registry)
if [ "${USE_REGISTRY:-false}" = "true" ]; then
    log_step "Pulling latest Docker images from registry..."
    docker compose -f "$COMPOSE_FILE" pull || {
        log_warn "Failed to pull images, will build locally"
    }
else
    log_step "Building Docker images locally..."
    docker compose -f "$COMPOSE_FILE" build --no-cache || {
        log_error "Failed to build images"
        exit 1
    }
fi

# Step 2: Stop old containers (but keep data)
log_step "Stopping old containers..."
docker compose -f "$COMPOSE_FILE" down --remove-orphans

# Step 3: Start database first
log_step "Starting PostgreSQL database..."
docker compose -f "$COMPOSE_FILE" up -d postgres redis

# Wait for database to be ready
sleep 5
check_health "postgres" || rollback
check_health "redis" || rollback

# Step 4: Run database migrations
log_step "Running database migrations..."
docker compose -f "$COMPOSE_FILE" run --rm backend dotnet ef database update || {
    log_warn "Migration failed or not configured, continuing..."
}

# Step 5: Start backend
log_step "Starting backend service..."
docker compose -f "$COMPOSE_FILE" up -d backend

# Wait for backend to be ready
sleep 10
check_health "backend" || rollback

# Step 6: Start frontend
log_step "Starting frontend service..."
docker compose -f "$COMPOSE_FILE" up -d frontend

# Wait for frontend to be ready
sleep 5
check_health "frontend" || rollback

# Step 7: Start Nginx reverse proxy
log_step "Starting Nginx reverse proxy..."
docker compose -f "$COMPOSE_FILE" up -d nginx

# Wait for Nginx to be ready
sleep 5
check_health "nginx" || rollback

# Step 8: Start Certbot (for SSL renewal)
log_step "Starting Certbot for SSL certificate management..."
docker compose -f "$COMPOSE_FILE" up -d certbot

# Step 9: Final health check
log_step "Running final health checks..."

# Check all services
SERVICES=("postgres" "redis" "backend" "frontend" "nginx")
ALL_HEALTHY=true

for service in "${SERVICES[@]}"; do
    if ! check_health "$service"; then
        ALL_HEALTHY=false
        log_error "$service is not healthy!"
    fi
done

if [ "$ALL_HEALTHY" = false ]; then
    rollback
fi

# Step 10: Test API endpoint
log_step "Testing API endpoint..."
if curl -f -s "http://localhost/api/health" > /dev/null 2>&1; then
    log_info "API health check passed!"
else
    log_warn "API health check endpoint not responding (may not be implemented yet)"
fi

# Step 11: Clean up old images
log_step "Cleaning up old Docker images..."
docker image prune -f

# Step 12: Display status
log_info "=========================================="
log_info "Deployment completed successfully!"
log_info "=========================================="
echo ""

log_info "Container Status:"
docker compose -f "$COMPOSE_FILE" ps

echo ""
log_info "Service URLs:"
log_info "  - Application: https://${DOMAIN_NAME}"
log_info "  - API: https://${DOMAIN_NAME}/api"

echo ""
log_info "Logs:"
log_info "  - View all logs: docker compose -f $COMPOSE_FILE logs -f"
log_info "  - View backend logs: docker compose -f $COMPOSE_FILE logs -f backend"
log_info "  - View nginx logs: docker compose -f $COMPOSE_FILE logs -f nginx"

echo ""
log_info "Backup created at: $BACKUP_DIR/$BACKUP_NAME"

# Optional: Send notification (Slack, Email, etc.)
if [ -n "${SLACK_WEBHOOK_URL:-}" ]; then
    curl -X POST -H 'Content-type: application/json' \
        --data "{\"text\":\"✅ HsaManageFahrzeug deployed successfully to ${DOMAIN_NAME}\"}" \
        "$SLACK_WEBHOOK_URL" 2>/dev/null || true
fi

log_info "Deployment completed at $(date)"
