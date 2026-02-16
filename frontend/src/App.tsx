import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import TrackingPage from './pages/TrackingPage';
import ZahlungsSeite from './pages/ZahlungsSeite';
import './App.css';

function App() {
  return (
    <Router>
      <div className="app">
        <Routes>
          <Route path="/tracking/:trackingCode" element={<TrackingPage />} />
          <Route 
            path="/zahlung/:rechnungId" 
            element={<ZahlungsSeite rechnungId="demo-id" betrag={150.00} />} 
          />
          <Route path="/" element={
            <div className="home">
              <h1>🚗 KFZ-Zulassung Portal</h1>
              <p>Multi-Payment Integration & Live-Tracking System</p>
              <div className="info">
                <h2>Features:</h2>
                <ul>
                  <li>✅ Multi-Payment Support (Stripe, PayPal, Überweisung)</li>
                  <li>✅ QR-Code Live-Tracking</li>
                  <li>✅ Real-time Status Updates (SignalR)</li>
                  <li>✅ Email & SMS Notifications</li>
                  <li>✅ Mobile-First Design</li>
                </ul>
              </div>
            </div>
          } />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
