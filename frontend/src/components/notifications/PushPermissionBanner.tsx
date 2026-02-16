import React, { useState, useEffect } from 'react';
import { usePushNotifications } from '../../hooks/usePushNotifications';

export function PushPermissionBanner() {
  const [isDismissed, setIsDismissed] = useState(false);
  const { isSupported, permission, subscribe } = usePushNotifications();

  useEffect(() => {
    // Check if user has previously dismissed the banner
    const dismissed = localStorage.getItem('pushBannerDismissed');
    if (dismissed) {
      setIsDismissed(true);
    }
  }, []);

  const handleSubscribe = async () => {
    try {
      await subscribe();
      setIsDismissed(true);
      localStorage.setItem('pushBannerDismissed', 'true');
    } catch (error) {
      console.error('Failed to subscribe:', error);
      alert('Benachrichtigungen konnten nicht aktiviert werden.');
    }
  };

  const handleDismiss = () => {
    setIsDismissed(true);
    localStorage.setItem('pushBannerDismissed', 'true');
  };

  // Don't show banner if:
  // - Not supported
  // - Already granted permission
  // - User dismissed it
  if (!isSupported || permission === 'granted' || isDismissed) {
    return null;
  }

  return (
    <div style={{
      position: 'fixed',
      bottom: 0,
      left: 0,
      right: 0,
      backgroundColor: '#2563eb',
      color: 'white',
      padding: '1rem',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'space-between',
      gap: '1rem',
      boxShadow: '0 -2px 10px rgba(0, 0, 0, 0.1)',
      zIndex: 1000
    }}>
      <div style={{ flex: 1 }}>
        <strong>🔔 Benachrichtigungen aktivieren</strong>
        <p style={{ margin: '0.25rem 0 0 0', fontSize: '0.875rem' }}>
          Erhalten Sie sofort Updates zu Ihren Anträgen
        </p>
      </div>
      <div style={{ display: 'flex', gap: '0.5rem' }}>
        <button
          onClick={handleSubscribe}
          style={{
            backgroundColor: 'white',
            color: '#2563eb',
            border: 'none',
            padding: '0.5rem 1rem',
            borderRadius: '0.375rem',
            cursor: 'pointer',
            fontWeight: 600
          }}
        >
          Aktivieren
        </button>
        <button
          onClick={handleDismiss}
          style={{
            backgroundColor: 'transparent',
            color: 'white',
            border: '1px solid white',
            padding: '0.5rem 1rem',
            borderRadius: '0.375rem',
            cursor: 'pointer'
          }}
        >
          Später
        </button>
      </div>
    </div>
  );
}
