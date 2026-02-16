# Deployment Architecture

```
┌─────────────────────────────────────────────────────────────────────┐
│                         Internet / Users                             │
└────────────────────────────────┬────────────────────────────────────┘
                                 │
                                 │ HTTPS (443)
                                 │ HTTP (80) → Redirect to HTTPS
                                 ▼
┌─────────────────────────────────────────────────────────────────────┐
│                        Hetzner Cloud Server                          │
│                     (CPX31: 4 vCPU, 8GB RAM)                        │
│                        Ubuntu 22.04 LTS                              │
├─────────────────────────────────────────────────────────────────────┤
│  ┌───────────────────────────────────────────────────────────────┐  │
│  │                      UFW Firewall                             │  │
│  │  Allowed: 22 (SSH), 80 (HTTP), 443 (HTTPS)                   │  │
│  │  Protected by: Fail2ban (SSH brute-force protection)         │  │
│  └───────────────────────────────────────────────────────────────┘  │
│                                 │                                    │
│  ┌──────────────────────────────┴─────────────────────────────┐    │
│  │                    Nginx Reverse Proxy                       │    │
│  │  ┌────────────────────────────────────────────────────────┐ │    │
│  │  │ SSL/TLS (Let's Encrypt)                                │ │    │
│  │  │ - TLS 1.2 / 1.3                                       │ │    │
│  │  │ - A+ Rating Configuration                             │ │    │
│  │  │ - Auto-renewal via Certbot                            │ │    │
│  │  └────────────────────────────────────────────────────────┘ │    │
│  │  ┌────────────────────────────────────────────────────────┐ │    │
│  │  │ Security Headers                                       │ │    │
│  │  │ - HSTS, CSP, X-Frame-Options                         │ │    │
│  │  │ - Rate Limiting (10/s API, 30/s General)             │ │    │
│  │  └────────────────────────────────────────────────────────┘ │    │
│  └────────────────┬──────────────────────┬─────────────────────┘    │
│                   │                      │                           │
│         /api/*    │                      │  /*                       │
│                   ▼                      ▼                           │
│  ┌────────────────────────────┐  ┌─────────────────────────────┐   │
│  │   Backend Container         │  │   Frontend Container        │   │
│  │   (ASP.NET Core 8)         │  │   (React + Nginx)          │   │
│  ├────────────────────────────┤  ├─────────────────────────────┤   │
│  │ Port: 8080 (internal)      │  │ Port: 80 (internal)         │   │
│  │ User: appuser (non-root)   │  │ User: nginx-app (non-root)  │   │
│  │ Image: Alpine-based        │  │ Image: Alpine-based         │   │
│  │ Health: /health endpoint   │  │ Compression: Brotli + Gzip  │   │
│  └────────────┬───────────────┘  └─────────────────────────────┘   │
│               │                                                      │
│               │                                                      │
│       ┌───────┴─────────┬──────────────────┐                       │
│       │                 │                  │                        │
│       ▼                 ▼                  ▼                        │
│  ┌─────────────┐  ┌──────────┐  ┌────────────────────┐            │
│  │ PostgreSQL  │  │  Redis   │  │  Certbot           │            │
│  │    16       │  │    7     │  │  (SSL Renewal)     │            │
│  ├─────────────┤  ├──────────┤  ├────────────────────┤            │
│  │ Port: 5432  │  │ Port:    │  │ Auto-renew every   │            │
│  │ (internal)  │  │ 6379     │  │ 12 hours           │            │
│  │ Volume:     │  │ (internal│  │                    │            │
│  │ persistent  │  │ Volume:  │  │                    │            │
│  │ Health:     │  │ persist. │  │                    │            │
│  │ pg_isready  │  │ LRU: 256M│  │                    │            │
│  └─────────────┘  └──────────┘  └────────────────────┘            │
│                                                                     │
│  ┌────────────────────────────────────────────────────────────┐   │
│  │            Docker Network: hsamanafahrzeug-backend          │   │
│  │            Isolated internal network (172.20.0.0/16)        │   │
│  └────────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────┐
│                    Optional: Monitoring Stack                        │
├─────────────────────────────────────────────────────────────────────┤
│  ┌──────────────┐  ┌───────────┐  ┌──────────────┐                 │
│  │ Prometheus   │  │  Grafana  │  │  cAdvisor    │                 │
│  │ Port: 9090   │  │ Port: 3000│  │  Port: 8081  │                 │
│  └──────┬───────┘  └─────┬─────┘  └──────┬───────┘                 │
│         │                │                │                          │
│         └────────────────┴────────────────┘                          │
│                          │                                           │
│  ┌───────────────────────┴───────────────────────────────────┐     │
│  │  Exporters: Node, PostgreSQL, Redis                       │     │
│  │  Dashboards: Container, DB, System metrics                │     │
│  └───────────────────────────────────────────────────────────┘     │
└─────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────┐
│                      Backup & Recovery                               │
├─────────────────────────────────────────────────────────────────────┤
│  Daily Backups (2:00 AM)                                            │
│  ┌──────────────────────────────────────────────────────────┐      │
│  │ PostgreSQL → Compressed SQL → Local Storage (7 days)     │      │
│  │                             ↓                             │      │
│  │                   Optional: Hetzner Object Storage (S3)   │      │
│  └──────────────────────────────────────────────────────────┘      │
└─────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────┐
│                        CI/CD Pipeline                                │
│                      (GitHub Actions)                                │
├─────────────────────────────────────────────────────────────────────┤
│  Push to main branch                                                │
│         ↓                                                            │
│  1. Run Tests (Backend + Frontend)                                  │
│         ↓                                                            │
│  2. Build Docker Images                                             │
│         ↓                                                            │
│  3. Push to GitHub Container Registry                               │
│         ↓                                                            │
│  4. Deploy to Hetzner (via SSH)                                     │
│         ↓                                                            │
│  5. Health Checks                                                   │
│         ↓                                                            │
│  6. Notification (Slack)                                            │
└─────────────────────────────────────────────────────────────────────┘

Security Layers:
═══════════════
1. Network: UFW Firewall + Fail2ban
2. SSL/TLS: Let's Encrypt A+ configuration
3. Headers: HSTS, CSP, X-Frame-Options, etc.
4. Rate Limiting: Nginx-level protection
5. Containers: Non-root execution
6. Network: Docker isolation
7. Secrets: .env file (chmod 600)
8. Updates: Automatic security patches

Performance:
═══════════
- Redis caching (256MB LRU)
- Brotli + Gzip compression
- Static asset caching (1 year)
- Connection pooling
- Health checks
- Multi-stage builds

Cost: ~€15-20/month
└── Server: €13.14
└── Backups: €2.00
└── Domain: ~€1/month
