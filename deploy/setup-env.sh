#!/bin/bash
# Configuration Generator for HsaManageFahrzeug
# This script helps generate secure passwords and create .env file

set -e

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m'

echo -e "${BLUE}========================================${NC}"
echo -e "${BLUE}HsaManageFahrzeug Configuration Setup${NC}"
echo -e "${BLUE}========================================${NC}"
echo ""

# Check if .env already exists
if [ -f ".env" ]; then
    echo -e "${YELLOW}Warning: .env file already exists!${NC}"
    read -p "Do you want to overwrite it? (y/N): " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        echo "Aborted. Existing .env file preserved."
        exit 0
    fi
fi

# Generate secure passwords
echo -e "${GREEN}Generating secure passwords...${NC}"
POSTGRES_PASSWORD=$(openssl rand -base64 32 | tr -d "=+/" | cut -c1-32)
REDIS_PASSWORD=$(openssl rand -base64 32 | tr -d "=+/" | cut -c1-32)
JWT_KEY=$(openssl rand -base64 64 | tr -d "\n")

# Prompt for domain
echo ""
echo -e "${BLUE}Configuration Questions:${NC}"
read -p "Enter your domain name (e.g., app.example.de): " DOMAIN_NAME

# Prompt for email
read -p "Enter your email for SSL certificates: " LETSENCRYPT_EMAIL

# Prompt for IKFZ API (optional)
echo ""
echo -e "${YELLOW}IKFZ API Configuration (optional - press Enter to skip):${NC}"
read -p "IKFZ API URL (optional): " IKFZ_API_URL
read -p "IKFZ API Key (optional): " IKFZ_API_KEY

# Create .env file
echo ""
echo -e "${GREEN}Creating .env file...${NC}"

cat > .env << EOF
# ===== Application Settings =====
ASPNETCORE_ENVIRONMENT=Production
DOMAIN_NAME=${DOMAIN_NAME}

# ===== PostgreSQL Database =====
POSTGRES_USER=hsamanafahrzeug
POSTGRES_PASSWORD=${POSTGRES_PASSWORD}
POSTGRES_DB=hsamanafahrzeug

# ===== Redis Cache =====
REDIS_PASSWORD=${REDIS_PASSWORD}

# ===== JWT Authentication =====
JWT_KEY=${JWT_KEY}
JWT_ISSUER=https://${DOMAIN_NAME}
JWT_AUDIENCE=https://${DOMAIN_NAME}

# ===== SSL/TLS (Let's Encrypt) =====
LETSENCRYPT_EMAIL=${LETSENCRYPT_EMAIL}

# ===== CORS Configuration =====
ALLOWED_ORIGINS=https://${DOMAIN_NAME}

# ===== IKFZ API Configuration =====
IKFZ_API_URL=${IKFZ_API_URL:-https://api.ikfz.de/v1}
IKFZ_API_KEY=${IKFZ_API_KEY}

# ===== Optional: Docker Registry =====
USE_REGISTRY=false

# ===== Optional: Notifications =====
# SLACK_WEBHOOK_URL=

# ===== Optional: Monitoring =====
# GRAFANA_ADMIN_PASSWORD=admin
EOF

# Set proper permissions
chmod 600 .env

echo ""
echo -e "${GREEN}✓ .env file created successfully!${NC}"
echo ""
echo -e "${BLUE}Generated Credentials:${NC}"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo -e "Domain:             ${DOMAIN_NAME}"
echo -e "PostgreSQL User:    hsamanafahrzeug"
echo -e "PostgreSQL Password: ${POSTGRES_PASSWORD:0:10}... (32 chars)"
echo -e "Redis Password:     ${REDIS_PASSWORD:0:10}... (32 chars)"
echo -e "JWT Key:            ${JWT_KEY:0:20}... (64+ chars)"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo -e "${YELLOW}IMPORTANT SECURITY NOTES:${NC}"
echo "1. The .env file contains sensitive information"
echo "2. Never commit .env to version control"
echo "3. File permissions set to 600 (owner read/write only)"
echo "4. Store passwords securely (password manager recommended)"
echo ""
echo -e "${GREEN}Next Steps:${NC}"
echo "1. Review .env file: nano .env"
echo "2. Obtain SSL certificate: bash nginx/ssl-renew.sh"
echo "3. Deploy application: bash deploy/deploy.sh"
echo ""
echo -e "${BLUE}For detailed instructions, see docs/DEPLOYMENT.md${NC}"
