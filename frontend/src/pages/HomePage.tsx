import React from 'react';
import { Link } from 'react-router-dom';

const HomePage: React.FC = () => {
  return (
    <div className="home-page">
      <h1>Welcome to iKfz</h1>
      <p>Internet-based Vehicle Registration System</p>
      
      <div className="action-cards">
        <Link to="/new-registration" className="action-card">
          <h2>New Registration</h2>
          <p>Register a new vehicle</p>
        </Link>
        
        <Link to="/my-vehicles" className="action-card">
          <h2>My Vehicles</h2>
          <p>View your registered vehicles</p>
        </Link>
        
        <Link to="/my-requests" className="action-card">
          <h2>My Requests</h2>
          <p>Track registration requests</p>
        </Link>
      </div>
      
      <div className="info-section">
        <h2>System Features</h2>
        <div className="features-grid">
          <div className="feature">
            <h3>🔒 Secure</h3>
            <p>OAuth2/OIDC authentication, ISO 27001 compliant</p>
          </div>
          <div className="feature">
            <h3>🛡️ Privacy</h3>
            <p>GDPR compliant, no PII in logs</p>
          </div>
          <div className="feature">
            <h3>⚡ Fast</h3>
            <p>Supports ≥100 parallel requests with caching</p>
          </div>
          <div className="feature">
            <h3>📱 Modern</h3>
            <p>Responsive React frontend, RESTful API</p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default HomePage;
