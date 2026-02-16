# 🚀 Projekt lokal starten - Schritt-für-Schritt Anleitung

## ⚡ Schnellstart (3 Befehle)

```bash
# 1️⃣ Backend starten (Terminal 1)
cd backend/src/FahrzeugZulassung.API && dotnet run

# 2️⃣ Frontend starten (Terminal 2 - NEUES TERMINAL ÖFFNEN!)
cd frontend && npm install && npm run dev

# 3️⃣ Browser öffnen
# Gehe zu: http://localhost:3000
```

---

## Voraussetzungen

Bevor Sie starten, stellen Sie sicher, dass folgende Software installiert ist:

- ✅ **.NET 10 SDK** → [Download](https://dotnet.microsoft.com/download)
- ✅ **Node.js 18+** → [Download](https://nodejs.org/)
- ✅ **Git** → [Download](https://git-scm.com/)

### Installationen überprüfen

```bash
# .NET Version prüfen
dotnet --version
# Sollte 10.x.x anzeigen

# Node.js Version prüfen
node --version
# Sollte v18.x.x oder höher anzeigen

# npm Version prüfen
npm --version
```

---

## 📦 Schritt 1: Projekt klonen

Falls noch nicht geschehen:

```bash
git clone https://github.com/hsersar/hsamanafahrzeug.git
cd hsamanafahrzeug
```

---

## 🔧 Schritt 2: Backend starten

### 2.1 Zum Backend-Ordner navigieren

```bash
cd backend/src/FahrzeugZulassung.API
```

### 2.2 Backend starten

```bash
dotnet run
```

**Was passiert jetzt?**
- Das Backend wird kompiliert
- Die In-Memory-Datenbank wird initialisiert
- Demo-Daten werden geladen (3 Standorte + Standard-Provisionsmodell)
- Die API startet auf **http://localhost:5000**

**Erfolg sehen Sie an dieser Ausgabe:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

✅ **Backend läuft jetzt!**

### 2.3 Backend testen (Optional)

Öffnen Sie einen neuen Terminal und testen Sie:

```bash
curl http://localhost:5000/api/provisionen/standard
```

Sie sollten JSON-Daten sehen:
```json
{
  "id": "...",
  "monatlicheGrundgebuehr": 199.00,
  "provisionsProzentsatz": 2.0,
  "istStandard": true,
  "istAktiv": true
}
```

---

## 🎨 Schritt 3: Frontend starten

### 3.1 Neues Terminal öffnen

⚠️ **Wichtig:** Lassen Sie das Backend-Terminal offen und laufend!

Öffnen Sie ein **neues Terminal-Fenster**.

### 3.2 Zum Frontend-Ordner navigieren

```bash
cd frontend
```

(Vom Projekt-Root aus: `cd frontend`)

### 3.3 Abhängigkeiten installieren

**Nur beim ersten Mal nötig:**

```bash
npm install
```

Dies lädt alle benötigten JavaScript-Pakete herunter (ca. 1-2 Minuten).

### 3.4 Frontend starten

```bash
npm run dev
```

**Was passiert jetzt?**
- Vite startet den Entwicklungsserver
- Das Frontend wird kompiliert
- Der Server startet auf **http://localhost:3000**

**Erfolg sehen Sie an dieser Ausgabe:**
```
  VITE v7.3.1  ready in XXX ms

  ➜  Local:   http://localhost:3000/
  ➜  Network: use --host to expose
  ➜  press h + enter to show help
```

✅ **Frontend läuft jetzt!**

---

## 🌐 Schritt 4: Anwendung öffnen

### 4.1 Browser öffnen

Öffnen Sie Ihren Browser und gehen Sie zu:

**👉 http://localhost:3000**

### 4.2 Was Sie sehen sollten

Sie sehen das **SuperAdmin Dashboard** mit:

1. **Standard-Provisionsmodell**
   - Grundgebühr: 199,00 €
   - Provisionssatz: 2,0%

2. **Plattform-Umsatz KPIs**
   - Standorte: 3 / 3
   - Verschiedene Umsatz-Metriken

3. **Monatsabrechnungen-Tabelle**
   - Liste der Abrechnungen (initial leer)

4. **"Alle Abrechnungen erstellen" Button**
   - Klicken Sie darauf, um Test-Abrechnungen zu erstellen!

---

## 🎮 Schritt 5: Funktionen testen

### Test 1: Abrechnungen erstellen

1. Klicken Sie auf den Button **"Alle Abrechnungen erstellen"**
2. Eine Erfolgs-Meldung erscheint
3. Die Tabelle zeigt nun 3 neue Abrechnungen (eine pro Standort)

### Test 2: Daten aktualisieren

1. Die Seite aktualisiert sich automatisch mit den neuen Daten
2. Sie sehen die Abrechnungsnummern (z.B. ABR-2026-02-001)
3. Status-Badges zeigen "Erstellt" an

### Test 3: API direkt testen

Im Browser oder mit curl:
```bash
# Alle Abrechnungen anzeigen
curl http://localhost:5000/api/monatsabrechnungen

# Standard-Provisionsmodell anzeigen
curl http://localhost:5000/api/provisionen/standard
```

---

## 🛑 Projekt stoppen

### Backend stoppen

Im Backend-Terminal:
- Drücken Sie **Ctrl+C** (Windows/Linux)
- Oder **Cmd+C** (Mac)

### Frontend stoppen

Im Frontend-Terminal:
- Drücken Sie **Ctrl+C** (Windows/Linux)
- Oder **Cmd+C** (Mac)

---

## 🔧 Fehlerbehebung

### Problem: "dotnet: command not found"

**Lösung:** .NET 10 SDK ist nicht installiert
- Download: https://dotnet.microsoft.com/download
- Nach Installation Terminal neu starten

### Problem: "npm: command not found"

**Lösung:** Node.js ist nicht installiert
- Download: https://nodejs.org/
- Nach Installation Terminal neu starten

### Problem: Backend startet nicht

**Lösung 1:** Port 5000 bereits belegt
```bash
# Anderen Port verwenden:
dotnet run --urls="http://localhost:5001"
```

**Lösung 2:** Alte Prozesse beenden
```bash
# Windows
netstat -ano | findstr :5000
taskkill /PID <PID> /F

# Mac/Linux
lsof -i :5000
kill -9 <PID>
```

### Problem: Frontend startet nicht

**Lösung 1:** node_modules löschen und neu installieren
```bash
cd frontend
rm -rf node_modules
npm install
npm run dev
```

**Lösung 2:** Port 3000 bereits belegt
```bash
# In package.json oder vite.config.ts Port ändern
# Oder anderen Port verwenden:
npm run dev -- --port 3001
```

### Problem: Frontend zeigt keine Daten

**Prüfen:**
1. ✅ Backend läuft auf http://localhost:5000
2. ✅ Browser-Konsole öffnen (F12) → Fehler prüfen
3. ✅ CORS-Fehler? → Backend neu starten

---

## 📁 Projekt-Struktur (Übersicht)

```
hsamanafahrzeug/
├── backend/                    # .NET Backend
│   └── src/
│       └── FahrzeugZulassung.API/
│           └── Program.cs      # 👈 Startet hier mit "dotnet run"
│
├── frontend/                   # React Frontend
│   ├── src/
│   │   ├── pages/
│   │   ├── services/
│   │   └── types/
│   └── package.json           # 👈 "npm run dev" hier
│
├── STARTEN.md                 # 👈 Diese Anleitung
├── QUICK_START.md             # Englische Version
└── README.md                  # Projekt-Dokumentation
```

---

## 📚 Weitere Dokumentation

Nach dem erfolgreichen Start können Sie mehr erfahren:

- **API-Dokumentation**: `backend/README.md`
- **Frontend-Details**: `frontend/README.md`
- **Technische Details**: `IMPLEMENTATION_SUMMARY.md`
- **Quick Start (EN)**: `QUICK_START.md`

---

## ✅ Zusammenfassung - Die 3 wichtigsten Befehle

```bash
# 1️⃣ Backend starten (Terminal 1)
cd backend/src/FahrzeugZulassung.API
dotnet run

# 2️⃣ Frontend starten (Terminal 2)
cd frontend
npm install    # nur beim ersten Mal
npm run dev

# 3️⃣ Browser öffnen
# http://localhost:3000
```

---

## 🎉 Viel Erfolg!

Bei Fragen:
- Siehe Fehlerbehebung oben
- Öffnen Sie ein Issue auf GitHub
- Prüfen Sie die ausführlichen README-Dateien

**Status-Check:**
- ✅ Backend läuft auf http://localhost:5000
- ✅ Frontend läuft auf http://localhost:3000
- ✅ Dashboard wird im Browser angezeigt
- ✅ Sie können Abrechnungen erstellen

→ **Alles funktioniert! 🚀**
