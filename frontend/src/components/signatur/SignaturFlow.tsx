import React, { useState } from 'react';
import { SignaturTyp, SignaturAuthMethode } from '../../types/signatur';
import DokumentVorschau from './DokumentVorschau';
import SignaturMethodenAuswahl from './SignaturMethodenAuswahl';
import SignaturWarten from './SignaturWarten';
import SignaturErfolg from './SignaturErfolg';
import SignaturFehler from './SignaturFehler';
import { SignaturApi } from '../../services/signaturApi';

export interface SignaturFlowProps {
  auftragId: string;
  typ: SignaturTyp;
  signiererEmail: string;
  signiererName: string;
  signiererTelefon?: string;
  dokumentBytes?: Uint8Array;
  dokumentName?: string;
  onAbschluss?: (signaturId: string) => void;
  onAbbruch?: () => void;
}

enum FlowSchritt {
  DokumentVorschau = 1,
  MethodenAuswahl = 2,
  KontaktdatenBestaetigen = 3,
  ProviderWeiterleitung = 4,
  Warten = 5,
  Erfolg = 6,
  Fehler = 7
}

/**
 * Hauptkomponente für den gesamten QES-Signatur-Prozess
 * Führt den Benutzer durch alle Schritte der Signatur
 */
const SignaturFlow: React.FC<SignaturFlowProps> = ({
  auftragId,
  typ,
  signiererEmail,
  signiererName,
  signiererTelefon,
  dokumentBytes,
  dokumentName,
  onAbschluss,
  onAbbruch
}) => {
  const [aktuellerSchritt, setAktuellerSchritt] = useState<FlowSchritt>(FlowSchritt.DokumentVorschau);
  const [gewaehlteMethode, setGewaehlteMethode] = useState<SignaturAuthMethode>(SignaturAuthMethode.SMS_TAN);
  const [signaturId, setSignaturId] = useState<string | null>(null);
  const [fehler, setFehler] = useState<string | null>(null);
  const [dokumentGelesen, setDokumentGelesen] = useState(false);

  const handleDokumentBestaetigt = () => {
    setAktuellerSchritt(FlowSchritt.MethodenAuswahl);
  };

  const handleMethodeGewaehlt = (methode: SignaturAuthMethode) => {
    setGewaehlteMethode(methode);
    setAktuellerSchritt(FlowSchritt.KontaktdatenBestaetigen);
  };

  const handleKontaktdatenBestaetigt = async () => {
    try {
      // Signatur anfordern
      const result = await SignaturApi.signaturAnfordern({
        auftragId,
        typ,
        signiererEmail,
        signiererName,
        signiererTelefon,
        bevorzugteAuthMethode: gewaehlteMethode,
        dokumentBytes,
        dokumentName
      });

      if (result.erfolg && result.signaturId && result.redirectUrl) {
        setSignaturId(result.signaturId);
        
        // Öffne Provider-URL in neuem Tab
        window.open(result.redirectUrl, '_blank');
        
        // Wechsle zu Warten-Schritt
        setAktuellerSchritt(FlowSchritt.Warten);
      } else {
        setFehler(result.fehlerNachricht || 'Unbekannter Fehler');
        setAktuellerSchritt(FlowSchritt.Fehler);
      }
    } catch (error) {
      setFehler(error instanceof Error ? error.message : 'Fehler bei der Signatur-Anforderung');
      setAktuellerSchritt(FlowSchritt.Fehler);
    }
  };

  const handleSignaturAbgeschlossen = (id: string) => {
    setSignaturId(id);
    setAktuellerSchritt(FlowSchritt.Erfolg);
    if (onAbschluss) {
      onAbschluss(id);
    }
  };

  const handleSignaturFehlgeschlagen = (fehlerNachricht: string) => {
    setFehler(fehlerNachricht);
    setAktuellerSchritt(FlowSchritt.Fehler);
  };

  const handleErneuerVersuch = () => {
    setAktuellerSchritt(FlowSchritt.MethodenAuswahl);
    setFehler(null);
    setSignaturId(null);
  };

  return (
    <div className="signatur-flow">
      {/* Fortschrittsanzeige */}
      <div className="flow-progress">
        <div className={`step ${aktuellerSchritt >= FlowSchritt.DokumentVorschau ? 'active' : ''}`}>
          1. Dokument
        </div>
        <div className={`step ${aktuellerSchritt >= FlowSchritt.MethodenAuswahl ? 'active' : ''}`}>
          2. Methode
        </div>
        <div className={`step ${aktuellerSchritt >= FlowSchritt.KontaktdatenBestaetigen ? 'active' : ''}`}>
          3. Kontakt
        </div>
        <div className={`step ${aktuellerSchritt >= FlowSchritt.Warten ? 'active' : ''}`}>
          4. Signatur
        </div>
        <div className={`step ${aktuellerSchritt >= FlowSchritt.Erfolg ? 'active' : ''}`}>
          5. Fertig
        </div>
      </div>

      {/* Schritte */}
      {aktuellerSchritt === FlowSchritt.DokumentVorschau && (
        <DokumentVorschau
          dokumentName={dokumentName}
          onBestaetigt={handleDokumentBestaetigt}
          onAbbruch={onAbbruch}
        />
      )}

      {aktuellerSchritt === FlowSchritt.MethodenAuswahl && (
        <SignaturMethodenAuswahl
          onMethodeGewaehlt={handleMethodeGewaehlt}
          onZurueck={() => setAktuellerSchritt(FlowSchritt.DokumentVorschau)}
        />
      )}

      {aktuellerSchritt === FlowSchritt.KontaktdatenBestaetigen && (
        <div className="kontaktdaten-bestaetigung">
          <h2>Kontaktdaten bestätigen</h2>
          <div className="kontakt-info">
            <p><strong>Name:</strong> {signiererName}</p>
            <p><strong>E-Mail:</strong> {signiererEmail}</p>
            {signiererTelefon && <p><strong>Telefon:</strong> {signiererTelefon}</p>}
            <p><strong>Methode:</strong> {getMethodenName(gewaehlteMethode)}</p>
          </div>
          <div className="actions">
            <button onClick={() => setAktuellerSchritt(FlowSchritt.MethodenAuswahl)}>
              Zurück
            </button>
            <button className="primary" onClick={handleKontaktdatenBestaetigt}>
              Jetzt signieren
            </button>
          </div>
        </div>
      )}

      {aktuellerSchritt === FlowSchritt.Warten && signaturId && (
        <SignaturWarten
          signaturId={signaturId}
          onErfolg={handleSignaturAbgeschlossen}
          onFehler={handleSignaturFehlgeschlagen}
        />
      )}

      {aktuellerSchritt === FlowSchritt.Erfolg && signaturId && (
        <SignaturErfolg
          signaturId={signaturId}
          onWeiter={() => onAbschluss && onAbschluss(signaturId)}
        />
      )}

      {aktuellerSchritt === FlowSchritt.Fehler && (
        <SignaturFehler
          fehlerNachricht={fehler || 'Unbekannter Fehler'}
          onErneuerVersuch={handleErneuerVersuch}
          onAbbruch={onAbbruch}
        />
      )}
    </div>
  );
};

function getMethodenName(methode: SignaturAuthMethode): string {
  switch (methode) {
    case SignaturAuthMethode.SMS_TAN:
      return 'SMS-TAN';
    case SignaturAuthMethode.PushNotification:
      return 'Push-Benachrichtigung';
    case SignaturAuthMethode.App_TAN:
      return 'App-TAN';
    case SignaturAuthMethode.VideoIdent:
      return 'Video-Ident';
    case SignaturAuthMethode.eID:
      return 'eID (Personalausweis)';
    default:
      return 'Unbekannt';
  }
}

export default SignaturFlow;
