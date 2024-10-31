# Activity Game: Comprehensive Documentation and Workflow

## Table of Contents
1. [Project Overview](#project-overview)
2. [Technology Stack](#technology-stack)
3. [Frontend Architecture](#frontend-architecture)
4. [Backend Architecture](#backend-architecture)
5. [Authentication and Authorization](#authentication-and-authorization)
6. [Game Workflow](#game-workflow)
7. [State Management](#state-management)
8. [Real-time Communication](#real-time-communication)
9. [API Endpoints](#api-endpoints)
10. [Data Models](#data-models)
11. [Key Components](#key-components)
12. [Custom Hooks](#custom-hooks)
13. [Form Handling](#form-handling)
14. [Styling](#styling)
15. [Error Handling](#error-handling)
16. [Performance Considerations](#performance-considerations)
17. [Development and Build Process](#development-and-build-process)
18. [Future Improvements](#future-improvements)

## Project Overview

The Activity Game is a multiplayer word-guessing game where players take turns describing, drawing, or acting out words for other players to guess. The game is played in rounds, with players earning points for correct guesses. This project implements both the frontend and backend components of the game, providing a seamless and interactive gaming experience with real-time updates using Server-Sent Events (SSE).

## Technology Stack

### Frontend:
- React with TypeScript
- Vite (build tool)
- React Query (server state management)
- Context API (global state management)
- React Router (routing)
- Tailwind CSS with shadcn/ui components (styling)
- Formik with Yup (form handling and validation)
- @react-oauth/google (Google OAuth integration)
- `useSSE`: Custom hook for managing SSE connections and handling real-time updates


### Backend:
- ASP.NET Core
- MS SQL Server (database)
- SSE Endpoints: Manages real-time communication using Server-Sent Events
- Google OAuth (authentication)

## Frontend Architecture

The frontend is built using React with TypeScript, utilizing a component-based architecture. Key structural elements include:

- `src/` - Root source directory
    - `components/` - Reusable UI components
    - `pages/` - Main page components (Main, Lobby, Game, GameStats, Login)
    - `context/` - React Context providers (AuthContext, GameContext)
    - `hooks/` - Custom React hooks
    - `interfaces/` - TypeScript interfaces and types
    - `utils/` - Utility functions
    - `api/` - API configuration and axios instance
    - `App.tsx` - Main application component
    - `main.tsx` - Entry point of the application

Key Components:
- `Main`: Landing page for creating/joining games
- `Lobby`: Pre-game lobby for players to gather
- `Game`: Main game interface
- `GameStats`: Displays game statistics after the game ends
- `Login`: Handles user authentication
- `UpdateSettingsForm`: Allows users to update game settings
- `GameActionCard`: Reusable component for game actions
- `JoinGameDialog`: Dialog for joining a game

## Backend Architecture

The backend is built with ASP.NET Core, following a layered architecture:

- Presentation layer: Controllers (e.g., `GamesController`): Handle HTTP requests and responses
- Application layer: Services (e.g., `GameService`): Implement business logic
- Infrastructure layer: ServiceDataProviders (e.g., `GameServiceDataProvider`): Manages database operations using Entity Framework Core

Key Components:
- `GameController`: Handles game-related API endpoints
- `GameService`: Implements core game logic
- `ApplicationDbContext`: Manages database operations
- `GameEventsController`: Handles SSE connections and event streaming
- `GameEventService`: Manages SSE client connections and event broadcasting

## Authentication and Authorization

- Google OAuth is used for user authentication
- JWT tokens are used for API authorization
- `AuthContext` in the frontend manages authentication state
- `AuthProvider` wraps the application to provide authentication functionality
- Backend validates JWT tokens for protected endpoints

## Game Workflow

1. **User Authentication**
    - Users sign in using Google OAuth

2. **Main Page**
    - Users can create a new game or join an existing one
    - Game IDs are stored in localStorage for persistence

3. **Lobby**
    - Players gather before the game starts
    - Host can adjust game settings (timer, max score, enabled methods)
    - Players can see who has joined

4. **Gameplay**
    - System selects a random player and word
    - Active player describes/draws/acts the word
    - Other players attempt to guess
    - Points are awarded for correct guesses
    - Turn ends when word is guessed or time runs out

5. **Game End**
    - Game ends when a player reaches the max score or all rounds are completed
    - Final scores and statistics are displayed

## State Management

- React Query is used for server state management
- Context API (`AuthContext`, `GameContext`) is used for global state management
- Local state is used for UI-specific state (e.g., modals, forms)
- Automatic refetching of game state occurs every 5 seconds

## Real-time Communication

- SSE is used for real-time updates
- Custom `useSSE` hook manages the SSE connection and event handling

## API Endpoints

- `POST /api/games/create`: Create a new game
- `POST /api/games/{gameId}/join`: Join an existing game
- `GET /api/games/{gameId}`: Get game details
- `PUT /api/games/{gameId}/settings`: Update game settings
- `POST /api/games/{gameId}/start`: Start the game
- `POST /api/games/{gameId}/end-turn`: End the current turn
- `GET /api/GameEvents/{gameId}`: Establishes an SSE connection for real-time game events

## Data Models

### Frontend:
- `User`: Represents a user/player
  ```typescript
  interface User {
    id: string;
    username: string;
    email?: string;
    score: number | null;
    isHost: boolean | null;
  }
  ```
- `GameDetails`: Represents the current state of a game
  ```typescript
  interface GameDetails {
    id: string;
    host: User;
    timer: number;
    maxScore: number;
    players: User[];
    rounds: Round[];
    gameStatus: GameStatus;
    enabledMethods: MethodType[];
    currentRound: Round | null;
  }
  ```
- `Round`: Represents a single round in the game
  ```typescript
  interface Round {
    id: string;
    methodType: MethodType;
    word: string;
    activePlayerUsername: string;
    roundWinnerId?: number;
  }
  ```
- `Word`: Represents a word to be guessed
- `MethodType`: Enum for different word-guessing methods
  ```typescript
  enum MethodType {
    Drawing = 0,
    Description = 1,
    Mimic = 2,
  }
  ```
- `GameStatus`: Enum for game status
  ```typescript
  enum GameStatus {
    Waiting = 0,
    InProgress = 1,
    Finished = 2,
  }
  ```
  
## Custom Hooks

- `useAuth`: Manages authentication state
- `useGame`: Manages game state
- `useSSE`: Handles real-time communication using Server-Sent Events
- `useCreateGame`: Handles game creation
- `useJoinGame`: Handles joining a game
- `useUpdateSettings`: Handles updating game settings
- `useStartGame`: Handles starting a game
- `useGameDetails`: Fetches and manages game details
- `useEndTurn`: Handles ending a turn

## Form Handling

- Formik is used for form management
- Yup is used for form validation
- Custom form components (e.g., `MaxScoreField`, `TimerField`, `MethodSelection`) are used in the `UpdateSettingsForm`

## Styling

- Tailwind CSS is used for utility-first styling
- shadcn/ui components are used for pre-styled UI elements
- Custom styles are applied using Tailwind classes and occasional custom CSS

## Error Handling

- Toast notifications are used for user-friendly error messages
- Error boundaries for React components are planned for implementation
- Centralized error handling in API calls using React Query's error handling

## Performance Considerations

- Vite is used as the build tool for faster development and optimized production builds
- React Query provides efficient caching and refetching strategies
- Debouncing and throttling for frequent operations are planned for implementation

## Development and Build Process

- Vite is used for both development and production builds
- Development server runs on port 5173
- Proxy settings in `vite.config.ts` handle API and SignalR connections:
```typescript
export default defineConfig({
  // ... other config
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: 'https://localhost:8082',
        changeOrigin: true,
        secure: false,
      }
    },
  },
  // ... other config
})
```~~~~


## Future Improvements

- Implement a drawing canvas for the "draw" method
- Add support for custom word lists
- Implement a spectator mode
- Add game statistics and leaderboards
- Enhance accessibility features
- Implement error boundaries for better error handling
- Add more comprehensive unit and integration tests
