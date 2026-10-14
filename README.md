# Konekto - University Communication and Alumni Tracking Platform (Backend API)

Welcome to the backend service of **Konekto**, an enterprise university communication and alumni tracking platform designed to bridge the gap between universities, graduates, and current students.

---

## 📊 Data Design Documentation (DDD)

A detailed technical breakdown of the database schema, entity relationships, normalization, indexing, and container persistence strategies is documented in:

👉 **[Detailed Data Design Document (ddd.md)](./ddd.md)**

* **Entity Relationship Diagram (ERD):** Full visual Mermaid diagram modeling `Users`, `AlumniProfiles`, `Departments`, `Companies`, `EmploymentHistories`, and `JobPostings`.
* **Data Dictionary:** Complete column definitions, data types, constraints, and business rules.
* **Storage & Persistence:** In-memory transition to relational databases and Docker Volume durability strategies (surviving "The Forgetting Test").

---

## 🏛️ Architecture and Design Decisions

During the system analysis and design phases, the following key decisions were made for Konekto:

1. **Clean Architecture:** Based on the "Separation of Concerns" principle, the project is built on a sustainable and flexible layered structure where dependencies flow from the outside in.
2. **Flexible Data Integration (SIS Readiness):** For future official integration with the university's Student Information System (SIS), data access is abstracted via Interfaces; a mock/in-memory data infrastructure is established for early prototype validation.
3. **Database Normalization:** Relational integrity is ensured using Entity Framework Core (Code-First); a robust schema is designed covering user roles, department details, companies, and employment histories.
4. **Cross-Origin Resource Sharing (CORS):** Fully enabled to allow seamless communication with client frontend applications (e.g., React/Vue/Dashboard on port 3000 or 5100).

---

## 🛠️ Tech Stack

* **Backend Framework:** C# (.NET 9.0) & ASP.NET Core Web API (Minimal APIs & Controller ready)
* **Database & ORM:** MS SQL Server / PostgreSQL & Entity Framework Core (Code-First)
* **DevOps & Containers:** Docker, Docker Compose (Multi-stage build)
* **Security & Authorization:** JWT (JSON Web Token) role-based access control (Admin, Alumni, Student, Academician)
* **API Documentation:** Swagger / OpenAPI (`/swagger` and `/api/swagger`)
* **Version Control:** Git & GitHub

---

## 📂 Project Directory Structure

```text
alumni-main/
│
├── src/
│   ├── Konekto.Domain/         # Database models (Entities) and Core Interfaces
│   ├── Konekto.DataAccess/     # EF Core configurations, DbContext, and Migrations
│   ├── Konekto.Business/       # Business rules, services, and DTOs
│   └── Konekto.API/            # API Endpoints, Middlewares, and Static Dashboard UI
│
├── ddd.md                      # Comprehensive Data Design Document (DDD)
├── Dockerfile                  # Multi-stage production container build
├── docker-compose.yml          # Container orchestration and port mappings
├── Konekto.sln                 # Visual Studio solution file
└── README.md                   # Main project documentation
```

---

## ⚙️ Installation and Setup Guide

### Option A: Running with Docker (Recommended)

1. Ensure **Docker Desktop** is running.
2. Build and launch the containerized application:
   ```bash
   docker compose up -d --build
   ```
3. Access the services:
   * **Web Dashboard:** [http://localhost:5100/](http://localhost:5100/)
   * **Swagger UI:** [http://localhost:5100/swagger](http://localhost:5100/swagger)

4. To stop the containers:
   ```bash
   docker compose down
   ```

### Option B: Running Locally with .NET SDK

1. Ensure **.NET 9.0 SDK** is installed.
2. Restore packages and run the API:
   ```bash
   dotnet run --project src/Konekto.API
   ```
3. Open your browser at `http://localhost:5100`.

---

## 🔌 Core API Endpoints

| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `GET` | `/api/health` | Healthcheck endpoint returning system JSON status. |
| `GET` | `/api/swagger` | Redirects to Swagger documentation. |
| `POST` | `/api/alumni` | Register a new alumni record (JSON body). |
| `GET` | `/api/alumni` | List all registered alumni records. |
| `GET` | `/api/alumni/{id}` | Retrieve details for a specific alumni by ID. |
| `PUT` | `/api/alumni/{id}` | Update existing alumni details. |
| `DELETE` | `/api/alumni/{id}` | Remove an alumni record. |
| `POST` | `/api/users` | Create user profile record. |
| `GET` | `/api/users` | List all users. |
| `GET` | `/api/users/{id}` | Retrieve user profile by ID. |
