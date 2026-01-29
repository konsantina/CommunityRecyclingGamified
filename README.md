# Community Recycling Gamified – Backend (ASP.NET Core Web API)

This folder contains the **backend** for the Community Recycling Gamified platform. It exposes a REST API secured with JWT and supports role-based authorization (User/Admin).

## Demo Credentials

Admin accounts:
- Email: test@gmail.com  
  Password: 123
- Email: takis@gmail.com  
  Password: 123

User account:
- Email: user@gmail.com  
  Password: 123

## Main Features (API)

- Authentication & Authorization (JWT, roles)
- Rewards & Redemptions (including admin approval flow)
- Points & Points Ledger (audit of point changes)
- Badges (user progress/unlocked badges)

## Key Endpoints (high level)

User:
- GET /api/points/me
- GET /api/points/ledger/me
- GET /api/badges/me
- GET /api/rewards
- POST /api/redemptions
- GET /api/redemptions/me

Admin:
- POST /api/rewards
- PUT /api/rewards/{id}
- DELETE /api/rewards/{id}
- GET /api/redemptions/pending
- POST /api/redemptions/{id}/approve
- POST /api/redemptions/{id}/reject

## How to Run (Local)

1) Open a terminal in the backend directory and run:

dotnet restore  
dotnet run  

2) The API usually runs on:

https://localhost:5001

## Notes

- If the frontend runs on a different origin (e.g. http://localhost:4200), ensure CORS is enabled for that origin.
- If you use database migrations/seed data, run them according to the project’s setup (e.g. EF Core migrations).
