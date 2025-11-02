import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import GameCatalog from './pages/GameCatalog';

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Navigate to="/games" replace />} />
        <Route path="/games" element={<GameCatalog />} />
      </Routes>
    </BrowserRouter>
  );
}
