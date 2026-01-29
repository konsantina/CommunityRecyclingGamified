# Community Recycling Gamified – Backend (ASP.NET Core Web API)

This folder contains the **backend** for the Community Recycling Gamified platform. It exposes a REST API secured with JWT and supports role-based authorization.

---

## Roles (Important)

This project uses numeric roles in the database:

- **Role = 0** → **Admin**
- **Role = 2** → **User**

You must have at least **one Admin** and **one User** in the database to fully test the application.

---

## Prerequisites

- .NET SDK installed (recommended: latest LTS)
- SQL Server installed locally (or SQL Server Express / LocalDB)
- (Optional) SQL Server Management Studio (SSMS) to view/edit the database

---

## How to Run (Local) + Create the Database Locally

Because the database is not included, you must create it locally using EF Core migrations.

### 1) Configure the Connection String

Open `appsettings.json` (or `appsettings.Development.json`) and set the SQL Server connection string.

Example (Windows LocalDB):
Server=(localdb)\\MSSQLLocalDB;Database=CommunityRecyclingGamifiedDb;Trusted_Connection=True;TrustServerCertificate=True;

Example (SQL Server Express):
Server=localhost\\SQLEXPRESS;Database=CommunityRecyclingGamifiedDb;Trusted_Connection=True;TrustServerCertificate=True;

### 2) Apply EF Core Migrations (Create Schema)

From the backend project directory (the folder that contains the `.csproj`), run:

dotnet tool restore  
dotnet restore  
dotnet ef database update  

This will:
- create the database locally (if it does not exist)
- apply all migrations
- create tables/relationships

If `dotnet ef` is not available, install the EF tool:

dotnet tool install --global dotnet-ef

Then run:

dotnet ef database update

### 3) Run the API

dotnet run  

The API usually runs on:

https://localhost:5001

---

## Database Setup: Create Required Users Manually

After the database is created, you must insert **two users** manually:

1) **One Admin user** with **Role = 0**  
2) **One normal User** with **Role = 2**

Use SSMS to insert these records into the appropriate users table (e.g. `Users`, `UserProfiles`, or the table used by your auth model in this project).

### Minimum Required Fields

Create the two users with the minimum fields required by your schema (commonly):
- Email / Username
- PasswordHash (depending on how auth is implemented)
- Role (0 for Admin, 2 for User)
- Any required profile fields (if enforced by NOT NULL constraints)

Important: If your auth uses hashed passwords, you must create users using the same hashing method as the backend expects. If your project provides a registration endpoint, you can create the accounts by registering normally and then updating the Role value in the database:
- Register a normal account → set Role to 2
- Register another account → set Role to 0

This is the recommended approach when password hashing is enabled.

---

## Notes / Troubleshooting

- If login fails but the API runs correctly, confirm you created the two required users and roles in the DB.
- If Angular calls fail with CORS errors, allow `http://localhost:4200` in the backend CORS policy.
- If `dotnet ef database update` fails, confirm:
  - you are running commands in the correct folder (where the Web API `.csproj` is)
  - connection string is correct
  - SQL Server / LocalDB is running

