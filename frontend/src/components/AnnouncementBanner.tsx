import React, { useEffect, useState } from 'react';
import ApiService from '../services/api';
import { Announcement } from '../types';

const AnnouncementBanner: React.FC = () => {
  const [announcements, setAnnouncements] = useState<Announcement[]>([]);
  const [dismissed, setDismissed] = useState<Set<number>>(new Set());

  useEffect(() => {
    const load = async () => {
      try {
        const data = await ApiService.getAnnouncements();
        setAnnouncements(data);
      } catch {
        // silently ignore
      }
    };
    load();
  }, []);

  const dismiss = (id: number) => {
    setDismissed((prev) => new Set(prev).add(id));
  };

  const visible = announcements.filter((a) => !dismissed.has(a.id));
  if (visible.length === 0) return null;

  const typeIcon: Record<string, string> = {
    info: 'ℹ️',
    warning: '⚠️',
    success: '✅',
    error: '❌',
  };

  return (
    <div className="announcement-list">
      {visible.map((a) => (
        <div key={a.id} className={`announcement-banner announcement--${a.typ}`}>
          <span className="announcement-icon">{typeIcon[a.typ] || 'ℹ️'}</span>
          <div className="announcement-body">
            <strong className="announcement-title">{a.titel}</strong>
            <span className="announcement-text">{a.nachricht}</span>
          </div>
          <button
            className="announcement-close"
            onClick={() => dismiss(a.id)}
            aria-label="Schließen"
          >
            ✕
          </button>
        </div>
      ))}
    </div>
  );
};

export default AnnouncementBanner;
