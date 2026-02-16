import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import RechnungsListe from './pages/RechnungsListe';
import RechnungErstellen from './components/rechnung/RechnungErstellen';
import './App.css';

function App() {
  return (
    <Router>
      <div className="App">
        <header style={{ 
          backgroundColor: '#343a40', 
          color: 'white', 
          padding: '16px 0',
          marginBottom: '24px'
        }}>
          <div style={{ maxWidth: '1200px', margin: '0 auto', padding: '0 20px' }}>
            <h1 style={{ margin: 0 }}>Fahrzeug Zulassung - ZUGFeRD E-Rechnung</h1>
          </div>
        </header>

        <Routes>
          <Route path="/" element={<Navigate to="/rechnungen" replace />} />
          <Route path="/rechnungen" element={<RechnungsListe />} />
          <Route path="/rechnungen/neu" element={<RechnungErstellen />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
