
``alumni
# Konekto - University Communication and Alumni Tracking Platform (Backend API)

## 🏛️ Architecture and Design Decisions

During the system analysis and design phases, the following key decisions were made for Konekto:

1. **Clean Architecture:** Based on the "Separation of Concerns" principle, the project is built on a sustainable and flexible layered structure where dependencies flow from the outside in.
2. **Flexible Data Integration (SIS Readiness):** For future official integration with the university's Student Information System (SIS), data access is abstracted via Interfaces; a mock data infrastructure is set up to run until real data is connected.
3. **Database Normalization:** Relational integrity is ensured using Entity Framework Core (Code-First); a robust MSSQL schema is designed covering user roles, department details, companies, and employment histories.

---

## 🛠️ Tech Stack

The project relies on industry-standard enterprise backend technologies:

*   **Backend:** 
    *   C# (.NET 8.0) - High-performance and type-safe object-oriented core language
    *   ASP.NET Core Web API - RESTful service architecture
*   **Database & ORM:** 
    *   MS SQL Server - Relational database management system
    *   Entity Framework Core - Database querying and Code-First modeling
*   **Security & Authorization:** 
    *   JWT (JSON Web Token) - Secure and role-based access control (Admin, Alumni, Academician)
*   **DevOps & Tools:** 
    *   Swagger (OpenAPI) - API endpoint documentation and testing
    *   Git & GitHub - Version control and repository management

---

## 📂 Project Directory Structure

```text
Konekto-Backend/
│
├── src/
│   ├── Konekto.Domain/         # Database models (Entity) and Interfaces
│   ├── Konekto.DataAccess/     # EF Core configurations, DbContext, and Repository pattern
│   ├── Konekto.Business/       # Business rules, validations, and data services
│   └── Konekto.API/            # Controllers, JWT settings, and Swagger (Main Entry Point)
│
├── Konekto.sln                 # Visual Studio main solution file
└── README.md                   # Project documentation

```

---

## ⚙️ Installation and Setup Guide

To run this project locally, ensure that **.NET 8.0 SDK** and **SQL Server** are installed on your machine.

**1. Clone the Repository**

```bash
git clone [https://github.com/alibarantc/alumni.git](https://github.com/alibarantc/alumni.git)
cd alumni

```

**2. Create the Database**
Run the database migrations via the terminal or Visual Studio Package Manager Console to set up the tables:

```bash
dotnet ef database update --project src/Konekto.DataAccess --startup-project src/Konekto.API

```

**3. Run the Application**
Start the API by running the following command in the project root directory:

```bash
cd src/Konekto.API
dotnet run

```

**Accessing the Application**

* Backend API Documentation (Swagger): `https://localhost:<port>/swagger`

---

## 🔌 Core API Endpoints (Draft)

The main routes planned and developed on the backend side:

| Method | Endpoint | Description |
| --- | --- | --- |
| POST | `/api/auth/register` | Register a new alumni, student, or academician. |
| POST | `/api/auth/login` | User authentication and JWT token generation. |
| GET | `/api/alumni` | List all alumni (with department and industry filtering options). |
| GET | `/api/alumni/{id}` | Get detailed career profile of a specific alumni. |
| GET | `/api/statistics/employment` | Department-based employment rate report for institution management (SIS Data Readiness). |
| POST | `/api/jobs` | Post a new job, internship, or announcement opportunity. |

```

```
