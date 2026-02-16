import React, { useState } from 'react';

interface Notification {
  id: string;
  title: string;
  message: string;
  timestamp: Date;
  read: boolean;
  url?: string;
}

export function NotificationCenter() {
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [isOpen, setIsOpen] = useState(false);

  const unreadCount = notifications.filter(n => !n.read).length;

  const markAsRead = (id: string) => {
    setNotifications(notifications.map(n => 
      n.id === id ? { ...n, read: true } : n
    ));
  };

  const markAllAsRead = () => {
    setNotifications(notifications.map(n => ({ ...n, read: true })));
  };

  return (
    <div style={{ position: 'relative' }}>
      {/* Bell icon with badge */}
      <button
        onClick={() => setIsOpen(!isOpen)}
        style={{
          position: 'relative',
          backgroundColor: 'transparent',
          border: 'none',
          cursor: 'pointer',
          padding: '0.5rem',
          fontSize: '1.5rem'
        }}
      >
        🔔
        {unreadCount > 0 && (
          <span style={{
            position: 'absolute',
            top: 0,
            right: 0,
            backgroundColor: '#ef4444',
            color: 'white',
            borderRadius: '50%',
            width: '1.25rem',
            height: '1.25rem',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            fontSize: '0.75rem',
            fontWeight: 'bold'
          }}>
            {unreadCount}
          </span>
        )}
      </button>

      {/* Notification dropdown */}
      {isOpen && (
        <div style={{
          position: 'absolute',
          top: '100%',
          right: 0,
          marginTop: '0.5rem',
          width: '20rem',
          maxHeight: '24rem',
          backgroundColor: 'white',
          border: '1px solid #e5e7eb',
          borderRadius: '0.5rem',
          boxShadow: '0 10px 15px -3px rgba(0, 0, 0, 0.1)',
          overflow: 'hidden',
          zIndex: 1000
        }}>
          {/* Header */}
          <div style={{
            padding: '1rem',
            borderBottom: '1px solid #e5e7eb',
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center'
          }}>
            <h3 style={{ margin: 0, fontSize: '1rem', fontWeight: 600 }}>
              Benachrichtigungen
            </h3>
            {unreadCount > 0 && (
              <button
                onClick={markAllAsRead}
                style={{
                  backgroundColor: 'transparent',
                  border: 'none',
                  color: '#2563eb',
                  cursor: 'pointer',
                  fontSize: '0.875rem'
                }}
              >
                Alle als gelesen markieren
              </button>
            )}
          </div>

          {/* Notifications list */}
          <div style={{ maxHeight: '18rem', overflowY: 'auto' }}>
            {notifications.length === 0 ? (
              <div style={{
                padding: '2rem',
                textAlign: 'center',
                color: '#9ca3af'
              }}>
                Keine Benachrichtigungen
              </div>
            ) : (
              notifications.map(notification => (
                <div
                  key={notification.id}
                  onClick={() => {
                    markAsRead(notification.id);
                    if (notification.url) {
                      window.location.href = notification.url;
                    }
                  }}
                  style={{
                    padding: '1rem',
                    borderBottom: '1px solid #e5e7eb',
                    cursor: 'pointer',
                    backgroundColor: notification.read ? 'white' : '#eff6ff'
                  }}
                >
                  <div style={{ fontWeight: 600, marginBottom: '0.25rem' }}>
                    {notification.title}
                  </div>
                  <div style={{ fontSize: '0.875rem', color: '#6b7280' }}>
                    {notification.message}
                  </div>
                  <div style={{ fontSize: '0.75rem', color: '#9ca3af', marginTop: '0.25rem' }}>
                    {notification.timestamp.toLocaleString('de-DE')}
                  </div>
                </div>
              ))
            )}
          </div>
        </div>
      )}
    </div>
  );
}
