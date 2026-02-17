import React, { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { NavLink, Link, useLocation, useNavigate } from 'react-router-dom';
import ApiService from '../services/api';
import { RegistrationRequest } from '../types';

interface NavigationProps {
  isSidebarCollapsed: boolean;
  onToggleSidebar: () => void;
}

const Navigation: React.FC<NavigationProps> = ({
  isSidebarCollapsed,
  onToggleSidebar,
}) => {
  const navigate = useNavigate();
  const location = useLocation();
  const [latestRequest, setLatestRequest] = useState<RegistrationRequest | null>(null);
  const [statusLoading, setStatusLoading] = useState(true);
  const [statusError, setStatusError] = useState(false);
  const [lastUpdated, setLastUpdated] = useState<Date | null>(null);
  const role = localStorage.getItem('user_role') || 'Kunde';
  const [isProfileOpen, setIsProfileOpen] = useState(false);
  const [isNotifOpen, setIsNotifOpen] = useState(false);
  const [isDark, setIsDark] = useState(() => {
    const saved = localStorage.getItem('theme');
    return saved === 'dark';
  });
  const profileRef = useRef<HTMLDivElement>(null);
  const notifRef = useRef<HTMLDivElement>(null);

  // Apply theme to document
  useEffect(() => {
    document.documentElement.setAttribute('data-theme', isDark ? 'dark' : 'light');
    localStorage.setItem('theme', isDark ? 'dark' : 'light');
  }, [isDark]);

  // Close dropdowns when clicking outside
  useEffect(() => {
    const handleClickOutside = (e: MouseEvent) => {
      if (profileRef.current && !profileRef.current.contains(e.target as Node)) {
        setIsProfileOpen(false);
      }
      if (notifRef.current && !notifRef.current.contains(e.target as Node)) {
        setIsNotifOpen(false);
      }
    };
    if (isProfileOpen) {
      document.addEventListener('mousedown', handleClickOutside);
    }
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, [isProfileOpen]);

  // Close dropdown on route change
  useEffect(() => {
    setIsProfileOpen(false);
  }, [location.pathname]);

  useEffect(() => {
    let isActive = true;

    const loadLatestRequest = async () => {
      try {
        setStatusLoading(true);
        const requests = await ApiService.getMyRequests();
        if (!isActive) return;
        setLatestRequest(requests[0] ?? null);
        setStatusError(false);
        setLastUpdated(new Date());
      } catch (error) {
        if (!isActive) return;
        setLatestRequest(null);
        setStatusError(true);
      } finally {
        if (isActive) {
          setStatusLoading(false);
        }
      }
    };

    loadLatestRequest();
    const intervalId = window.setInterval(loadLatestRequest, 30000);

    return () => {
      isActive = false;
      window.clearInterval(intervalId);
    };
  }, []);

  const statusMeta = useMemo(() => {
    const status = latestRequest?.status ?? null;
    const statusLabelMap: Record<string, string> = {
      Pending: 'Eingegangen',
      UnderReview: 'In Prüfung',
      Approved: 'Genehmigt',
      Rejected: 'Abgelehnt',
    };
    const stepMap: Record<string, number> = {
      Pending: 1,
      UnderReview: 2,
      Approved: 3,
      Rejected: 3,
    };

    return {
      status,
      label: status ? statusLabelMap[status] ?? status : null,
      step: status ? stepMap[status] ?? 1 : 1,
    };
  }, [latestRequest]);



  const handleLogout = () => {
    ApiService.logout()
      .catch(() => undefined)
      .finally(() => {
        localStorage.removeItem('access_token');
        navigate('/login');
      });
  };

  return (
    <>
      <header className="topbar">
        <div className="topbar-left">
          <button
            className="icon-button"
            aria-label="Menü"
            aria-expanded={!isSidebarCollapsed}
            onClick={onToggleSidebar}
          >
            <span className="icon">☰</span>
          </button>
          <Link to="/" className="brand">
            iKfz
          </Link>
          <span className="brand-tag">2026</span>
        </div>
        <div className="topbar-actions">
          <span className="role-badge">{role}</span>
          <button
            className="icon-button"
            aria-label="Suche"
            onClick={() => navigate('/search')}
          >
            <span className="icon">🔍</span>
            Suche
          </button>
          <button className="icon-button" aria-label="Chat">
            <span className="icon">💬</span>
            Chat
          </button>
          <div className="notif-bell-wrapper" ref={notifRef}>
            <button
              className={`notif-bell-btn ${statusLoading ? 'is-loading' : ''} ${statusMeta.label ? 'has-status' : ''}`}
              onClick={() => setIsNotifOpen((prev) => !prev)}
              aria-label="Benachrichtigungen"
              aria-expanded={isNotifOpen}
            >
              <svg className="notif-bell-icon" viewBox="0 0 24 24" width="20" height="20" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9" />
                <path d="M13.73 21a2 2 0 0 1-3.46 0" />
              </svg>
              {statusMeta.label && <span className="notif-bell-dot" />}
            </button>

            {isNotifOpen && (
              <div className="notif-dropdown">
                <div className="notif-dropdown-header">
                  <span className="notif-dropdown-title">Bearbeitungsstand</span>
                </div>
                <div className="notif-dropdown-body">
                  {statusLoading ? (
                    <div className="notif-item notif-item--loading">
                      <span className="notif-item-icon">⟳</span>
                      <span>Status wird geladen…</span>
                    </div>
                  ) : statusError ? (
                    <div className="notif-item notif-item--error">
                      <span className="notif-item-icon">⚠️</span>
                      <span>Status nicht verfügbar</span>
                    </div>
                  ) : statusMeta.label ? (
                    <div className="notif-item notif-item--active">
                      <span className="notif-item-icon">📋</span>
                      <div className="notif-item-content">
                        <strong>{statusMeta.label}</strong>
                        {lastUpdated && (
                          <span className="notif-item-time">
                            Aktualisiert:{' '}
                            {lastUpdated.toLocaleString('de-DE', {
                              day: '2-digit',
                              month: '2-digit',
                              year: 'numeric',
                              hour: '2-digit',
                              minute: '2-digit',
                            })}
                          </span>
                        )}
                      </div>
                    </div>
                  ) : (
                    <div className="notif-item notif-item--empty">
                      <span className="notif-item-icon">✅</span>
                      <span>Kein offener Vorgang</span>
                    </div>
                  )}
                </div>
              </div>
            )}
          </div>
          <button
            className="theme-toggle-btn"
            onClick={() => setIsDark((prev) => !prev)}
            aria-label={isDark ? 'Lichtmodus aktivieren' : 'Dunkelmodus aktivieren'}
            title={isDark ? 'Lichtmodus' : 'Dunkelmodus'}
          >
            {isDark ? (
              <svg className="theme-toggle-icon" viewBox="0 0 24 24" width="18" height="18" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <circle cx="12" cy="12" r="5" />
                <line x1="12" y1="1" x2="12" y2="3" />
                <line x1="12" y1="21" x2="12" y2="23" />
                <line x1="4.22" y1="4.22" x2="5.64" y2="5.64" />
                <line x1="18.36" y1="18.36" x2="19.78" y2="19.78" />
                <line x1="1" y1="12" x2="3" y2="12" />
                <line x1="21" y1="12" x2="23" y2="12" />
                <line x1="4.22" y1="19.78" x2="5.64" y2="18.36" />
                <line x1="18.36" y1="5.64" x2="19.78" y2="4.22" />
              </svg>
            ) : (
              <svg className="theme-toggle-icon" viewBox="0 0 24 24" width="18" height="18" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z" />
              </svg>
            )}
          </button>
          <div className="profile-wrapper" ref={profileRef}>
            <button
              className="profile-button"
              aria-label="Profil"
              aria-expanded={isProfileOpen}
              onClick={() => setIsProfileOpen((prev) => !prev)}
            >
              <span className="profile-dot">HS</span>
              Profil
            </button>
            {isProfileOpen && (
              <div className="profile-dropdown">
                <div className="profile-dropdown-top">
                  <div className="profile-dropdown-avatar">HS</div>
                  <div className="profile-dropdown-info">
                    <span className="profile-dropdown-name">Hicham Sersar</span>
                    <span className="profile-dropdown-role">{role}</span>
                  </div>
                </div>
                <div className="profile-dropdown-stats">
                  <div className="profile-dropdown-stat">
                    <span className="profile-dropdown-stat-value">
                      {latestRequest ? '1' : '0'}
                    </span>
                    <span className="profile-dropdown-stat-label">Anträge</span>
                  </div>
                  <div className="profile-dropdown-stat">
                    <span className="profile-dropdown-stat-value">2</span>
                    <span className="profile-dropdown-stat-label">Fahrzeuge</span>
                  </div>
                  <div className="profile-dropdown-stat">
                    <span className="profile-dropdown-stat-value">0</span>
                    <span className="profile-dropdown-stat-label">Bescheide</span>
                  </div>
                </div>
                <div className="profile-dropdown-section-title">Konto</div>
                <nav className="profile-dropdown-links">
                  <NavLink to="/profile?tab=stammdaten" className="profile-dropdown-link">
                    <span className="profile-dropdown-link-icon" aria-hidden="true">👤</span>
                    <span>Persönliche Daten</span>
                    <span className="profile-dropdown-chevron" aria-hidden="true">›</span>
                  </NavLink>
                  <NavLink to="/reset-password" className="profile-dropdown-link">
                    <span className="profile-dropdown-link-icon" aria-hidden="true">🔒</span>
                    <span>Anmelden &amp; Sicherheit</span>
                    <span className="profile-dropdown-chevron" aria-hidden="true">›</span>
                  </NavLink>
                  <NavLink to="/profile?tab=zahlungsarten" className="profile-dropdown-link">
                    <span className="profile-dropdown-link-icon" aria-hidden="true">💳</span>
                    <span>Zahlungsmethoden</span>
                    <span className="profile-dropdown-chevron" aria-hidden="true">›</span>
                  </NavLink>
                </nav>
                <button className="profile-dropdown-logout" onClick={handleLogout}>
                  abmelden
                </button>
              </div>
            )}
          </div>
        </div>
      </header>

      <div className="subbar">
        <div className="subbar-left">
          <span className="breadcrumb">Übersicht</span>
        </div>
      </div>

      <aside className={`sidebar ${isSidebarCollapsed ? 'is-collapsed' : ''}`}>
        <div className="sidebar-section">
          <nav className="sidebar-nav">
            <NavLink to="/" className="sidebar-link" end>
              <span className="sidebar-icon" aria-hidden="true">🏠</span>
              <span className="sidebar-text">Dashboard</span>
            </NavLink>
          </nav>
        </div>
        <div className="sidebar-section">
          <div className="sidebar-title">Zulassungsaufträge</div>
          <nav className="sidebar-nav">
            <NavLink to="/my-requests" className="sidebar-link">
              <span className="sidebar-icon" aria-hidden="true">📋</span>
              <span className="sidebar-text">Meine Anträge</span>
            </NavLink>
            <NavLink to="/new-registration" className="sidebar-link">
              <span className="sidebar-icon" aria-hidden="true">➕</span>
              <span className="sidebar-text">Neuer Antrag</span>
            </NavLink>
            <NavLink to="/my-vehicles" className="sidebar-link">
              <span className="sidebar-icon" aria-hidden="true">🚗</span>
              <span className="sidebar-text">Meine Fahrzeuge</span>
            </NavLink>
          </nav>
        </div>
        <div className="sidebar-section">
          <div className="sidebar-title">Dokumente &amp; Finanzen</div>
          <nav className="sidebar-nav">
            <NavLink to="/dokumente" className="sidebar-link">
              <span className="sidebar-icon" aria-hidden="true">📁</span>
              <span className="sidebar-text">Dokumente</span>
            </NavLink>
            <NavLink to="/bescheide" className="sidebar-link">
              <span className="sidebar-icon" aria-hidden="true">📄</span>
              <span className="sidebar-text">Bescheide</span>
            </NavLink>
            <NavLink to="/rechnungen" className="sidebar-link">
              <span className="sidebar-icon" aria-hidden="true">💰</span>
              <span className="sidebar-text">Rechnungen</span>
            </NavLink>
            <NavLink to="/vollmachten" className="sidebar-link">
              <span className="sidebar-icon" aria-hidden="true">📎</span>
              <span className="sidebar-text">Vollmachten</span>
            </NavLink>
          </nav>
        </div>
        {(role === 'SuperAdmin' || role === 'StandortAdmin' || role === 'Mitarbeiter') && (
          <div className="sidebar-section">
            <div className="sidebar-title">Administration</div>
            <nav className="sidebar-nav">
              {role === 'SuperAdmin' && (
                <NavLink to="/admin/super" className="sidebar-link">
                  <span className="sidebar-icon" aria-hidden="true">🛡️</span>
                  <span className="sidebar-text">Super-Admin</span>
                </NavLink>
              )}
              {(role === 'StandortAdmin' || role === 'SuperAdmin') && (
                <NavLink to="/admin/standort" className="sidebar-link">
                  <span className="sidebar-icon" aria-hidden="true">🏢</span>
                  <span className="sidebar-text">Standort-Admin</span>
                </NavLink>
              )}
              {(role === 'StandortAdmin' || role === 'SuperAdmin') && (
                <NavLink to="/company-profile" className="sidebar-link">
                  <span className="sidebar-icon" aria-hidden="true">🏭</span>
                  <span className="sidebar-text">Unternehmensprofil</span>
                </NavLink>
              )}
              {(role === 'Mitarbeiter' || role === 'StandortAdmin' || role === 'SuperAdmin') && (
                <NavLink to="/mitarbeiter/inbox" className="sidebar-link">
                  <span className="sidebar-icon" aria-hidden="true">📥</span>
                  <span className="sidebar-text">Mitarbeiter-Inbox</span>
                </NavLink>
              )}
            </nav>
          </div>
        )}
        <div className="sidebar-section">
          <nav className="sidebar-nav">
            <NavLink to="/help" className="sidebar-link">
              <span className="sidebar-icon" aria-hidden="true">❓</span>
              <span className="sidebar-text">Hilfe &amp; Kontakt</span>
            </NavLink>
          </nav>
        </div>
      </aside>
    </>
  );
};

export default Navigation;
