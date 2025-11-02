import React from 'react';
import { Link } from 'react-router-dom';

export default function HomePage() {
  return (
    <div style={{ fontFamily: 'system-ui', padding: '2rem' }}>
      <h1>Welcome to Board Game Café</h1>
      <p>Your destination for games, food, and fun!</p>
      
      <nav style={{ marginTop: '2rem' }}>
        <ul style={{ listStyle: 'none', padding: 0 }}>
          <li style={{ marginBottom: '1rem' }}>
            <Link to="/games" style={{ fontSize: '1.2rem', color: '#0066cc' }}>
              Browse Games
            </Link>
          </li>
          <li style={{ marginBottom: '1rem' }}>
            <Link to="/reservations" style={{ fontSize: '1.2rem', color: '#0066cc' }}>
              Make a Reservation
            </Link>
          </li>
          <li style={{ marginBottom: '1rem' }}>
            <Link to="/orders" style={{ fontSize: '1.2rem', color: '#0066cc' }}>
              Order Food & Drinks
            </Link>
          </li>
        </ul>
      </nav>
    </div>
  );
}
