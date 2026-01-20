# Pokemon API

A RESTful API built with .NET 10 and Entity Framework Core for managing Pokemon trainers, their party Pokemon, and PC Box storage system.

---

## Tech Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| .NET | 10.0 | Runtime Framework |
| Entity Framework Core | 10.0.2 | ORM |
| PostgreSQL | - | Database |
| Npgsql | 10.0.0 | PostgreSQL Provider |

---

## Features

- RESTful API endpoints for trainers and Pokemon management
- PC Box storage system with Pokemon transfer functionality
- CORS enabled for cross-origin requests

---

## Project Structure

```
DOTNETPokemonAPI/
├── Database/
│   └── PokemonDb.cs              # DbContext configuration
├── DTO/
│   ├── BoxPokemonDTO.cs          # PC Box response model
│   ├── PokemonFromBoxToMoveDTO.cs # Move request model
│   ├── TrainerAllPokemonDTO.cs   # Complete trainer Pokemon collection
│   ├── TrainerWithPokemonDTO.cs  # Trainer with party Pokemon
│   └── TransferedPokemonDTO.cs   # Transfer response model
├── Exceptions/
│   └── NotFoundException.cs      # Custom not found exception
├── Models/
│   ├── BoxPC.cs                  # PC Box storage entity
│   ├── MoveTypeEnum.cs           # Move type enumeration
│   ├── Pokemon.cs                # Pokemon entity
│   └── Trainer.cs                # Trainer entity
├── Usecases/
│   ├── BoxDbUsecases.cs          # PC Box operations
│   └── PokemonDbUsecases.cs      # Trainer & Pokemon operations
└── Program.cs                    # Application entry point
```

---

## API Endpoints

### Pokémon

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/pokemons` | Get all Pokemon registered |

### Trainers

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/trainers` | Get all trainers with their party Pokemon |
| `GET` | `/trainer/{id}` | Get a specific trainer by ID |
| `GET` | `/trainer/{id}/box` | Get a trainer's PC Box Pokemon |
| `GET` | `/trainer/{id}/all` | Get all Pokemon (party + box) for a trainer |

### PC Box

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/box/{boxId}` | Get a specific PC Box by ID |
| `PUT` | `/box/{boxId}/move` | Move Pokemon between party and box |

---

## Data Models

### Pokemon
| Property | Type | Description |
|----------|------|-------------|
| `Id` | `int` | Unique identifier |
| `Name` | `string` | Pokemon name |
| `Description` | `string` | Pokemon description |

### Trainer
| Property | Type | Description |
|----------|------|-------------|
| `Id` | `int` | Unique identifier |
| `Name` | `string` | Trainer name |
| `PokemonIds` | `List<int>` | Party Pokemon IDs |
| `BoxPcId` | `int?` | Associated PC Box ID |

### BoxPC
| Property | Type | Description |
|----------|------|-------------|
| `Id` | `int` | Unique identifier |
| `PokemonIds` | `List<int>` | Stored Pokemon IDs |
| `BoxTrainer` | `Trainer` | Box owner reference |

### MoveTypeEnum
| Value | Description |
|-------|-------------|
| `BoxToParty` | Move Pokemon from PC Box to party |
| `PartyToBox` | Move Pokemon from party to PC Box |

---

## Request/Response Models

### PokemonFromBoxToMoveDTO (Request)
| Property | Type | Description |
|----------|------|-------------|
| `PokemonToMoveIds` | `List<int>` | IDs of Pokemon to move |
| `TrainerId` | `int` | Trainer performing the move |
| `Type` | `MoveTypeEnum` | Direction of the move |

### TransferedPokemonDTO (Response)
| Property | Type | Description |
|----------|------|-------------|
| `TrainerId` | `int` | Trainer who performed the move |
| `BoxId` | `int` | PC Box involved in the transfer |
| `TransferedToBox` | `List<Pokemon>` | Pokemon moved to box |
| `TransferedToParty` | `List<Pokemon>` | Pokemon moved to party |

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)

### Configuration

1. Create an `appsettings.json` file in the project root:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=pokemon_db;Username=your_user;Password=your_password"
  }
}
```

### Running the Application

```bash
# Restore dependencies
dotnet restore

# Run the application
dotnet run
```

---

## Response Examples

### GET /trainers
```json
[
  {
    "id": 1,
    "boxId": 1,
    "pokemons": [
      { "id": 25, "name": "Pikachu", "description": "Electric Mouse Pokemon" }
    ]
  }
]
```

### GET /trainer/{id}/all
```json
{
  "id": 1,
  "name": "Ash",
  "allPokemon": [
    { "id": 25, "name": "Pikachu", "description": "Electric Mouse Pokemon" },
    { "id": 6, "name": "Charizard", "description": "Flame Pokemon" }
  ]
}
```

### GET /box/{boxId}
```json
{
  "id": 1,
  "boxTrainerId": 1,
  "boxTrainerName": "Ash",
  "pokemons": [
    { "id": 143, "name": "Snorlax", "description": "Sleeping Pokemon" }
  ]
}
```

### PUT /box/{boxId}/move

**Request:**
```json
{
  "pokemonToMoveIds": [143],
  "trainerId": 1,
  "type": "BoxToParty"
}
```

**Response:**
```json
{
  "trainerId": 1,
  "boxId": 1,
  "transferedToBox": [],
  "transferedToParty": [
    { "id": 143, "name": "Snorlax", "description": "Sleeping Pokemon" }
  ]
}
```

---

## Business Rules

- **Party Limit**: Trainers can have a maximum of 6 Pokemon in their party
- **Minimum Party Size**: At least 1 Pokemon must remain in the party at all times
- **Box Ownership**: Pokemon can only be moved between a trainer's own party and box

---

## Architecture

This API follows a clean architecture pattern:

- **Models** - Entity definitions mapped to database tables
- **DTOs** - Data Transfer Objects for API requests and responses
- **Usecases** - Business logic and database operations
- **Exceptions** - Custom exception types for error handling
- **Database** - Entity Framework Core DbContext configuration

---

## License

MIT
