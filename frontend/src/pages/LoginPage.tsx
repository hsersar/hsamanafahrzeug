import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';

const LoginPage: React.FC = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [role, setRole] = useState('Kunde');

  const handleLogin = async () => {
    setLoading(true);
    
    // In production, this would redirect to OAuth2/OIDC provider
    // For demo purposes, we'll simulate a token
    try {
      // Simulate OAuth2 flow - in production, use real OIDC client
      const demoToken = 'demo_jwt_token_' + Date.now();
      localStorage.setItem('access_token', demoToken);
      localStorage.setItem('user_role', role);
      
      navigate('/');
    } catch (error) {
      console.error('Login failed', error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-page">
      <div className="login-container">
        <h1>iKfz - Online-Zulassung</h1>
        <p>Der sichere digitale Antrag zur Fahrzeugzulassung.</p>
        <div className="login-info">
          <h2>Ihre Vorteile:</h2>
          <ul>
            <li>✓ OAuth2/OIDC Anmeldung</li>
            <li>✓ DSGVO-konform</li>
            <li>✓ ISO 27001 Sicherheitsstandard</li>
            <li>✓ Keine personenbezogenen Daten in Logs</li>
            <li>✓ Verschlüsselte Kommunikation</li>
          </ul>
        </div>
        <div className="form-group">
          <label htmlFor="role">Rolle (Mock)</label>
          <select
            id="role"
            name="role"
            value={role}
            onChange={(e) => setRole(e.target.value)}
          >
            <option value="Kunde">Kunde</option>
            <option value="Mitarbeiter">Mitarbeiter</option>
            <option value="StandortAdmin">Standort-Admin</option>
            <option value="SuperAdmin">Super-Admin</option>
          </select>
        </div>
        <button onClick={handleLogin} disabled={loading} className="login-button">
          {loading ? 'Anmeldung läuft...' : 'Anmelden und fortfahren'}
        </button>
        <div className="login-links">
          <Link to="/reset-password">Passwort vergessen?</Link>
        </div>
        <p className="demo-note">
          Hinweis: Demo-Modus. Im Produktivbetrieb erfolgt die Weiterleitung an
          einen echten OAuth2/OIDC-Anbieter.
        </p>
      </div>
    </div>
  );
};

export default LoginPage;
