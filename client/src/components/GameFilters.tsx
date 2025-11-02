import React, { useState, useEffect } from 'react';
import { GameFilters as Filters } from '../api/useGames';

interface GameFiltersProps {
  onFilterChange: (filters: Filters) => void;
}

/**
 * GameFilters component provides filtering controls for the game catalog
 */
export function GameFilters({ onFilterChange }: GameFiltersProps) {
  const [category, setCategory] = useState<string>('');
  const [minPlayers, setMinPlayers] = useState<number>(1);
  const [maxPlayers, setMaxPlayers] = useState<number>(20);
  const [minComplexity, setMinComplexity] = useState<number>(1.0);
  const [maxComplexity, setMaxComplexity] = useState<number>(5.0);
  const [search, setSearch] = useState<string>('');
  const [availableOnly, setAvailableOnly] = useState<boolean>(false);
  
  const applyFilters = React.useCallback(() => {
    const filters: Filters = {
      pageSize: 20,
    };
    
    if (category) {
      const categoryMap: Record<string, number> = {
        Strategy: 0,
        Party: 1,
        Family: 2,
        Cooperative: 3,
        Abstract: 4,
      };
      filters.category = categoryMap[category];
    }
    
    if (minPlayers > 1 || maxPlayers < 20) {
      filters.minPlayerCount = minPlayers;
      filters.maxPlayerCount = maxPlayers;
    }
    
    if (minComplexity > 1.0 || maxComplexity < 5.0) {
      filters.minComplexity = minComplexity;
      filters.maxComplexity = maxComplexity;
    }
    
    if (search.trim()) {
      filters.search = search.trim();
    }
    
    if (availableOnly) {
      filters.availableOnly = true;
    }
    
    onFilterChange(filters);
  }, [onFilterChange, category, minPlayers, maxPlayers, minComplexity, maxComplexity, search, availableOnly]);
  
  // Debounce search input
  useEffect(() => {
    const timer = setTimeout(() => {
      applyFilters();
    }, 300);
    
    return () => clearTimeout(timer);
  }, [search, applyFilters]);
  
  // Apply filters immediately for non-search changes
  useEffect(() => {
    applyFilters();
  }, [category, minPlayers, maxPlayers, minComplexity, maxComplexity, availableOnly, applyFilters]);
  
  const resetFilters = () => {
    setCategory('');
    setMinPlayers(1);
    setMaxPlayers(20);
    setMinComplexity(1.0);
    setMaxComplexity(5.0);
    setSearch('');
    setAvailableOnly(false);
  };

  return (
    <div
      style={{
        backgroundColor: '#f9fafb',
        border: '1px solid #e5e7eb',
        borderRadius: '12px',
        padding: '20px',
        marginBottom: '24px',
      }}
    >
      <h2 style={{ fontSize: '18px', fontWeight: 'bold', marginBottom: '16px', color: '#1f2937' }}>
        Filter Games
      </h2>

      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '16px' }}>
        {/* Search Input */}
        <div>
          <label
            htmlFor="search"
            style={{ display: 'block', fontSize: '14px', fontWeight: '500', marginBottom: '6px', color: '#374151' }}
          >
            Search by title
          </label>
          <input
            id="search"
            data-testid="game-search-input"
            type="text"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            placeholder="e.g., Catan"
            style={{
              width: '100%',
              padding: '8px 12px',
              border: '1px solid #d1d5db',
              borderRadius: '6px',
              fontSize: '14px',
            }}
          />
        </div>

        {/* Category Filter */}
        <div>
          <label
            htmlFor="category"
            style={{ display: 'block', fontSize: '14px', fontWeight: '500', marginBottom: '6px', color: '#374151' }}
          >
            Category
          </label>
          <select
            id="category"
            data-testid="category-filter"
            value={category}
            onChange={(e) => setCategory(e.target.value)}
            style={{
              width: '100%',
              padding: '8px 12px',
              border: '1px solid #d1d5db',
              borderRadius: '6px',
              fontSize: '14px',
              backgroundColor: '#ffffff',
            }}
          >
            <option value="">All Categories</option>
            <option value="Strategy" data-testid="category-option-strategy">Strategy</option>
            <option value="Party" data-testid="category-option-party">Party</option>
            <option value="Family" data-testid="category-option-family">Family</option>
            <option value="Cooperative" data-testid="category-option-cooperative">Cooperative</option>
            <option value="Abstract" data-testid="category-option-abstract">Abstract</option>
          </select>
        </div>

        {/* Player Count Range */}
        <div>
          <label
            htmlFor="player-count"
            style={{ display: 'block', fontSize: '14px', fontWeight: '500', marginBottom: '6px', color: '#374151' }}
          >
            Player Count: {minPlayers}-{maxPlayers}
          </label>
          <div data-testid="player-count-filter" style={{ display: 'flex', gap: '8px', alignItems: 'center' }}>
            <input
              id="min-players"
              type="range"
              min="1"
              max="20"
              value={minPlayers}
              onChange={(e) => setMinPlayers(Math.min(Number(e.target.value), maxPlayers))}
              style={{ flex: 1 }}
            />
            <input
              id="max-players"
              type="range"
              min="1"
              max="20"
              value={maxPlayers}
              onChange={(e) => setMaxPlayers(Math.max(Number(e.target.value), minPlayers))}
              style={{ flex: 1 }}
            />
          </div>
        </div>

        {/* Complexity Range */}
        <div>
          <label
            htmlFor="complexity"
            style={{ display: 'block', fontSize: '14px', fontWeight: '500', marginBottom: '6px', color: '#374151' }}
          >
            Complexity: {minComplexity.toFixed(1)}-{maxComplexity.toFixed(1)}
          </label>
          <div data-testid="complexity-filter" style={{ display: 'flex', gap: '8px', alignItems: 'center' }}>
            <input
              id="min-complexity"
              type="range"
              min="1.0"
              max="5.0"
              step="0.1"
              value={minComplexity}
              onChange={(e) => setMinComplexity(Math.min(Number(e.target.value), maxComplexity))}
              style={{ flex: 1 }}
            />
            <input
              id="max-complexity"
              type="range"
              min="1.0"
              max="5.0"
              step="0.1"
              value={maxComplexity}
              onChange={(e) => setMaxComplexity(Math.max(Number(e.target.value), minComplexity))}
              style={{ flex: 1 }}
            />
          </div>
        </div>

        {/* Available Only Checkbox */}
        <div style={{ display: 'flex', alignItems: 'center', marginTop: '28px' }}>
          <input
            id="available-only"
            data-testid="available-only-checkbox"
            type="checkbox"
            checked={availableOnly}
            onChange={(e) => setAvailableOnly(e.target.checked)}
            style={{ marginRight: '8px', width: '16px', height: '16px' }}
          />
          <label htmlFor="available-only" style={{ fontSize: '14px', fontWeight: '500', color: '#374151' }}>
            Available only
          </label>
        </div>

        {/* Reset Button */}
        <div style={{ display: 'flex', alignItems: 'center', marginTop: '28px' }}>
          <button
            onClick={resetFilters}
            style={{
              padding: '8px 16px',
              backgroundColor: '#6b7280',
              color: '#ffffff',
              border: 'none',
              borderRadius: '6px',
              fontSize: '14px',
              fontWeight: '500',
              cursor: 'pointer',
            }}
            onMouseEnter={(e) => {
              e.currentTarget.style.backgroundColor = '#4b5563';
            }}
            onMouseLeave={(e) => {
              e.currentTarget.style.backgroundColor = '#6b7280';
            }}
          >
            Reset Filters
          </button>
        </div>
      </div>
    </div>
  );
}
