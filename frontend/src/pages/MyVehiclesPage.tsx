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
      setError('Failed to load vehicles');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <div className="loading">Loading...</div>;
  }

  if (error) {
    return <div className="error">{error}</div>;
  }

  return (
    <div className="my-vehicles-page">
      <h1>My Vehicles</h1>
      {vehicles.length === 0 ? (
        <p>No vehicles registered yet.</p>
      ) : (
        <div className="vehicles-list">
          {vehicles.map((vehicle) => (
            <div key={vehicle.id} className="vehicle-card">
              <h3>
                {vehicle.brand} {vehicle.model} ({vehicle.year})
              </h3>
              <div className="vehicle-details">
                <p>
                  <strong>License Plate:</strong> {vehicle.licensePlate}
                </p>
                <p>
                  <strong>VIN:</strong> {vehicle.vin}
                </p>
                <p>
                  <strong>Color:</strong> {vehicle.color}
                </p>
                <p>
                  <strong>First Registration:</strong>{' '}
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
