import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import ApiService from '../services/api';
import { CreateRegistrationRequestDto } from '../types';

const NewRegistrationPage: React.FC = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState<CreateRegistrationRequestDto>({
    vin: '',
    requestedLicensePlate: '',
    brand: '',
    model: '',
    year: new Date().getFullYear(),
    color: '',
    firstRegistrationDate: new Date().toISOString().split('T')[0],
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: name === 'year' ? parseInt(value) || 0 : value,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      await ApiService.createRegistrationRequest(formData);
      navigate('/my-requests');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to submit registration request');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="new-registration-page">
      <h1>New Vehicle Registration</h1>
      {error && <div className="error">{error}</div>}
      <form onSubmit={handleSubmit} className="registration-form">
        <div className="form-group">
          <label htmlFor="vin">VIN (17 characters):</label>
          <input
            type="text"
            id="vin"
            name="vin"
            value={formData.vin}
            onChange={handleChange}
            maxLength={17}
            minLength={17}
            required
          />
        </div>

        <div className="form-group">
          <label htmlFor="requestedLicensePlate">Requested License Plate:</label>
          <input
            type="text"
            id="requestedLicensePlate"
            name="requestedLicensePlate"
            value={formData.requestedLicensePlate}
            onChange={handleChange}
            maxLength={15}
            required
          />
        </div>

        <div className="form-group">
          <label htmlFor="brand">Brand:</label>
          <input
            type="text"
            id="brand"
            name="brand"
            value={formData.brand}
            onChange={handleChange}
            maxLength={100}
            required
          />
        </div>

        <div className="form-group">
          <label htmlFor="model">Model:</label>
          <input
            type="text"
            id="model"
            name="model"
            value={formData.model}
            onChange={handleChange}
            maxLength={100}
            required
          />
        </div>

        <div className="form-group">
          <label htmlFor="year">Year:</label>
          <input
            type="number"
            id="year"
            name="year"
            value={formData.year}
            onChange={handleChange}
            min={1900}
            max={2100}
            required
          />
        </div>

        <div className="form-group">
          <label htmlFor="color">Color:</label>
          <input
            type="text"
            id="color"
            name="color"
            value={formData.color}
            onChange={handleChange}
            maxLength={50}
            required
          />
        </div>

        <div className="form-group">
          <label htmlFor="firstRegistrationDate">First Registration Date:</label>
          <input
            type="date"
            id="firstRegistrationDate"
            name="firstRegistrationDate"
            value={formData.firstRegistrationDate}
            onChange={handleChange}
            required
          />
        </div>

        <button type="submit" disabled={loading}>
          {loading ? 'Submitting...' : 'Submit Registration Request'}
        </button>
      </form>
    </div>
  );
};

export default NewRegistrationPage;
