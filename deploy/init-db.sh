#!/bin/bash
# Database initialization script for PostgreSQL
# This script runs on first database creation

set -e

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    -- Enable required extensions
    CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
    CREATE EXTENSION IF NOT EXISTS "pg_trgm";
    
    -- Set default timezone to Europe/Berlin
    SET timezone = 'Europe/Berlin';
    
    -- Grant necessary permissions
    GRANT ALL PRIVILEGES ON DATABASE $POSTGRES_DB TO $POSTGRES_USER;
    
    -- Create index for better performance
    -- (Application-specific schemas will be created by EF Core migrations)
EOSQL

echo "Database initialization completed successfully!"
