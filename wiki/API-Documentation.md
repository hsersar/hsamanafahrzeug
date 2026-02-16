# API Dokumentation

Diese Seite enthält die technische API-Dokumentation für das hsamanafahrzeug Projekt.

## Übersicht

Die API-Dokumentation wird erweitert, sobald die entsprechenden Module implementiert sind.

## Geplante API-Endpunkte

### Fahrzeugverwaltung

#### Fahrzeug abrufen
```
GET /api/vehicles/{id}
```

**Beschreibung**: Ruft ein einzelnes Fahrzeug anhand seiner ID ab.

**Parameter**:
- `id` (string): Eindeutige Fahrzeug-ID

**Antwort**:
```json
{
  "id": "vehicle-123",
  "make": "Hersteller",
  "model": "Modell",
  "year": 2024,
  "status": "active"
}
```

#### Alle Fahrzeuge auflisten
```
GET /api/vehicles
```

**Beschreibung**: Listet alle Fahrzeuge auf.

**Query-Parameter**:
- `limit` (integer): Anzahl der zurückzugebenden Einträge (Standard: 20)
- `offset` (integer): Offset für Pagination (Standard: 0)
- `status` (string): Filter nach Status

**Antwort**:
```json
{
  "total": 100,
  "vehicles": [
    {
      "id": "vehicle-123",
      "make": "Hersteller",
      "model": "Modell"
    }
  ]
}
```

#### Fahrzeug erstellen
```
POST /api/vehicles
```

**Beschreibung**: Erstellt ein neues Fahrzeug.

**Request Body**:
```json
{
  "make": "Hersteller",
  "model": "Modell",
  "year": 2024,
  "vin": "VIN123456789"
}
```

**Antwort**:
```json
{
  "id": "vehicle-124",
  "make": "Hersteller",
  "model": "Modell",
  "year": 2024,
  "created_at": "2024-01-01T00:00:00Z"
}
```

#### Fahrzeug aktualisieren
```
PUT /api/vehicles/{id}
```

**Beschreibung**: Aktualisiert ein bestehendes Fahrzeug.

**Parameter**:
- `id` (string): Eindeutige Fahrzeug-ID

**Request Body**:
```json
{
  "status": "inactive",
  "mileage": 50000
}
```

#### Fahrzeug löschen
```
DELETE /api/vehicles/{id}
```

**Beschreibung**: Löscht ein Fahrzeug.

**Parameter**:
- `id` (string): Eindeutige Fahrzeug-ID

## Fehlerbehandlung

### Fehlerformate

Alle API-Fehler folgen diesem Format:

```json
{
  "error": {
    "code": "ERROR_CODE",
    "message": "Beschreibung des Fehlers",
    "details": {}
  }
}
```

### HTTP-Statuscodes

- `200 OK`: Erfolgreiche Anfrage
- `201 Created`: Ressource erfolgreich erstellt
- `400 Bad Request`: Ungültige Anfrage
- `404 Not Found`: Ressource nicht gefunden
- `500 Internal Server Error`: Serverfehler

## Authentifizierung

Details zur Authentifizierung werden in zukünftigen Versionen ergänzt.

## Rate Limiting

Informationen zu Rate Limits werden hinzugefügt, sobald implementiert.

## Beispiele

### cURL-Beispiele

```bash
# Fahrzeug abrufen
curl -X GET https://api.example.com/api/vehicles/vehicle-123

# Neues Fahrzeug erstellen
curl -X POST https://api.example.com/api/vehicles \
  -H "Content-Type: application/json" \
  -d '{"make":"BMW","model":"X5","year":2024}'
```

## Weitere Ressourcen

- [Projektübersicht](Project-Overview.md)
- [Erste Schritte](Getting-Started.md)
- [FAQ](FAQ.md)

---

**Hinweis**: Diese API-Dokumentation ist vorläufig und wird mit der Entwicklung des Projekts erweitert.
