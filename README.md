# Konekto - University Communication and Alumni Tracking Platform

Welcome to **Konekto**, an enterprise university communication and alumni tracking platform designed to bridge the gap between universities, graduates, and current students.

---

## 🚀 4. Hafta Proje Özellikleri ve Mimarisi

4. hafta isterleri doğrultusunda sistem modern **Katmanlı Mimari (Clean/Layered Architecture)** prensiplerine tam uyumlu hale getirilmiştir:

1. **Katmanlı Mimari (Layered Architecture):**
   - **`Konekto.Domain`**: Temel varlıklar (`User`, `Alumni`, `Announcement`, `JobPosting`, `Department`, `Company`, `Application`) ve soyut repository arayüzleri (`IUserRepository`, `IAlumniRepository`, `IAnnouncementRepository`).
   - **`Konekto.DataAccess`**: Entity Framework Core DbContext (`KonektoDbContext`), Code-First Fluent API konfigürasyonları, Migration dosyaları ve somut Repository uygulamaları.
   - **`Konekto.Business`**: Model Factory'leri (`UserFactory`, `AlumniFactory`, `AnnouncementFactory`, `JobPostingFactory`) ve veritabanı tohumlama mekanizması (`DatabaseSeeder`).
   - **`Konekto.API`**: Hem **Web Controllers (ASP.NET Core MVC)** hem de **RESTful API Controllers** uç noktaları ve Razor Görünümleri.

2. **Web Controllers (MVC) & Razor Görünümleri (Views):**
   - `HomeController`: Ana sayfa, özet duyuru akışı ve sistem istatistikleri.
   - `AnnouncementsController` (Web): Duyuruların filtrelenmesi, detay sayfası ve yeni duyuru yayınlama formu (`Index`, `Details`, `Create`, `Delete`).
   - `AlumniController` (Web): Mezun rehberi ve arama filtreleme.
   - `Views/Shared/_Layout.cshtml`: Modern, duyarlı (responsive) Bootstrap 5 ve FontAwesome arayüzü.

3. **API Controllers (REST API):**
   - `/api/announcements`: Duyuru CRUD işlemleri (`GET`, `POST`, `PUT`, `DELETE`).
   - `/api/users`: Kullanıcı CRUD işlemleri (`GET`, `POST`, `PUT`, `DELETE`).
   - `/api/alumni`: Mezun profili CRUD işlemleri (`GET`, `POST`, `PUT`, `DELETE`).
   - `/api/job-postings`: İş ilanı ve alt başvuru (nested applications) API işlemleri.

---

## 📊 Data Design Documentation (DDD)

Veritabanı şeması, ER diyagramları ve normalizasyon kuralları detaylı olarak belgelenmiştir:
👉 **[Detailed Data Design Document (ddd.md)](./ddd.md)**

---

## 🛠️ Teknoloji Yığını

* **Backend & Web:** C# (.NET 9.0) & ASP.NET Core (MVC + Web API)
* **Veritabanı & ORM:** MS SQL Server / PostgreSQL & Entity Framework Core 9.0 (Code-First)
* **Arayüz (Frontend):** ASP.NET Core Razor Views, Bootstrap 5.3, Font Awesome 6
* **DevOps & Konteyner:** Docker & Docker Compose
* **API Dokümantasyonu:** Swagger / OpenAPI (`/swagger`)

---

## 📂 Dizin Yapısı

```text
alumni-main/
│
├── src/
│   ├── Konekto.Domain/         # Varlıklar (Entities: User, Announcement, Alumni vb.) & Arayüzler
│   ├── Konekto.DataAccess/     # EF Core DbContext, Repository sınıfları, Migrations
│   ├── Konekto.Business/       # Model Factories ve Database Seeder
│   └── Konekto.API/
│       ├── Controllers/        # Web MVC Controllers (Home, Announcements, Alumni)
│       │   └── Api/            # API Controllers (Announcements, Users, Alumni, JobPostings)
│       ├── Views/              # Razor Görünümleri (Home, Announcements, Alumni, Shared)
│       ├── wwwroot/            # Statik dosyalar
│       └── Program.cs          # Dependency Injection & Routing Yapılandırması
│
├── ddd.md                      # Kapsamlı Veri Tasarım Dokümanı (DDD)
├── Dockerfile                  # Multi-stage production container build
├── docker-compose.yml          # Konteyner orkestrasyonu
├── Konekto.sln                 # Çözüm dosyası
└── README.md                   # Proje ana dokümanı
```

---

## ⚙️ Kurulum ve Çalıştırma

### .NET SDK ile Yerel Olarak Çalıştırma:

```bash
dotnet restore
dotnet run --project src/Konekto.API
```

Tarayıcınızdan erişin:
* **Web Arayüzü (MVC Views):** [http://localhost:5100/](http://localhost:5100/)
* **Duyurular Sayfası:** [http://localhost:5100/Announcements](http://localhost:5100/Announcements)
* **Mezun Rehberi:** [http://localhost:5100/Alumni](http://localhost:5100/Alumni)
* **Swagger API Dokümantasyonu:** [http://localhost:5100/swagger](http://localhost:5100/swagger)
