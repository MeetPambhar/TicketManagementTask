# Support Ticket Management API

A RESTful API for a company helpdesk system with Role-Based Access Control (RBAC).

## Features
- JWT Authentication & Bcrypt Password Hashing.
- RBAC (MANAGER, SUPPORT, USER).
- Ticket Lifecycle: OPEN → IN_PROGRESS → RESOLVED → CLOSED.
- Ticket status logging.
- Ticket comments with ownership/assignment checks.
- Swagger UI for API exploration.

## Tech Stack
- ASP.NET Core 8.0
- Entity Framework Core
- SQL Server (LocalDB)
- JWT (JSON Web Tokens)
- BCrypt.Net

## Setup Instructions
1. **Clone the repository.**
2. **Update Connection String**: Check `appsettings.json` and update the `DefaultConnection` if necessary.
3. **Run Migrations (if applicable)** or ensure DB is created.
4. **Run the application**: `dotnet run` or via Visual Studio.
5. **Swagger UI**: Accessible at `http://localhost:3000/swagger` or `/docs`.

## Initial Setup
- Create a MANAGER user directly in the database to start creating other users.
- Role IDs:
  - 1: MANAGER
  - 2: SUPPORT
  - 3: USER

## API Endpoints Matrix

### Auth
- `POST /api/Auth/login`: Public login.

### Users (MANAGER Only)
- `POST /api/Users`: Create SUPPORT or USER accounts.
- `GET /api/Users`: List all users.

### Tickets & Comments
- `POST /api/Tickets`: Create a new ticket.
- `GET /api/Tickets`: List tickets (filtered by role).
- `PATCH /api/Tickets/{id}/assign`: Assign ticket to staff (MANAGER/SUPPORT).
- `PATCH /api/Tickets/{id}/status`: Update ticket status (MANAGER/SUPPORT).
- `DELETE /api/Tickets/{id}`: Delete ticket (MANAGER).
- `POST /api/Tickets/{id}/comments`: Add a comment to a ticket.
- `GET /api/Tickets/{id}/comments`: View all comments for a ticket.
- `PATCH /api/comments/{commentId}`: Update a specific comment.
- `DELETE /api/comments/{commentId}`: Delete a specific comment.

## API Usage Guide

### 1. Authentication (Login)
**Endpoint**: `POST /api/Auth/login`  
**Payload**:
```json
{
  "email": "manager@example.com",
  "password": "password123"
}
```
**Response**:
```json
{
  "token": "eyJhbG...",
  "user": {
    "id": 1,
    "name": "Admin",
    "email": "manager@example.com",
    "role": "MANAGER"
  }
}
```

### 2. Using the Token
To access protected "all services" (Tickets, Users, etc.), add the token to your HTTP headers:
- **Key**: `Authorization`
- **Value**: `Bearer YOUR_TOKEN_HERE`

### 3. Example: Fetching Tickets
Once logged in, use your token to fetch tickets.

**Endpoint**: `GET /api/Tickets`  
**Header**: `Authorization: Bearer <your_token>`

---

### 4. Permissions Table
- **MANAGER**: Full access to all endpoints.
- **SUPPORT**: Access to assigned tickets and status updates.
- **USER**: Access to create and view own tickets and associated comments.
