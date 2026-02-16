#!/bin/bash
# SSL Certificate Renewal Script for Let's Encrypt

set -e

DOMAIN="${DOMAIN_NAME}"
EMAIL="${LETSENCRYPT_EMAIL}"
COMPOSE_FILE="/opt/hsamanafahrzeug/docker-compose.prod.yml"

echo "Starting SSL certificate renewal process..."

# Stop Nginx to free up port 80
docker-compose -f "$COMPOSE_FILE" stop nginx

# Renew certificate
docker-compose -f "$COMPOSE_FILE" run --rm certbot certonly \
    --standalone \
    --preferred-challenges http \
    --email "$EMAIL" \
    --agree-tos \
    --no-eff-email \
    --force-renewal \
    -d "$DOMAIN"

# Start Nginx again
docker-compose -f "$COMPOSE_FILE" start nginx

# Reload Nginx to use new certificate
docker-compose -f "$COMPOSE_FILE" exec nginx nginx -s reload

echo "SSL certificate renewed successfully!"
