# Board Game Café - Frontend Client

React + TypeScript frontend for the Board Game Café management system.

## Tech Stack

- **React 18** - UI framework
- **TypeScript** - Type safety
- **Vite** - Build tool and dev server
- **TanStack Query** (React Query) - Server state management
- **React Router** - Client-side routing
- **Axios** - HTTP client
- **ESLint + Prettier** - Code quality and formatting
- **Playwright** - E2E testing

## Getting Started

### Prerequisites

- Node.js 20+ LTS
- npm or yarn

### Installation

```bash
npm install
```

### Development

Start the development server:

```bash
npm run dev
```

The app will be available at http://localhost:5173

The dev server proxies API requests to the backend at http://localhost:5000

### Build

Build for production:

```bash
npm run build
```

Preview the production build:

```bash
npm run preview
```

### Linting & Formatting

Run ESLint:

```bash
npm run lint
```

Format code with Prettier:

```bash
npm run format
```

### Testing

Run E2E tests with Playwright:

```bash
npm run test:e2e
```

Run tests in UI mode:

```bash
npm run test:e2e:ui
```

## Project Structure

```
client/
├── src/
│   ├── api/              # API client and React Query hooks
│   │   ├── client.ts     # Axios instance with interceptors
│   │   ├── games.ts      # Game-related API hooks
│   │   ├── reservations.ts
│   │   └── orders.ts
│   ├── components/       # Reusable UI components
│   ├── pages/            # Route components
│   │   ├── HomePage.tsx
│   │   ├── GamesPage.tsx
│   │   ├── ReservationsPage.tsx
│   │   └── OrdersPage.tsx
│   ├── types/            # TypeScript interfaces
│   │   ├── game.ts
│   │   ├── reservation.ts
│   │   └── order.ts
│   ├── hooks/            # Custom React hooks
│   ├── App.tsx           # Root component with routing
│   └── main.tsx          # Entry point
├── tests/e2e/            # Playwright E2E tests
├── vite.config.ts        # Vite configuration
├── tsconfig.json         # TypeScript configuration
├── eslint.config.js      # ESLint configuration
└── .prettierrc           # Prettier configuration
```

## API Integration

### React Query Setup

The app uses TanStack Query for server state management with:

- Automatic caching and refetching
- Optimistic updates
- 5-minute stale time
- 1 retry on failure

### API Client

All API requests go through the configured Axios instance in `src/api/client.ts`:

- Base URL: `/api/v1` (proxied to backend)
- 10-second timeout
- Automatic error handling
- Request/response interceptors for auth and logging

### Using API Hooks

Example usage:

```tsx
import { useGames, useCreateGame } from './api/games';

function GameList() {
  const { data: games, isLoading, error } = useGames();
  const createGame = useCreateGame();

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error: {error.message}</div>;

  return (
    <div>
      {games?.map(game => (
        <div key={game.id}>{game.title}</div>
      ))}
    </div>
  );
}
```

## Routing

The app uses React Router v6 with the following routes:

- `/` - Home page
- `/games` - Game catalog (Issue #15)
- `/reservations` - Table reservations (Issue #18)
- `/orders` - Food and beverage orders

## TypeScript Types

All DTOs match the backend C# models:

- `Game` - Board game catalog item
- `Reservation` - Table reservation
- `Order` - Food/beverage order
- `MenuItem` - Menu item

## Development Guidelines

1. **Components**: Create reusable components in `src/components/`
2. **Pages**: Route-level components go in `src/pages/`
3. **API Hooks**: Use React Query hooks for all API calls
4. **Types**: Define TypeScript interfaces in `src/types/`
5. **Styling**: Use inline styles or CSS modules (to be decided)
6. **Linting**: Run `npm run lint` before committing
7. **Formatting**: Use Prettier for consistent formatting

## Next Steps

- [ ] Implement Game Catalog UI (Issue #15)
- [ ] Implement Reservation Flow (Issue #18)
- [ ] Add authentication
- [ ] Add UI component library (TailwindCSS or MUI)
- [ ] Add loading states and error boundaries
- [ ] Implement optimistic updates for mutations
