import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';

const LoginPage: React.FC = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);

  const handleLogin = async () => {
    setLoading(true);
    
    // In production, this would redirect to OAuth2/OIDC provider
    // For demo purposes, we'll simulate a token
    try {
      // Simulate OAuth2 flow - in production, use real OIDC client
      const demoToken = 'demo_jwt_token_' + Date.now();
      localStorage.setItem('access_token', demoToken);
      
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
        <h1>iKfz - Vehicle Registration</h1>
        <p>Secure online vehicle registration system</p>
        <div className="login-info">
          <h2>Security Features:</h2>
          <ul>
            <li>✓ OAuth2/OIDC Authentication</li>
            <li>✓ GDPR Compliant</li>
            <li>✓ ISO 27001 Security Standards</li>
            <li>✓ No Personal Data in Logs</li>
            <li>✓ Encrypted Communication</li>
          </ul>
        </div>
        <button onClick={handleLogin} disabled={loading} className="login-button">
          {loading ? 'Logging in...' : 'Login with OAuth2/OIDC'}
        </button>
        <p className="demo-note">
          Note: This is a demo. In production, this would redirect to a real OAuth2/OIDC provider.
        </p>
      </div>
    </div>
  );
};

export default LoginPage;
