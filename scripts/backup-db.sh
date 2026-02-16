#!/bin/bash
#
# Database Backup Script
# Creates a timestamped backup of the PostgreSQL database
#
# Usage: ./backup-db.sh
#

set -e

# Configuration
BACKUP_DIR="/opt/hsamanafahrzeug/data/backups"
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
BACKUP_FILE="fahrzeugzulassung_backup_${TIMESTAMP}.sql"
KEEP_DAYS=30

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${GREEN}=========================================="
echo "Database Backup Script"
echo -e "==========================================${NC}"
echo ""

# Check if Docker is running
if ! docker ps > /dev/null 2>&1; then
    echo -e "${RED}Error: Docker is not running${NC}"
    exit 1
fi

# Check if PostgreSQL container is running
if ! docker ps | grep -q fahrzeugzulassung-db; then
    echo -e "${RED}Error: PostgreSQL container is not running${NC}"
    exit 1
fi

# Create backup directory if it doesn't exist
mkdir -p "$BACKUP_DIR"

echo "Creating backup..."
echo "Backup file: $BACKUP_FILE"

# Create backup
if docker exec fahrzeugzulassung-db-prod pg_dump -U fahrzeug_user FahrzeugZulassung > "$BACKUP_DIR/$BACKUP_FILE"; then
    echo -e "${GREEN}✓ Backup created successfully${NC}"
    
    # Compress backup
    echo "Compressing backup..."
    gzip "$BACKUP_DIR/$BACKUP_FILE"
    echo -e "${GREEN}✓ Backup compressed: ${BACKUP_FILE}.gz${NC}"
    
    # Get file size
    SIZE=$(du -h "$BACKUP_DIR/${BACKUP_FILE}.gz" | cut -f1)
    echo "Backup size: $SIZE"
else
    echo -e "${RED}✗ Backup failed${NC}"
    exit 1
fi

# Clean up old backups
echo ""
echo "Cleaning up backups older than $KEEP_DAYS days..."
find "$BACKUP_DIR" -name "fahrzeugzulassung_backup_*.sql.gz" -type f -mtime +$KEEP_DAYS -delete
REMAINING=$(ls -1 "$BACKUP_DIR"/fahrzeugzulassung_backup_*.sql.gz 2>/dev/null | wc -l)
echo -e "${GREEN}✓ Cleanup complete. $REMAINING backup(s) remaining${NC}"

echo ""
echo -e "${GREEN}Backup completed successfully!${NC}"
echo "Location: $BACKUP_DIR/${BACKUP_FILE}.gz"
