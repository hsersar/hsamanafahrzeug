import { useState, useEffect } from 'react';
import { paymentApi } from '../services/api';
import type { ZahlungsMethode } from '../types';
import PaymentMethodSelector from '../components/payment/PaymentMethodSelector';
import UeberweisungsDetails from '../components/payment/UeberweisungsDetails';
import './ZahlungsSeite.css';

interface Props {
  rechnungId: string;
  betrag: number;
}

export default function ZahlungsSeite({ rechnungId, betrag }: Props) {
  const [methods, setMethods] = useState<ZahlungsMethode[]>([]);
  const [selectedMethod, setSelectedMethod] = useState<number | null>(null);
  const [loading, setLoading] = useState(false);
  const [paymentResult, setPaymentResult] = useState<any>(null);

  useEffect(() => {
    loadPaymentMethods();
  }, []);

  const loadPaymentMethods = async () => {
    try {
      const data = await paymentApi.getAvailableMethods();
      setMethods(data);
    } catch (err) {
      console.error('Failed to load payment methods:', err);
    }
  };

  const handlePayment = async () => {
    if (!selectedMethod) return;

    setLoading(true);
    try {
      const result = await paymentApi.initiatePayment({
        rechnungId,
        methode: selectedMethod,
      });
      setPaymentResult(result);
    } catch (err) {
      console.error('Payment failed:', err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="zahlungs-seite">
      <header className="payment-header">
        <h1>💳 Zahlung</h1>
        <div className="amount">
          <span className="amount-label">Zu zahlen:</span>
          <span className="amount-value">{betrag.toFixed(2)} €</span>
        </div>
      </header>

      {!paymentResult ? (
        <>
          <PaymentMethodSelector
            methods={methods}
            selected={selectedMethod}
            onSelect={setSelectedMethod}
          />

          <button
            className="pay-button"
            onClick={handlePayment}
            disabled={!selectedMethod || loading}
          >
            {loading ? 'Wird verarbeitet...' : 'Jetzt bezahlen'}
          </button>
        </>
      ) : (
        <div className="payment-result">
          {paymentResult.ueberweisung ? (
            <UeberweisungsDetails details={paymentResult.ueberweisung} />
          ) : paymentResult.clientSecret ? (
            <div>Stripe Payment Form würde hier erscheinen</div>
          ) : paymentResult.redirectUrl ? (
            <div>
              <p>Weiterleitung zu PayPal...</p>
              <a href={paymentResult.redirectUrl}>Zu PayPal</a>
            </div>
          ) : null}
        </div>
      )}
    </div>
  );
}
