#!/bin/bash
# PostgreSQL Backup Script for HsaManageFahrzeug
# This script creates automated backups of the PostgreSQL database

set -e

# Color output
GREEN='\033[0;32m'
RED='\033[0;31m'
NC='\033[0m'

log_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Configuration
APP_DIR="/opt/hsamanafahrzeug"
BACKUP_DIR="$APP_DIR/backups"
ENV_FILE="$APP_DIR/.env"
RETENTION_DAYS=7

# Load environment variables
if [ -f "$ENV_FILE" ]; then
    source "$ENV_FILE"
else
    log_error ".env file not found at $ENV_FILE"
    exit 1
fi

# Create backup directory if it doesn't exist
mkdir -p "$BACKUP_DIR"

# Generate backup filename with timestamp
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
BACKUP_FILE="$BACKUP_DIR/db_backup_$TIMESTAMP.sql"
BACKUP_FILE_GZ="$BACKUP_FILE.gz"

log_info "Starting PostgreSQL backup..."
log_info "Backup file: $BACKUP_FILE_GZ"

# Create database backup
if docker exec hsamanafahrzeug-postgres pg_dump -U "$POSTGRES_USER" "$POSTGRES_DB" > "$BACKUP_FILE"; then
    # Compress the backup
    gzip "$BACKUP_FILE"
    
    # Get file size
    SIZE=$(du -h "$BACKUP_FILE_GZ" | cut -f1)
    
    log_info "Backup completed successfully!"
    log_info "Backup size: $SIZE"
    log_info "Backup location: $BACKUP_FILE_GZ"
else
    log_error "Backup failed!"
    rm -f "$BACKUP_FILE" 2>/dev/null
    exit 1
fi

# Clean up old backups (keep only last N days)
log_info "Cleaning up old backups (keeping last $RETENTION_DAYS days)..."
find "$BACKUP_DIR" -name "db_backup_*.sql.gz" -type f -mtime +$RETENTION_DAYS -delete

# Count remaining backups
BACKUP_COUNT=$(find "$BACKUP_DIR" -name "db_backup_*.sql.gz" -type f | wc -l)
log_info "Total backups retained: $BACKUP_COUNT"

# Optional: Upload to Hetzner Object Storage (S3-compatible)
if [ -n "${HETZNER_S3_BUCKET:-}" ] && [ -n "${HETZNER_S3_ACCESS_KEY:-}" ]; then
    log_info "Uploading backup to Hetzner Object Storage..."
    
    # Install s3cmd if not present
    if ! command -v s3cmd &> /dev/null; then
        log_error "s3cmd not found. Install with: apt-get install s3cmd"
    else
        # Configure s3cmd (create config if needed)
        cat > /tmp/s3cfg <<EOF
[default]
access_key = ${HETZNER_S3_ACCESS_KEY}
secret_key = ${HETZNER_S3_SECRET_KEY}
host_base = ${HETZNER_S3_ENDPOINT:-fsn1.your-objectstorage.com}
host_bucket = %(bucket)s.${HETZNER_S3_ENDPOINT:-fsn1.your-objectstorage.com}
use_https = True
EOF
        
        # Upload to S3
        if s3cmd -c /tmp/s3cfg put "$BACKUP_FILE_GZ" "s3://${HETZNER_S3_BUCKET}/backups/"; then
            log_info "Backup uploaded to S3 successfully!"
        else
            log_error "Failed to upload backup to S3"
        fi
        
        rm -f /tmp/s3cfg
    fi
fi

# Optional: Send notification
if [ -n "${SLACK_WEBHOOK_URL:-}" ]; then
    curl -X POST -H 'Content-type: application/json' \
        --data "{\"text\":\"✅ Database backup completed: $BACKUP_FILE_GZ ($SIZE)\"}" \
        "$SLACK_WEBHOOK_URL" 2>/dev/null || true
fi

log_info "Backup process completed at $(date)"

# Exit successfully
exit 0
