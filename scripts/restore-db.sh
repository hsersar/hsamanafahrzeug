#!/bin/bash
#
# Database Restore Script
# Restores a PostgreSQL database from a backup file
#
# Usage: ./restore-db.sh <backup_file>
#

set -e

# Configuration
BACKUP_DIR="/opt/hsamanafahrzeug/data/backups"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${GREEN}=========================================="
echo "Database Restore Script"
echo -e "==========================================${NC}"
echo ""

# Check arguments
if [ "$#" -ne 1 ]; then
    echo -e "${RED}Usage: $0 <backup_file>${NC}"
    echo ""
    echo "Available backups:"
    ls -lh "$BACKUP_DIR"/fahrzeugzulassung_backup_*.sql.gz 2>/dev/null || echo "No backups found"
    exit 1
fi

BACKUP_FILE="$1"

# Check if backup file exists
if [ ! -f "$BACKUP_FILE" ]; then
    # Try in backup directory
    if [ -f "$BACKUP_DIR/$BACKUP_FILE" ]; then
        BACKUP_FILE="$BACKUP_DIR/$BACKUP_FILE"
    else
        echo -e "${RED}Error: Backup file not found: $BACKUP_FILE${NC}"
        exit 1
    fi
fi

echo "Backup file: $BACKUP_FILE"
echo ""

# Confirm restore
echo -e "${YELLOW}WARNING: This will overwrite the current database!${NC}"
echo -n "Are you sure you want to continue? (yes/no): "
read -r CONFIRM

if [ "$CONFIRM" != "yes" ]; then
    echo "Restore cancelled"
    exit 0
fi

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

echo ""
echo "Stopping backend service..."
docker-compose -f /opt/hsamanafahrzeug/repo/docker-compose.prod.yml stop backend

echo "Decompressing backup if needed..."
if [[ "$BACKUP_FILE" == *.gz ]]; then
    TEMP_FILE="${BACKUP_FILE%.gz}"
    gunzip -c "$BACKUP_FILE" > "$TEMP_FILE"
    BACKUP_FILE="$TEMP_FILE"
    CLEANUP_TEMP=true
fi

echo "Dropping existing database..."
docker exec fahrzeugzulassung-db-prod psql -U fahrzeug_user -d postgres -c "DROP DATABASE IF EXISTS FahrzeugZulassung;"

echo "Creating new database..."
docker exec fahrzeugzulassung-db-prod psql -U fahrzeug_user -d postgres -c "CREATE DATABASE FahrzeugZulassung;"

echo "Restoring backup..."
if docker exec -i fahrzeugzulassung-db-prod psql -U fahrzeug_user FahrzeugZulassung < "$BACKUP_FILE"; then
    echo -e "${GREEN}✓ Database restored successfully${NC}"
else
    echo -e "${RED}✗ Restore failed${NC}"
    exit 1
fi

# Cleanup temporary file
if [ "$CLEANUP_TEMP" = true ]; then
    rm -f "$TEMP_FILE"
fi

echo "Starting backend service..."
docker-compose -f /opt/hsamanafahrzeug/repo/docker-compose.prod.yml start backend

echo ""
echo -e "${GREEN}Restore completed successfully!${NC}"
