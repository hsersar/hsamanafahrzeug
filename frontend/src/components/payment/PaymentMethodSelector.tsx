import type { ZahlungsMethode } from '../../types';
import './PaymentMethodSelector.css';

interface Props {
  methods: ZahlungsMethode[];
  selected: number | null;
  onSelect: (id: number) => void;
}

export default function PaymentMethodSelector({ methods, selected, onSelect }: Props) {
  return (
    <div className="payment-method-selector">
      <h3>Zahlungsmethode wählen</h3>
      <div className="methods-grid">
        {methods.map((method) => (
          <div
            key={method.id}
            className={`method-card ${selected === method.id ? 'selected' : ''}`}
            onClick={() => onSelect(method.id)}
          >
            <div className="method-icon">{method.icon}</div>
            <div className="method-name">{method.name}</div>
          </div>
        ))}
      </div>
    </div>
  );
}
