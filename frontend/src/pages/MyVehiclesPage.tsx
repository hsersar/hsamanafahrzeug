import React, { useState, useEffect } from 'react';
import ApiService from '../services/api';
import { Vehicle } from '../types';

const MyVehiclesPage: React.FC = () => {
  const [vehicles, setVehicles] = useState<Vehicle[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadVehicles();
  }, []);

  const loadVehicles = async () => {
    try {
      setLoading(true);
      const data = await ApiService.getMyVehicles();
      setVehicles(data);
      setError(null);
    } catch (err) {
      setError('Fahrzeuge konnten nicht geladen werden');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <div className="loading">Vorgang wird geladen...</div>;
  }

  if (error) {
    return <div className="error">{error}</div>;
  }

  return (
    <div className="page my-vehicles-page">
      <div className="page-header">
        <div>
          <h1>Meine Fahrzeuge</h1>
          <p>Übersicht über die registrierten Fahrzeuge.</p>
        </div>
      </div>
      {vehicles.length === 0 ? (
        <div className="card">Derzeit sind keine Fahrzeuge registriert.</div>
      ) : (
        <div className="vehicles-list">
          {vehicles.map((vehicle) => (
            <div key={vehicle.id} className="vehicle-card">
              <div className="card-header">
                <div className="card-title">
                  {vehicle.brand} {vehicle.model} ({vehicle.year})
                </div>
                <span className="badge">Aktiv</span>
              </div>
              <div className="vehicle-details">
                <p>
                  <strong>Kennzeichen:</strong> {vehicle.licensePlate}
                </p>
                <p>
                  <strong>FIN:</strong> {vehicle.vin}
                </p>
                <p>
                  <strong>Farbe:</strong> {vehicle.color}
                </p>
                <p>
                  <strong>Erstzulassung:</strong>{' '}
                  {new Date(vehicle.firstRegistrationDate).toLocaleDateString()}
                </p>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default MyVehiclesPage;
