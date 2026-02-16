import React, { useState } from 'react';

interface EVBNummerInputProps {
  value: string;
  onChange: (value: string) => void;
  onValidation?: (isValid: boolean) => void;
}

export function EVBNummerInput({ value, onChange, onValidation }: EVBNummerInputProps) {
  const [isValid, setIsValid] = useState<boolean | null>(null);
  const [isValidating, setIsValidating] = useState(false);
  const [showInfo, setShowInfo] = useState(false);

  const formatEVB = (input: string): string => {
    // Remove all non-alphanumeric characters and convert to uppercase
    const cleaned = input.replace(/[^A-Za-z0-9]/g, '').toUpperInvariant();
    
    // Limit to 7 characters
    const limited = cleaned.substring(0, 7);
    
    // Format as XXX-XXXX
    if (limited.length > 3) {
      return `${limited.substring(0, 3)}-${limited.substring(3)}`;
    }
    
    return limited;
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const formatted = formatEVB(e.target.value);
    onChange(formatted);
    
    // Reset validation when user types
    if (formatted.replace('-', '').length !== 7) {
      setIsValid(null);
    }
  };

  const validateEVB = async () => {
    const cleaned = value.replace('-', '');
    
    if (cleaned.length !== 7) {
      setIsValid(false);
      onValidation?.(false);
      return;
    }

    setIsValidating(true);
    
    try {
      const response = await fetch('/api/evb/validieren', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          evbNummer: cleaned
        })
      });

      const result = await response.json();
      setIsValid(result.istGueltig);
      onValidation?.(result.istGueltig);
    } catch (error) {
      console.error('Validation error:', error);
      setIsValid(false);
      onValidation?.(false);
    } finally {
      setIsValidating(false);
    }
  };

  const handleBlur = () => {
    if (value.replace('-', '').length === 7) {
      validateEVB();
    }
  };

  return (
    <div style={{ position: 'relative' }}>
      <label style={{
        display: 'block',
        marginBottom: '0.5rem',
        fontWeight: 600,
        fontSize: '0.875rem'
      }}>
        eVB-Nummer *
        <button
          type="button"
          onClick={() => setShowInfo(!showInfo)}
          style={{
            marginLeft: '0.5rem',
            backgroundColor: 'transparent',
            border: 'none',
            cursor: 'pointer',
            fontSize: '1rem'
          }}
          title="Was ist eine eVB-Nummer?"
        >
          ℹ️
        </button>
      </label>

      <div style={{ position: 'relative' }}>
        <input
          type="text"
          value={value}
          onChange={handleChange}
          onBlur={handleBlur}
          placeholder="XXX-XXXX"
          maxLength={8}
          style={{
            width: '100%',
            padding: '0.5rem',
            border: `2px solid ${
              isValid === null ? '#d1d5db' :
              isValid ? '#10b981' : '#ef4444'
            }`,
            borderRadius: '0.375rem',
            fontSize: '1rem',
            fontFamily: 'monospace',
            textTransform: 'uppercase'
          }}
        />
        
        {isValidating && (
          <div style={{
            position: 'absolute',
            right: '0.5rem',
            top: '50%',
            transform: 'translateY(-50%)'
          }}>
            ⏳
          </div>
        )}
        
        {!isValidating && isValid === true && (
          <div style={{
            position: 'absolute',
            right: '0.5rem',
            top: '50%',
            transform: 'translateY(-50%)',
            color: '#10b981'
          }}>
            ✓
          </div>
        )}
        
        {!isValidating && isValid === false && (
          <div style={{
            position: 'absolute',
            right: '0.5rem',
            top: '50%',
            transform: 'translateY(-50%)',
            color: '#ef4444'
          }}>
            ✗
          </div>
        )}
      </div>

      <p style={{
        marginTop: '0.25rem',
        fontSize: '0.75rem',
        color: '#6b7280'
      }}>
        Die eVB-Nummer erhalten Sie von Ihrer KFZ-Versicherung
      </p>

      {isValid === false && (
        <p style={{
          marginTop: '0.25rem',
          fontSize: '0.75rem',
          color: '#ef4444'
        }}>
          Ungültige eVB-Nummer. Bitte überprüfen Sie Ihre Eingabe.
        </p>
      )}
    </div>
  );
}
