import React from 'react';
import { GameDto } from '../api/useGames';

interface GameDetailModalProps {
  game: GameDto;
  onClose: () => void;
}

/**
 * GameDetailModal displays full game details in a modal overlay
 */
export function GameDetailModal({ game, onClose }: GameDetailModalProps) {
  const categoryColors: Record<string, string> = {
    Strategy: '#3b82f6',
    Party: '#ef4444',
    Family: '#10b981',
    Cooperative: '#f59e0b',
    Abstract: '#8b5cf6',
  };

  const getCategoryColor = () => categoryColors[game.category] || '#6b7280';

  return (
    <div
      data-testid="game-detail-modal"
      style={{
        position: 'fixed',
        top: 0,
        left: 0,
        right: 0,
        bottom: 0,
        backgroundColor: 'rgba(0, 0, 0, 0.5)',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        zIndex: 1000,
        padding: '20px',
      }}
      onClick={onClose}
    >
      <div
        style={{
          backgroundColor: '#ffffff',
          borderRadius: '16px',
          maxWidth: '800px',
          width: '100%',
          maxHeight: '90vh',
          overflow: 'auto',
          boxShadow: '0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)',
        }}
        onClick={(e) => e.stopPropagation()}
      >
        {/* Modal Header */}
        <div
          style={{
            padding: '24px',
            borderBottom: '1px solid #e5e7eb',
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'flex-start',
          }}
        >
          <div style={{ flex: 1 }}>
            <h2
              data-testid="modal-game-title"
              style={{
                fontSize: '28px',
                fontWeight: 'bold',
                margin: '0 0 8px 0',
                color: '#1f2937',
              }}
            >
              {game.title}
            </h2>
            <p style={{ fontSize: '16px', color: '#6b7280', margin: 0 }}>by {game.publisher}</p>
          </div>
          <button
            data-testid="close-modal-button"
            onClick={onClose}
            style={{
              padding: '8px',
              backgroundColor: 'transparent',
              border: 'none',
              cursor: 'pointer',
              fontSize: '24px',
              color: '#6b7280',
              lineHeight: 1,
            }}
            onMouseEnter={(e) => {
              e.currentTarget.style.color = '#1f2937';
            }}
            onMouseLeave={(e) => {
              e.currentTarget.style.color = '#6b7280';
            }}
          >
            ✕
          </button>
        </div>

        {/* Modal Body */}
        <div style={{ padding: '24px' }}>
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 2fr', gap: '24px' }}>
            {/* Game Image */}
            <div>
              <div
                style={{
                  width: '100%',
                  aspectRatio: '1',
                  backgroundColor: '#e5e7eb',
                  borderRadius: '12px',
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                  overflow: 'hidden',
                  border: `3px solid ${getCategoryColor()}`,
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
                  <span style={{ fontSize: '72px', color: '#9ca3af' }}>🎲</span>
                )}
              </div>

              {/* Category Badge */}
              <div
                style={{
                  marginTop: '16px',
                  backgroundColor: getCategoryColor(),
                  color: '#ffffff',
                  padding: '8px 16px',
                  borderRadius: '20px',
                  fontSize: '14px',
                  fontWeight: '600',
                  textAlign: 'center',
                }}
              >
                {game.category}
              </div>
            </div>

            {/* Game Details */}
            <div>
              {/* Description */}
              {game.description && (
                <div style={{ marginBottom: '24px' }}>
                  <h3
                    style={{
                      fontSize: '18px',
                      fontWeight: '600',
                      marginBottom: '8px',
                      color: '#1f2937',
                    }}
                  >
                    Description
                  </h3>
                  <p style={{ fontSize: '14px', color: '#4b5563', lineHeight: '1.6' }}>
                    {game.description}
                  </p>
                </div>
              )}

              {/* Game Metadata */}
              <div style={{ marginBottom: '24px' }}>
                <h3
                  style={{
                    fontSize: '18px',
                    fontWeight: '600',
                    marginBottom: '12px',
                    color: '#1f2937',
                  }}
                >
                  Game Details
                </h3>
                <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px' }}>
                  <div style={{ display: 'flex', alignItems: 'center' }}>
                    <span style={{ marginRight: '8px', fontSize: '18px' }}>👤</span>
                    <span style={{ fontSize: '14px', color: '#374151' }}>
                      {game.minPlayers === game.maxPlayers
                        ? `${game.minPlayers} players`
                        : `${game.minPlayers}-${game.maxPlayers} players`}
                    </span>
                  </div>
                  <div style={{ display: 'flex', alignItems: 'center' }}>
                    <span style={{ marginRight: '8px', fontSize: '18px' }}>⏱️</span>
                    <span style={{ fontSize: '14px', color: '#374151' }}>
                      {game.playTimeMinutes} minutes
                    </span>
                  </div>
                  <div style={{ display: 'flex', alignItems: 'center' }}>
                    <span style={{ marginRight: '8px', fontSize: '18px' }}>🎲</span>
                    <span style={{ fontSize: '14px', color: '#374151' }}>
                      Complexity: {game.complexity.toFixed(1)}/5.0
                    </span>
                  </div>
                  <div style={{ display: 'flex', alignItems: 'center' }}>
                    <span style={{ marginRight: '8px', fontSize: '18px' }}>👶</span>
                    <span style={{ fontSize: '14px', color: '#374151' }}>
                      Age: {game.ageRating}+
                    </span>
                  </div>
                  <div style={{ display: 'flex', alignItems: 'center' }}>
                    <span style={{ marginRight: '8px', fontSize: '18px' }}>💰</span>
                    <span style={{ fontSize: '14px', color: '#374151' }}>
                      ${game.dailyRentalFee.toFixed(2)}/day
                    </span>
                  </div>
                </div>
              </div>

              {/* Availability Status */}
              <div style={{ marginBottom: '24px' }}>
                <h3
                  style={{
                    fontSize: '18px',
                    fontWeight: '600',
                    marginBottom: '8px',
                    color: '#1f2937',
                  }}
                >
                  Availability
                </h3>
                <div
                  style={{
                    padding: '12px 16px',
                    borderRadius: '8px',
                    backgroundColor: game.isAvailable ? '#d1fae5' : '#fee2e2',
                    border: `2px solid ${game.isAvailable ? '#10b981' : '#ef4444'}`,
                  }}
                >
                  <div
                    style={{
                      fontSize: '16px',
                      fontWeight: '600',
                      color: game.isAvailable ? '#065f46' : '#991b1b',
                      marginBottom: '4px',
                    }}
                  >
                    {game.isAvailable ? '✓ Available for Checkout' : '✗ All Copies In Use'}
                  </div>
                  <div
                    style={{ fontSize: '14px', color: game.isAvailable ? '#047857' : '#991b1b' }}
                  >
                    {game.copiesOwned - game.copiesInUse} of {game.copiesOwned} copies available
                  </div>
                </div>
              </div>

              {/* Action Buttons */}
              <div style={{ display: 'flex', gap: '12px' }}>
                <button
                  style={{
                    flex: 1,
                    padding: '12px 24px',
                    backgroundColor: '#3b82f6',
                    color: '#ffffff',
                    border: 'none',
                    borderRadius: '8px',
                    fontSize: '16px',
                    fontWeight: '600',
                    cursor: 'pointer',
                  }}
                  onMouseEnter={(e) => {
                    e.currentTarget.style.backgroundColor = '#2563eb';
                  }}
                  onMouseLeave={(e) => {
                    e.currentTarget.style.backgroundColor = '#3b82f6';
                  }}
                  onClick={() => alert('Reservation flow not yet implemented')}
                >
                  Reserve a Table
                </button>
                <button
                  style={{
                    flex: 1,
                    padding: '12px 24px',
                    backgroundColor: game.isAvailable ? '#10b981' : '#d1d5db',
                    color: '#ffffff',
                    border: 'none',
                    borderRadius: '8px',
                    fontSize: '16px',
                    fontWeight: '600',
                    cursor: game.isAvailable ? 'pointer' : 'not-allowed',
                  }}
                  disabled={!game.isAvailable}
                  onMouseEnter={(e) => {
                    if (game.isAvailable) {
                      e.currentTarget.style.backgroundColor = '#059669';
                    }
                  }}
                  onMouseLeave={(e) => {
                    if (game.isAvailable) {
                      e.currentTarget.style.backgroundColor = '#10b981';
                    }
                  }}
                  onClick={() => alert('Checkout flow not yet implemented')}
                >
                  Checkout Game
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
