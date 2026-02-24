# Support Ticket Management API

Support Ticket Management — Student Project Assignment

Overview: Build a backend for a company helpdesk system where employees raise tickets, support staff handle them, and managers track everything.

## Tools and Packages Used
To build this project, I used the following libraries and tools:
* ASP.NET Core 8.0
* Entity Framework Core
* JWT Bearer
* Swagger (Swashbuckle)
* SQL Server
* Tools and Design

## Tasks and Features Implemented
Here is what the system can do:

### 1. User Security and Roles
* Secure Login: Users can log in with their email and password to receive a JWT token.
* Password Hashing: I used BCrypt so passwords are never stored in plain text.
* Roles (RBAC): There are three roles: MANAGER, SUPPORT, and USER. Each has different permissions.
* Auto-Seeding: On first run, the system automatically creates the roles and a default manager user (m2@gmail.com).

### 2. User Management
* Only managers can create new users (Support or standard Users).
* Managers can list all registered people in the system.

### 3. Ticket Management
* Creating Tickets: Users and Managers can start new support tickets.
* Assigning Tickets: Managers or Support staff can assign a ticket to a specific person.
* Status Lifecycle: Tickets follow a strict flow: `OPEN` -> `IN_PROGRESS` -> `RESOLVED` -> `CLOSED`.
* Tracking: Every time a status changes, it gets logged automatically so you can see the history.

### 4. Communication (Comments)
* Every ticket has its own comment section.
* Users can explain their issues, and support staff can provide updates.
* The API returns comments nested inside the ticket info for easy reading.

### 5. API Design
* I tailored the API to return data in a specific "JSON Document" style.
* All IDs are returned as `_id` and all field names use `camelCase` to match common frontend standards.

---

## How to Start
1. Update your connection string in `appsettings.json`.
2. Run Project http/https
3. Open `http://localhost:5002/swagger` to see the endpoints.
4. Login with **m2@gmail.com / Meet@123** as your first step is this manager.
5. After generated token apply token and complete login and check all role control.
