import React, { useState } from 'react';
import { useGames, GameFilters as Filters, GameDto } from '../api/useGames';
import { GameCard } from '../components/GameCard';
import { GameFilters } from '../components/GameFilters';
import { GameDetailModal } from '../components/GameDetailModal';

/**
 * GameCatalog page component displays the game catalog with filtering
 */
export default function GameCatalog() {
  const [filters, setFilters] = useState<Filters>({ pageSize: 20 });
  const [selectedGame, setSelectedGame] = useState<GameDto | null>(null);
  
  const { data: games, isLoading, error } = useGames(filters);

  const handleFilterChange = (newFilters: Filters) => {
    setFilters(newFilters);
  };

  const handleGameClick = (game: GameDto) => {
    setSelectedGame(game);
  };

  const handleCloseModal = () => {
    setSelectedGame(null);
  };

  return (
    <div
      style={{
        minHeight: '100vh',
        backgroundColor: '#f3f4f6',
        padding: '40px 20px',
      }}
    >
      <div
        style={{
          maxWidth: '1400px',
          margin: '0 auto',
        }}
      >
        {/* Page Header */}
        <div style={{ marginBottom: '32px', textAlign: 'center' }}>
          <h1
            data-testid="game-catalog-heading"
            style={{
              fontSize: '48px',
              fontWeight: 'bold',
              margin: '0 0 16px 0',
              color: '#1f2937',
              textShadow: '2px 2px 4px rgba(0, 0, 0, 0.1)',
            }}
          >
            🎲 Game Catalog
          </h1>
          <p style={{ fontSize: '18px', color: '#6b7280', margin: 0 }}>
            Browse our collection of board games and find your next adventure
          </p>
        </div>

        {/* Filters */}
        <GameFilters onFilterChange={handleFilterChange} />

        {/* Loading State */}
        {isLoading && (
          <div style={{ textAlign: 'center', padding: '60px 20px' }}>
            <div
              style={{
                fontSize: '48px',
                marginBottom: '16px',
              }}
            >
              🎲
            </div>
            <p style={{ fontSize: '18px', color: '#6b7280' }}>Loading games...</p>
          </div>
        )}

        {/* Error State */}
        {error && (
          <div
            style={{
              backgroundColor: '#fee2e2',
              border: '2px solid #ef4444',
              borderRadius: '12px',
              padding: '20px',
              textAlign: 'center',
            }}
          >
            <p style={{ fontSize: '18px', color: '#991b1b', margin: 0 }}>
              ⚠️ Error loading games. Please try again later.
            </p>
          </div>
        )}

        {/* No Results */}
        {!isLoading && !error && games && games.length === 0 && (
          <div
            data-testid="no-results-message"
            style={{
              textAlign: 'center',
              padding: '60px 20px',
              backgroundColor: '#ffffff',
              borderRadius: '12px',
              border: '2px dashed #d1d5db',
            }}
          >
            <div style={{ fontSize: '64px', marginBottom: '16px' }}>🔍</div>
            <h3 style={{ fontSize: '24px', fontWeight: '600', margin: '0 0 8px 0', color: '#1f2937' }}>
              No games found
            </h3>
            <p style={{ fontSize: '16px', color: '#6b7280', margin: 0 }}>
              Try adjusting your filters to see more results
            </p>
          </div>
        )}

        {/* Game Grid */}
        {!isLoading && !error && games && games.length > 0 && (
          <>
            <div
              style={{
                display: 'grid',
                gridTemplateColumns: 'repeat(auto-fill, minmax(280px, 1fr))',
                gap: '24px',
                marginBottom: '32px',
              }}
            >
              {games.map((game) => (
                <GameCard key={game.id} game={game} onClick={() => handleGameClick(game)} />
              ))}
            </div>

            {/* Results Count */}
            <div style={{ textAlign: 'center', padding: '20px' }}>
              <p style={{ fontSize: '16px', color: '#6b7280' }}>
                Showing {games.length} {games.length === 1 ? 'game' : 'games'}
              </p>
            </div>
          </>
        )}

        {/* Game Detail Modal */}
        {selectedGame && <GameDetailModal game={selectedGame} onClose={handleCloseModal} />}
      </div>
    </div>
  );
}
