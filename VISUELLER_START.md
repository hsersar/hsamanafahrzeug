# 🎯 Lokaler Start - Visueller Guide

## Übersicht

```
┌─────────────────────────────────────────────────────────────┐
│                     IHRE MASCHINE                            │
│                                                              │
│  Terminal 1                         Terminal 2              │
│  ┌──────────────────┐               ┌──────────────────┐   │
│  │ Backend          │               │ Frontend         │   │
│  │ .NET 10          │               │ React + Vite     │   │
│  │ Port: 5000       │◄──────────────│ Port: 3000       │   │
│  │                  │   API Calls   │                  │   │
│  │ • In-Memory DB   │               │ • TypeScript     │   │
│  │ • 3 Standorte    │               │ • Dashboard      │   │
│  │ • REST API       │               │                  │   │
│  └──────────────────┘               └──────────────────┘   │
│           │                                  │              │
│           │                                  │              │
│           └──────────┬───────────────────────┘              │
│                      │                                      │
│                      ▼                                      │
│              ┌───────────────┐                              │
│              │   Browser     │                              │
│              │ localhost:3000│                              │
│              └───────────────┘                              │
└─────────────────────────────────────────────────────────────┘
```

## Schritt-für-Schritt

### 1️⃣ Backend starten

```bash
cd backend/src/FahrzeugZulassung.API
dotnet run
```

**Warten Sie auf diese Meldung:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

✅ Backend läuft!

---

### 2️⃣ Frontend starten (Neues Terminal!)

⚠️ **WICHTIG:** Öffnen Sie ein NEUES Terminal-Fenster. Backend muss weiter laufen!

```bash
cd frontend
npm install          # Nur beim ersten Mal (dauert 1-2 Minuten)
npm run dev
```

**Warten Sie auf diese Meldung:**
```
  VITE v7.3.1  ready in XXX ms

  ➜  Local:   http://localhost:3000/
```

✅ Frontend läuft!

---

### 3️⃣ Browser öffnen

Öffnen Sie: **http://localhost:3000**

---

## Was Sie sehen sollten

### SuperAdmin Dashboard

```
╔═══════════════════════════════════════════════════════════╗
║  SuperAdmin Dashboard - Provisions- und Abrechnung        ║
╠═══════════════════════════════════════════════════════════╣
║                                                           ║
║  📊 Standard-Provisionsmodell                             ║
║  ┌───────────────────────────────────────────────────┐   ║
║  │ Grundgebühr: 199.00 €  │ Provisionssatz: 2.0 %   │   ║
║  │ Gültig ab: [Datum]     │ Status: ✅ Aktiv        │   ║
║  └───────────────────────────────────────────────────┘   ║
║                                                           ║
║  💰 Plattform-Umsatz 2026                                 ║
║  ┌────────────┬────────────┬────────────┐                ║
║  │ Standorte  │  Umsatz    │ Einnahmen  │                ║
║  │    3/3     │  0.00 €    │  0.00 €    │                ║
║  └────────────┴────────────┴────────────┘                ║
║                                                           ║
║  📋 Monatsabrechnungen (0)                                ║
║  ┌─────────────────────────────────────────────────┐     ║
║  │ [Tabelle leer - keine Abrechnungen]            │     ║
║  │                                                 │     ║
║  │ [Alle Abrechnungen erstellen] <-- KLICK HIER!  │     ║
║  └─────────────────────────────────────────────────┘     ║
║                                                           ║
╚═══════════════════════════════════════════════════════════╝
```

---

## Test: Abrechnungen erstellen

1. **Klicken Sie** auf den Button "Alle Abrechnungen erstellen"
2. **Warten Sie** auf die Erfolgsmeldung (Alert)
3. **Sehen Sie** 3 neue Abrechnungen in der Tabelle:
   - ABR-2026-02-001 - Standort Berlin
   - ABR-2026-02-002 - Standort München
   - ABR-2026-02-003 - Standort Hamburg

---

## Häufige Probleme & Lösungen

### ❌ Backend startet nicht

**Problem:** Port 5000 bereits belegt

**Lösung:**
```bash
# Windows
netstat -ano | findstr :5000
taskkill /PID <PID_NUMMER> /F

# Mac/Linux
lsof -i :5000
kill -9 <PID_NUMMER>
```

---

### ❌ Frontend zeigt Fehler

**Problem:** Backend ist nicht erreichbar

**Prüfen:**
1. Backend läuft wirklich auf Port 5000?
2. Browser-Konsole öffnen (F12)
3. Netzwerk-Tab prüfen

**Lösung:** Backend neu starten

---

### ❌ "npm: command not found"

**Problem:** Node.js nicht installiert

**Lösung:**
1. Download von https://nodejs.org/
2. Installation durchführen
3. Terminal NEU öffnen
4. `npm --version` testen

---

### ❌ "dotnet: command not found"

**Problem:** .NET SDK nicht installiert

**Lösung:**
1. Download von https://dotnet.microsoft.com/download
2. Installation durchführen
3. Terminal NEU öffnen
4. `dotnet --version` testen

---

## API Endpoints testen

### Mit Browser

- http://localhost:5000/api/provisionen/standard
- http://localhost:5000/api/monatsabrechnungen

### Mit curl

```bash
# Standard-Provisionsmodell abrufen
curl http://localhost:5000/api/provisionen/standard

# Alle Abrechnungen abrufen
curl http://localhost:5000/api/monatsabrechnungen

# Plattform-Umsatz abrufen
curl "http://localhost:5000/api/monatsabrechnungen/plattform-umsatz?jahr=2026"
```

---

## Projekt stoppen

### Backend stoppen
Im Backend-Terminal: **Ctrl+C**

### Frontend stoppen
Im Frontend-Terminal: **Ctrl+C**

---

## Zusammenfassung: 3 Schritte zum Erfolg

```
┌─────────────────────────────────────────────────────┐
│ 1. Backend starten                                  │
│    cd backend/src/FahrzeugZulassung.API            │
│    dotnet run                                       │
│    → Port 5000                                      │
└─────────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────┐
│ 2. Frontend starten (NEUES TERMINAL!)               │
│    cd frontend                                      │
│    npm install && npm run dev                       │
│    → Port 3000                                      │
└─────────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────┐
│ 3. Browser öffnen                                   │
│    http://localhost:3000                           │
│    → Dashboard erscheint!                           │
└─────────────────────────────────────────────────────┘
```

✅ **Fertig! Das Projekt läuft lokal!**

---

## Weitere Hilfe

- **Ausführliche Anleitung:** `STARTEN.md`
- **API Dokumentation:** `backend/README.md`
- **Frontend Details:** `frontend/README.md`
- **English Version:** `QUICK_START.md`
