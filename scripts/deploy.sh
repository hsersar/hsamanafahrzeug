#!/bin/bash
#
# Deployment Script for Hetzner
# Deploys or updates the Fahrzeugzulassungs-Webapp
#
# Usage: ./deploy.sh [--initial|--update]
#

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${GREEN}=========================================="
echo "Fahrzeugzulassung Deployment Script"
echo -e "==========================================${NC}"
echo ""

# Configuration
APP_DIR="/opt/hsamanafahrzeug/repo"
BACKUP_BEFORE_UPDATE=true

# Parse arguments
MODE="${1:---update}"

# Check if running as root
if [ "$EUID" -ne 0 ]; then 
    echo -e "${RED}Please run as root or with sudo${NC}"
    exit 1
fi

case "$MODE" in
    --initial)
        echo -e "${BLUE}Initial Deployment Mode${NC}"
        echo ""
        
        # Check if already deployed
        if [ -d "$APP_DIR" ]; then
            echo -e "${YELLOW}Warning: Application directory already exists${NC}"
            echo -n "Overwrite? (yes/no): "
            read -r CONFIRM
            if [ "$CONFIRM" != "yes" ]; then
                echo "Deployment cancelled"
                exit 0
            fi
            rm -rf "$APP_DIR"
        fi
        
        # Clone repository
        echo "Cloning repository..."
        git clone https://github.com/hsersar/hsamanafahrzeug.git "$APP_DIR"
        cd "$APP_DIR"
        
        # Create environment file
        if [ ! -f ".env.production" ]; then
            echo "Creating production environment file..."
            cp .env.example .env.production
            echo ""
            echo -e "${YELLOW}IMPORTANT: Edit .env.production with your settings:${NC}"
            echo "  nano .env.production"
            echo ""
            echo "Required settings:"
            echo "  - POSTGRES_PASSWORD (generate strong password)"
            echo "  - JWT_SECRET (generate with: openssl rand -base64 32)"
            echo "  - DOMAIN (your domain name)"
            echo ""
            echo -n "Press Enter when done editing..."
            read
        fi
        
        # Create data directories
        echo "Creating data directories..."
        mkdir -p /opt/hsamanafahrzeug/data/{postgres,uploads,logs,backups}
        chmod -R 755 /opt/hsamanafahrzeug/data
        
        # Build and start containers
        echo "Building Docker images..."
        docker-compose -f docker-compose.prod.yml build --no-cache
        
        echo "Starting services..."
        docker-compose -f docker-compose.prod.yml up -d
        
        echo ""
        echo -e "${GREEN}✓ Initial deployment complete!${NC}"
        echo ""
        echo "Next steps:"
        echo "1. Setup SSL certificate:"
        echo "   certbot --nginx -d yourdomain.de -d api.yourdomain.de -d www.yourdomain.de"
        echo ""
        echo "2. Access your application:"
        echo "   https://yourdomain.de/swagger"
        echo ""
        echo "3. Change default password!"
        ;;
        
    --update)
        echo -e "${BLUE}Update Mode${NC}"
        echo ""
        
        # Check if app directory exists
        if [ ! -d "$APP_DIR" ]; then
            echo -e "${RED}Error: Application not deployed yet${NC}"
            echo "Run: $0 --initial"
            exit 1
        fi
        
        cd "$APP_DIR"
        
        # Create backup before update
        if [ "$BACKUP_BEFORE_UPDATE" = true ]; then
            echo "Creating backup before update..."
            ./scripts/backup-db.sh
        fi
        
        # Pull latest changes
        echo "Pulling latest changes..."
        git pull origin main
        
        # Rebuild containers
        echo "Rebuilding Docker images..."
        docker-compose -f docker-compose.prod.yml build --no-cache
        
        # Restart services
        echo "Restarting services..."
        docker-compose -f docker-compose.prod.yml down
        docker-compose -f docker-compose.prod.yml up -d
        
        # Wait for services to start
        echo "Waiting for services to start..."
        sleep 10
        
        # Check health
        echo "Checking application health..."
        if curl -f http://localhost:5001/health > /dev/null 2>&1; then
            echo -e "${GREEN}✓ Application is healthy${NC}"
        else
            echo -e "${RED}✗ Health check failed${NC}"
            echo "Check logs: docker-compose -f docker-compose.prod.yml logs backend"
        fi
        
        echo ""
        echo -e "${GREEN}✓ Update complete!${NC}"
        ;;
        
    *)
        echo -e "${RED}Unknown mode: $MODE${NC}"
        echo ""
        echo "Usage: $0 [--initial|--update]"
        echo ""
        echo "Modes:"
        echo "  --initial  : Initial deployment (clone repo, setup)"
        echo "  --update   : Update existing deployment"
        exit 1
        ;;
esac

echo ""
echo "Deployment log:"
docker-compose -f "$APP_DIR/docker-compose.prod.yml" ps
