import React from 'react';
import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import HomePage from './pages/HomePage';
import GameCatalog from './pages/GameCatalog';
import ReservationsPage from './pages/ReservationsPage';
import OrdersPage from './pages/OrdersPage';

// Create a client for React Query
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      retry: 1,
      staleTime: 5 * 60 * 1000, // 5 minutes
    },
  },
});

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <div style={{ minHeight: '100vh', fontFamily: 'system-ui' }}>
          {/* Navigation Header */}
          <header style={{
            backgroundColor: '#2c3e50',
            color: 'white',
            padding: '1rem 2rem',
            boxShadow: '0 2px 4px rgba(0,0,0,0.1)'
          }}>
            <nav style={{ maxWidth: '1200px', margin: '0 auto' }}>
              <Link to="/" style={{ 
                color: 'white', 
                textDecoration: 'none', 
                fontSize: '1.5rem',
                fontWeight: 'bold',
                marginRight: '2rem'
              }}>
                Board Game Café
              </Link>
              <Link to="/games" style={{ color: 'white', textDecoration: 'none', marginRight: '1rem' }}>
                Games
              </Link>
              <Link to="/reservations" style={{ color: 'white', textDecoration: 'none', marginRight: '1rem' }}>
                Reservations
              </Link>
              <Link to="/orders" style={{ color: 'white', textDecoration: 'none' }}>
                Orders
              </Link>
            </nav>
          </header>

          {/* Main Content */}
          <main style={{ maxWidth: '1200px', margin: '0 auto' }}>
            <Routes>
              <Route path="/" element={<HomePage />} />
              <Route path="/games" element={<GameCatalog />} />
              <Route path="/reservations" element={<ReservationsPage />} />
              <Route path="/orders" element={<OrdersPage />} />
            </Routes>
          </main>
        </div>
      </BrowserRouter>
    </QueryClientProvider>
  );
}
