import React from 'react';
import { render, screen } from '@testing-library/react';
import App from './App';

test('renders iKfz application', () => {
  render(<App />);
  // Should render login page by default (no token)
  const loginElement = screen.getByText(/iKfz - Vehicle Registration/i);
  expect(loginElement).toBeInTheDocument();
});
