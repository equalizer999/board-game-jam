import React from 'react';
import { GameDto } from '../api/useGames';

interface GameCardProps {
  game: GameDto;
  onClick: () => void;
}

/**
 * GameCard component displays a single game in the catalog
 * with game-themed styling reminiscent of board game boxes
 */
export function GameCard({ game, onClick }: GameCardProps) {
  const categoryColors: Record<string, string> = {
    Strategy: '#3b82f6',
    Party: '#ef4444',
    Family: '#10b981',
    Cooperative: '#f59e0b',
    Abstract: '#8b5cf6',
  };

  const getCategoryColor = () => categoryColors[game.category] || '#6b7280';
  
  // Generate a test ID based on the game title
  const testId = game.title.toLowerCase().replace(/\s+/g, '-');

  return (
    <div
      data-testid="game-card"
      data-testid-title={testId}
      onClick={onClick}
      style={{
        border: `3px solid ${getCategoryColor()}`,
        borderRadius: '12px',
        padding: '16px',
        cursor: 'pointer',
        backgroundColor: '#ffffff',
        boxShadow: '0 4px 6px rgba(0, 0, 0, 0.1)',
        transition: 'transform 0.2s, box-shadow 0.2s',
        height: '100%',
        display: 'flex',
        flexDirection: 'column',
      }}
      onMouseEnter={(e) => {
        e.currentTarget.style.transform = 'translateY(-4px)';
        e.currentTarget.style.boxShadow = '0 8px 12px rgba(0, 0, 0, 0.15)';
      }}
      onMouseLeave={(e) => {
        e.currentTarget.style.transform = 'translateY(0)';
        e.currentTarget.style.boxShadow = '0 4px 6px rgba(0, 0, 0, 0.1)';
      }}
    >
      {/* Game Image */}
      <div
        data-testid="game-image"
        style={{
          width: '100%',
          height: '200px',
          backgroundColor: '#e5e7eb',
          borderRadius: '8px',
          marginBottom: '12px',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          overflow: 'hidden',
        }}
      >
        {game.imageUrl ? (
          <img
            src={game.imageUrl}
            alt={game.title}
            style={{
              width: '100%',
              height: '100%',
              objectFit: 'cover',
            }}
          />
        ) : (
          <span style={{ fontSize: '48px', color: '#9ca3af' }}>🎲</span>
        )}
      </div>

      {/* Game Title */}
      <h3
        data-testid="game-title"
        style={{
          fontSize: '18px',
          fontWeight: 'bold',
          margin: '0 0 8px 0',
          color: '#1f2937',
        }}
      >
        {game.title}
      </h3>

      {/* Publisher */}
      <p
        style={{
          fontSize: '14px',
          color: '#6b7280',
          margin: '0 0 12px 0',
        }}
      >
        {game.publisher}
      </p>

      {/* Category Badge */}
      <div
        data-testid="game-category"
        style={{
          display: 'inline-block',
          backgroundColor: getCategoryColor(),
          color: '#ffffff',
          padding: '4px 12px',
          borderRadius: '16px',
          fontSize: '12px',
          fontWeight: '600',
          marginBottom: '12px',
          width: 'fit-content',
        }}
      >
        {game.category}
      </div>

      {/* Game Info */}
      <div style={{ marginTop: 'auto' }}>
        {/* Player Count with Meeple Icon */}
        <div
          data-testid="game-players"
          style={{
            display: 'flex',
            alignItems: 'center',
            marginBottom: '8px',
            fontSize: '14px',
            color: '#374151',
          }}
        >
          <span style={{ marginRight: '6px', fontSize: '16px' }}>👤</span>
          <span>
            {game.minPlayers === game.maxPlayers
              ? `${game.minPlayers} players`
              : `${game.minPlayers}-${game.maxPlayers} players`}
          </span>
        </div>

        {/* Play Time */}
        <div
          style={{
            display: 'flex',
            alignItems: 'center',
            marginBottom: '8px',
            fontSize: '14px',
            color: '#374151',
          }}
        >
          <span style={{ marginRight: '6px', fontSize: '16px' }}>⏱️</span>
          <span>{game.playTimeMinutes} min</span>
        </div>

        {/* Complexity with Dice Icon */}
        <div
          data-testid="game-complexity"
          style={{
            display: 'flex',
            alignItems: 'center',
            marginBottom: '8px',
            fontSize: '14px',
            color: '#374151',
          }}
        >
          <span style={{ marginRight: '6px', fontSize: '16px' }}>🎲</span>
          <span>Complexity: {game.complexity.toFixed(1)}/5.0</span>
        </div>

        {/* Daily Rental Fee */}
        <div
          style={{
            display: 'flex',
            alignItems: 'center',
            marginBottom: '8px',
            fontSize: '14px',
            fontWeight: '600',
            color: '#374151',
          }}
        >
          <span style={{ marginRight: '6px', fontSize: '16px' }}>💰</span>
          <span>${game.dailyRentalFee.toFixed(2)}/day</span>
        </div>

        {/* Availability Status */}
        <div
          data-testid="game-availability"
          style={{
            display: 'inline-block',
            padding: '6px 12px',
            borderRadius: '6px',
            fontSize: '12px',
            fontWeight: '600',
            backgroundColor: game.isAvailable ? '#d1fae5' : '#fee2e2',
            color: game.isAvailable ? '#065f46' : '#991b1b',
            marginTop: '8px',
          }}
        >
          {game.isAvailable
            ? `✓ Available (${game.copiesOwned - game.copiesInUse}/${game.copiesOwned})`
            : '✗ All copies in use'}
        </div>
      </div>
    </div>
  );
}
