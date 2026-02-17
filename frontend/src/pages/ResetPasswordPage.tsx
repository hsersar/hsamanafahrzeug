import React, { useState } from 'react';
import { Link } from 'react-router-dom';

const ResetPasswordPage: React.FC = () => {
  const [email, setEmail] = useState('');
  const [submitted, setSubmitted] = useState(false);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitted(true);
  };

  return (
    <div className="page reset-page">
      <div className="page-header">
        <div>
          <h1>Passwort zurücksetzen</h1>
          <p>Im Mock-Modus wird keine echte E-Mail versendet.</p>
        </div>
      </div>
      <div className="card">
        {submitted ? (
          <p>
            Wenn die E-Mail-Adresse bekannt ist, erhalten Sie in Kürze eine Nachricht
            mit weiteren Schritten.
          </p>
        ) : (
          <form onSubmit={handleSubmit} className="reset-form">
            <div className="form-group">
              <label htmlFor="email">E-Mail-Adresse</label>
              <input
                id="email"
                name="email"
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="name@beispiel.de"
                required
              />
            </div>
            <div className="form-footer">
              <button type="submit" className="primary-button">
                Link anfordern
              </button>
              <Link to="/login" className="secondary-button">
                Zurück zum Login
              </Link>
            </div>
          </form>
        )}
      </div>
    </div>
  );
};

export default ResetPasswordPage;
