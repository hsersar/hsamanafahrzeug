# Hetzner Schnellstart-Anleitung

**Fahrzeugzulassungs-Webapp auf Hetzner Cloud in 15 Minuten deployen**

## Schritt 1: Hetzner Cloud Server erstellen (5 Minuten)

1. **Anmelden** bei https://console.hetzner.cloud
2. **Server erstellen**:
   - **Standort**: Nürnberg (DSGVO)
   - **Image**: Ubuntu 22.04 LTS
   - **Typ**: CPX21 (empfohlen für Produktion, ~11€/Monat)
   - **SSH-Key**: Eigenen Public Key hinzufügen
   - **Name**: fahrzeugzulassung-prod
3. **Server erstellen** - Warten Sie ~60 Sekunden

**Kosten**: Ab 3,79€/Monat (CX11) bis 11,39€/Monat (CPX21 empfohlen)

## Schritt 2: DNS einrichten (5 Minuten)

Bei Ihrem Domain-Provider (z.B. Namecheap, GoDaddy):

```
Type: A
Name: @
Wert: <SERVER_IP_ADRESSE>

Type: A
Name: api
Wert: <SERVER_IP_ADRESSE>

Type: A
Name: www
Wert: <SERVER_IP_ADRESSE>
```

**Warten Sie 5-30 Minuten** bis DNS propagiert ist.

## Schritt 3: Server vorbereiten (3 Minuten)

SSH-Verbindung zum Server:

```bash
ssh root@<SERVER_IP_ADRESSE>
```

Setup-Script ausführen:

```bash
# Script herunterladen und ausführen
curl -fsSL https://raw.githubusercontent.com/hsersar/hsamanafahrzeug/main/scripts/setup-hetzner.sh | bash
```

Das Script installiert automatisch:
- ✅ Docker & Docker Compose
- ✅ Nginx
- ✅ Firewall (UFW)
- ✅ Fail2ban
- ✅ Let's Encrypt (Certbot)
- ✅ Automatische Sicherheitsupdates

## Schritt 4: Applikation deployen (2 Minuten)

Repository klonen:

```bash
cd /opt
git clone https://github.com/hsersar/hsamanafahrzeug.git
cd hsamanafahrzeug
```

Umgebungsvariablen konfigurieren:

```bash
# Environment-Datei kopieren
cp .env.example .env.production

# Bearbeiten mit nano
nano .env.production
```

**Wichtig**: Diese Werte MÜSSEN gesetzt werden:

```bash
# Starkes Passwort generieren
POSTGRES_PASSWORD=$(openssl rand -base64 24)

# JWT Secret generieren
JWT_SECRET=$(openssl rand -base64 32)

# Ihre Domain
DOMAIN=ihre-domain.de
EMAIL=admin@ihre-domain.de
```

**Speichern**: `Ctrl+X`, dann `Y`, dann `Enter`

Data-Verzeichnisse erstellen:

```bash
mkdir -p /opt/hsamanafahrzeug/data/{postgres,uploads,logs,backups}
```

Docker Container starten:

```bash
docker compose -f docker-compose.prod.yml up -d
```

Status prüfen:

```bash
docker compose -f docker-compose.prod.yml ps
docker compose -f docker-compose.prod.yml logs -f backend
```

## Schritt 5: SSL-Zertifikat einrichten (1 Minute)

**Wichtig**: DNS muss bereits propagiert sein!

```bash
# Nginx Config kopieren
cp nginx/fahrzeugzulassung.conf /etc/nginx/sites-available/
ln -s /etc/nginx/sites-available/fahrzeugzulassung.conf /etc/nginx/sites-enabled/

# SSL-Zertifikat anfordern
certbot --nginx -d ihre-domain.de -d api.ihre-domain.de -d www.ihre-domain.de --email admin@ihre-domain.de --agree-tos --non-interactive

# Nginx testen und starten
nginx -t
systemctl restart nginx
```

## Schritt 6: Testen! ✅

Öffnen Sie in Ihrem Browser:

1. **Swagger UI**: https://ihre-domain.de/swagger
2. **Health Check**: https://ihre-domain.de/health
3. **API**: https://api.ihre-domain.de/api

### Standard-Login

```
Email: admin@fahrzeugzulassung.de
Passwort: Admin@123456789
```

**⚠️ WICHTIG**: Ändern Sie das Standard-Passwort SOFORT nach dem ersten Login!

## Fertig! 🎉

Ihre Applikation läuft jetzt produktiv auf Hetzner Cloud mit:
- ✅ HTTPS/SSL (Let's Encrypt)
- ✅ Firewall-Schutz
- ✅ Automatische Backups möglich
- ✅ BSI-konforme Sicherheit
- ✅ DSGVO-konform (deutscher Server)

## Nächste Schritte

### 1. Sicherheit härten

```bash
# Standard-Admin-Passwort ändern (über Swagger UI)
# Neuen SuperAdmin erstellen
# Standard-Admin deaktivieren
```

### 2. Automatische Backups einrichten

```bash
# Backup-Script zu Cron hinzufügen
crontab -e

# Tägliches Backup um 2:00 Uhr
0 2 * * * /opt/hsamanafahrzeug/scripts/backup-db.sh
```

### 3. Monitoring einrichten

```bash
# Logs überwachen
docker compose -f /opt/hsamanafahrzeug/docker-compose.prod.yml logs -f

# Systemressourcen überwachen
htop
docker stats
```

## Häufige Probleme

### 1. "502 Bad Gateway" Fehler

```bash
# Container-Status prüfen
docker compose ps

# Logs ansehen
docker compose logs backend

# Neu starten
docker compose restart backend
```

### 2. SSL-Zertifikat funktioniert nicht

```bash
# DNS-Propagierung prüfen
nslookup ihre-domain.de

# Certbot erneut ausführen
certbot renew --force-renewal

# Nginx neu starten
systemctl restart nginx
```

### 3. Datenbank-Verbindungsfehler

```bash
# PostgreSQL-Container prüfen
docker compose exec postgres psql -U fahrzeug_user -d FahrzeugZulassung -c "SELECT 1;"

# Connection String in .env.production prüfen
```

## Wartung

### Updates einspielen

```bash
cd /opt/hsamanafahrzeug

# Backup erstellen
./scripts/backup-db.sh

# Neuesten Code holen
git pull origin main

# Container neu bauen
docker compose -f docker-compose.prod.yml down
docker compose -f docker-compose.prod.yml build --no-cache
docker compose -f docker-compose.prod.yml up -d
```

### Backup wiederherstellen

```bash
./scripts/restore-db.sh /opt/hsamanafahrzeug/data/backups/fahrzeugzulassung_backup_20260216.sql.gz
```

## Support

- **Dokumentation**: Siehe [DEPLOYMENT_HETZNER.md](DEPLOYMENT_HETZNER.md)
- **GitHub Issues**: https://github.com/hsersar/hsamanafahrzeug/issues
- **Hetzner Support**: https://docs.hetzner.com

## Kosten-Übersicht

| Komponente | Preis/Monat |
|------------|-------------|
| Server CPX21 | ~11,39€ |
| Domain | ~1€ (10€/Jahr) |
| SSL-Zertifikat | Kostenlos (Let's Encrypt) |
| Backup-Volume (optional) | ~2€ |
| **Gesamt** | **~14€/Monat** |

Alle Preise sind Circa-Angaben und können variieren.
