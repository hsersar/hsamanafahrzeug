import React, { useState } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate, Link } from 'react-router-dom';
import './App.css';
import Navigation from './components/Navigation';
import HomePage from './pages/HomePage';
import LoginPage from './pages/LoginPage';
import MyVehiclesPage from './pages/MyVehiclesPage';
import MyRequestsPage from './pages/MyRequestsPage';
import NewRegistrationPage from './pages/NewRegistrationPage';
import SearchPage from './pages/SearchPage';
import ProfilePage from './pages/ProfilePage';
import HelpPage from './pages/HelpPage';
import ImpressumPage from './pages/ImpressumPage';
import AgbPage from './pages/AgbPage';
import DatenschutzPage from './pages/DatenschutzPage';
import DatenschutzeinstellungenPage from './pages/DatenschutzeinstellungenPage';
import ResetPasswordPage from './pages/ResetPasswordPage';
import SuperAdminPage from './pages/SuperAdminPage';
import StandortAdminPage from './pages/StandortAdminPage';
import MitarbeiterPage from './pages/MitarbeiterPage';

import VollmachtenPage from './pages/VollmachtenPage';
import BescheidePage from './pages/BescheidePage';
import DokumentePage from './pages/DokumentePage';
import CompanyProfilePage from './pages/CompanyProfilePage';

import RechnungenPage from './pages/RechnungenPage';
import OrderDetailPage from './pages/OrderDetailPage';
import AnnouncementBanner from './components/AnnouncementBanner';

const AppFooter: React.FC = () => (
  <footer className="app-footer">
    <div className="footer-links">
      <span className="footer-copy">© 2026 AMANAKFZ Zulassungsportal GmbH Wuppertal</span>
      <Link to="/impressum">Impressum</Link>
      <Link to="/agb">AGB</Link>
      <Link to="/datenschutz">Datenschutz</Link>
      <Link to="/datenschutzeinstellungen">Datenschutzeinstellungen</Link>
    </div>
  </footer>
);

function App() {
  const isAuthenticated = () => {
    return !!localStorage.getItem('access_token');
  };

  const getRole = () => localStorage.getItem('user_role') || 'Kunde';

  const [isSidebarCollapsed, setIsSidebarCollapsed] = useState(false);

  const ProtectedRoute: React.FC<{ children: React.ReactElement }> = ({ children }) => {
    return isAuthenticated() ? children : <Navigate to="/login" />;
  };

  const RoleRoute: React.FC<{ children: React.ReactElement; roles: string[] }> = ({
    children,
    roles,
  }) => {
    return roles.includes(getRole()) ? children : <Navigate to="/" />;
  };

  return (
    <Router>
      <div className="App">
        {isAuthenticated() ? (
          <div className={`app-shell ${isSidebarCollapsed ? 'is-collapsed' : ''}`}>
            <Navigation
              isSidebarCollapsed={isSidebarCollapsed}
              onToggleSidebar={() => setIsSidebarCollapsed((prev) => !prev)}
            />
            <main className="main-content">
              <AnnouncementBanner />
              <Routes>
                <Route
                  path="/"
                  element={
                    <ProtectedRoute>
                      <HomePage />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/my-vehicles"
                  element={
                    <ProtectedRoute>
                      <MyVehiclesPage />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/my-requests"
                  element={
                    <ProtectedRoute>
                      <MyRequestsPage />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/new-registration"
                  element={
                    <ProtectedRoute>
                      <NewRegistrationPage />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/search"
                  element={
                    <ProtectedRoute>
                      <SearchPage />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/profile"
                  element={
                    <ProtectedRoute>
                      <ProfilePage />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/personal-data"
                  element={<Navigate to="/profile?tab=stammdaten" replace />}
                />
                <Route
                  path="/vollmachten"
                  element={
                    <ProtectedRoute>
                      <VollmachtenPage />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/bescheide"
                  element={
                    <ProtectedRoute>
                      <BescheidePage />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/dokumente"
                  element={
                    <ProtectedRoute>
                      <DokumentePage />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/company-profile"
                  element={
                    <ProtectedRoute>
                      <CompanyProfilePage />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/payment-methods"
                  element={<Navigate to="/profile?tab=zahlungsarten" replace />}
                />
                <Route
                  path="/rechnungen"
                  element={
                    <ProtectedRoute>
                      <RechnungenPage />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/my-requests/:id"
                  element={
                    <ProtectedRoute>
                      <OrderDetailPage />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/help"
                  element={
                    <ProtectedRoute>
                      <HelpPage />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/admin/super"
                  element={
                    <ProtectedRoute>
                      <RoleRoute roles={["SuperAdmin"]}>
                        <SuperAdminPage />
                      </RoleRoute>
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/admin/standort"
                  element={
                    <ProtectedRoute>
                      <RoleRoute roles={["StandortAdmin", "SuperAdmin"]}>
                        <StandortAdminPage />
                      </RoleRoute>
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/mitarbeiter/inbox"
                  element={
                    <ProtectedRoute>
                      <RoleRoute roles={["Mitarbeiter", "StandortAdmin", "SuperAdmin"]}>
                        <MitarbeiterPage />
                      </RoleRoute>
                    </ProtectedRoute>
                  }
                />
                <Route path="/reset-password" element={<ResetPasswordPage />} />
                <Route path="/impressum" element={<ImpressumPage />} />
                <Route path="/agb" element={<AgbPage />} />
                <Route path="/datenschutz" element={<DatenschutzPage />} />
                <Route
                  path="/datenschutzeinstellungen"
                  element={<DatenschutzeinstellungenPage />}
                />
              </Routes>
              <AppFooter />
            </main>
          </div>
        ) : (
          <main className="main-content main-content--auth">
            <Routes>
              <Route path="/login" element={<LoginPage />} />
              <Route path="/reset-password" element={<ResetPasswordPage />} />
              <Route path="/impressum" element={<ImpressumPage />} />
              <Route path="/agb" element={<AgbPage />} />
              <Route path="/datenschutz" element={<DatenschutzPage />} />
              <Route
                path="/datenschutzeinstellungen"
                element={<DatenschutzeinstellungenPage />}
              />
              <Route path="*" element={<Navigate to="/login" />} />
            </Routes>
            <AppFooter />
          </main>
        )}
      </div>
    </Router>
  );
}

export default App;
