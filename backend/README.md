# Activity Game: Comprehensive Backend Documentation

## Table of Contents
1. [Project Overview](#project-overview)
2. [Architecture](#architecture)
3. [Technology Stack](#technology-stack)
4. [Layer Breakdown](#layer-breakdown)
   - [Application Layer](#application-layer)
   - [Presentation Layer](#presentation-layer)
   - [Infrastructure Layer](#infrastructure-layer)
5. [Key Components](#key-components)
6. [Data Models](#data-models)
7. [API Endpoints](#api-endpoints)
8. [Server-Sent Events (SSE)](#server-sent-events-sse)
9. [Game Flow](#game-flow)
10. [Validation and Error Handling](#validation-and-error-handling)
11. [Database Configuration](#database-configuration)
12. [Docker Configuration](#docker-configuration)
13. [Development and Build Process](#development-and-build-process)

## Project Overview

The Activity Game is a multiplayer word-guessing game implemented using .NET 8 and following clean architecture principles. The backend is responsible for managing game logic, user authentication, and real-time communication between players using Server-Sent Events (SSE).

## Architecture

The project follows a clean architecture pattern with three main layers:

1. **Application Layer**: Contains business logic and use cases.
2. **Presentation Layer**: Handles HTTP requests and responses, including controllers, DTOs, and SSE endpoints.
3. **Infrastructure Layer**: Manages data access and external concerns.

## Technology Stack

- .NET 8
- C# 11
- Entity Framework Core
- Server-Sent Events (SSE) for real-time communication
- MS SQL Server for data storage
- Docker for containerization

## Layer Breakdown

### Application Layer

[Content remains largely the same]

### Presentation Layer

The Presentation layer handles the API endpoints and real-time communication. Key components include:

- **Controllers**: 
  - `GamesController`: Manages game-related API endpoints.
  - `AuthController`: Handles authentication and user management.
  - `GameEventsController`: Manages SSE connections and event streaming.

- **DTOs**: 
  - Request and Response objects for API communication.
  - Ensures structured data exchange between client and server

- **Services**:
  - `IGameEventService` and `GameEventService`: Manage SSE client connections and event broadcasting.

- **Program.cs**: Configuration and setup of the ASP.NET Core application.

### Infrastructure Layer

The Infrastructure layer manages data access and external concerns. Key components include:

- **DbContext**: 
  - `ApplicationDbContext`: Manages database operations.

- **Entity Configurations**: 
  - Configure database schema and relationships.

- **Data Providers**: 
  - `GameServiceDataProvider`: Implements game data access.
  - `UserServiceDataProvider`: Implements user data access.

## Key Components

1. **GameService**: Implements game logic, including creating games, adding players, updating settings, starting games, and ending turns.

2. **UserService**: Manages user-related operations such as creating, retrieving, and updating user information.

3. **GameValidator**: Validates game states, settings, and player actions.

4. **GamesController**: Handles HTTP requests for game-related operations.

5. **GameEventService**: Manages SSE client connections and broadcasts game events to connected clients.


## Data Models

Key entities include:

- **Game**: Represents a game session.
- **User**: Represents a player.
- **Round**: Represents a single round in the game.
- **Word**: Represents a word to be guessed.

## API Endpoints

### Authentication Endpoints

- **POST** `/api/auth/register`
  - Register a new user
  - Returns: `AuthResponseDto`

- **POST** `/api/auth/login`
  - Authenticate user and issue JWT
  - Returns: `AuthResponseDto`

### Game Management Endpoints

- **POST** `/api/games/create`
  - Create new game
  - Request: `CreateGameRequest`
  - Returns: `CreateGameResponse`

- **POST** `/api/games/{gameId}/join`
  - Join existing game
  - Returns: `ApiResponse`

- **PUT** `/api/games/{gameId}/settings`
  - Update game settings
  - Request: `UpdateGameSettingsRequest`
  - Returns: `ApiResponse`

## Server-Sent Events (SSE)

### Event Types
1. `GameStarted`
2. `TurnEnded`
3. `GameEnded`
4. `GameSettingsUpdated`

### Sample Event Format
```json
{
  "event": "TurnEnded",
  "data": {
    "gameId": "20930197-4612-4354-944b-e0658a20014f"
  }
}
```

## Game Flow

1. **Authentication**
   - User registers/logs in
   - Receives JWT token

2. **Game Setup**
   - Host creates game
   - Players join
   - Host configures settings

3. **Gameplay**
   - Game starts
   - Players take turns
   - Real-time updates via SSE

4. **Game End**
   - Score reached
   - Stats displayed

## Validation and Error Handling

### Custom Exceptions
```csharp
public class GameNotFoundException : Exception
{
    public GameNotFoundException(Guid gameId)
        : base($"Game with ID {gameId} not found")
    {
    }
}

public class UserNotFoundException : Exception
{
    public UserNotFoundException(Guid userId)
        : base($"User with ID {userId} not found")
    {
    }
}
```

### Global Error Handling
```csharp
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
}
```

## Database Configuration

- Uses Entity Framework Core with MS SQL Server
- Entity configurations define database schema and relationships

## Docker Configuration

- Multi-container setup with separate containers for the web application and SQL Server
- Environment variables used for configuration

## Development and Build Process

- Uses .NET 8 SDK for building and running the application
- Docker support for containerized development and deployment