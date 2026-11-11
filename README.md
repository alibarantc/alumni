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

3. **API Controllers & Minimal API Uç Noktaları (REST API):**
   - `/api/announcements`: Duyuru CRUD işlemleri (`GET`, `POST`, `PUT`, `DELETE`).
   - `/api/users`: Kullanıcı CRUD işlemleri (`GET`, `POST`, `PUT`, `DELETE`).
   - `/api/users/{id}/job-postings`: Kullanıcıya ait iş ilanları iç içe rotası (6. Hafta Nested Route - `GET`, `POST`).
   - `/api/alumni`: Mezun profili CRUD işlemleri (`GET`, `POST`, `PUT`, `DELETE`).
   - `/api/job-postings`: İş ilanı ve alt başvuru (`/applications`) API işlemleri.
   - `/api/test-500`: Yalnızca **Development** ortamında erişilebilen 5xx Guardian test uç noktası (`GET`).

4. **Middleware Mimarisi, Güvenlik Kalkanları (Guardians) ve Loglama:**
   - **Terminal Logger Middleware (`RequestLoggingMiddleware`):** Gelen her isteğin süresini (`Stopwatch`) ölçer ve terminale formatlı log basar:
     `time method route restype duration` (Örn: `10:20:30 post /law 201 18ms`).
   - **404 Guardian Middleware (`NotFoundGuardianMiddleware`):**
     - API ve JSON istemcileri için hatanın nedenini (`reason`) içeren yapılandırılmış 404 JSON nesnesi döner.
     - Web tarayıcı isteklerinde kullanıcıyı otomatik olarak jenerik 404 sayfasına (`/not-found`) yönlendirir.
     - Tüm 404 olaylarını `logs/error.logs` dosyasına yazar.
   - **5xx Guardian Middleware (`ServerErrorGuardianMiddleware`):** Uygulama genelinde yakalanmamış sunucu istisnalarını (`unhandled exceptions`) yakalar, çöküşü önler, stack trace'i loglar ve standart 500 JSON döner.
   - **Dosya Tabanlı Hata Loglama (`ErrorFileLogger`):** `logs/error.logs` dosyasına thread-safe biçimde tüm 404 ve 5xx hata kayıtlarını işler.
   - **Jenerik 404 Görünümü (`Views/Home/NotFound.cshtml`):** Modern Bootstrap 5 kart tasarımıyla kullanıcı dostu hata ve yönlendirme arayüzü sunar.

5. **İş Kuralları ve Doğrulamalar (JobPosting Validation Rules):**
   - **`Title`:** Zorunlu alan, en fazla 120 karakter (`max 120 chars`).
   - **`CompanyId`:** Zorunlu alan; geçerli bir şirkete ait olmalıdır.
   - **`Location (City)` & `Description`:** İsteğe bağlı alanlar (`optional`).

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
├── logs/                       # Hata denetim logları
│   └── error.logs              # 404 ve 5xx hata detayları
│
├── src/
│   ├── Konekto.Domain/         # Varlıklar (Entities: User, Announcement, Alumni vb.) & Arayüzler
│   ├── Konekto.DataAccess/     # EF Core DbContext, Repository sınıfları, Migrations
│   ├── Konekto.Business/       # Model Factories ve Database Seeder
│   └── Konekto.API/
│       ├── Controllers/        # Web MVC Controllers (Home, Announcements, Alumni)
│       │   └── Api/            # API Controllers (Announcements, Users, Alumni, JobPostings)
│       ├── Middlewares/        # RequestLogging, NotFoundGuardian, ServerErrorGuardian, ErrorFileLogger
│       ├── Views/              # Razor Görünümleri (Home, Announcements, Alumni, Shared, NotFound)
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
* **Jenerik 404 Sayfası:** [http://localhost:5100/not-found](http://localhost:5100/not-found)
* **Swagger API Dokümantasyonu:** [http://localhost:5100/swagger](http://localhost:5100/swagger)

---

## ✅ 6. Hafta: API Doğrulamaları ve Testler

- **Çakışmayan rotalar:** `/api/users`, `/api/alumni`, `/api/announcements` ve `/api/job-postings` CRUD rotaları tek bir MVC API Controller üzerinden sunulur. Minimal API tarafında yalnızca MVC Controller'da olmayan application detay rotaları ve yardımcı endpoint'ler kalır.
- **JobPosting POST / PUT kuralları:** Boş başlık reddedilir, başlık en fazla **120 karakter** olmalıdır, `CompanyId` zorunludur ve **Companies** tablosunda bulunmalıdır. `Description` ve `Location` gönderilmeyebilir.
- **Nested JobPosting POST:** `POST /api/users/{id}/job-postings` aynı doğrulamaları kullanır.
- **5xx Guardian:** Production yanıtlarında istisna mesajı istemciye verilmez; ayrıntılar sunucu loglarına yazılır. Bilinçli hata üreten `/api/test-500` sadece Development'ta açıktır.
- **Hata dosyası:** `logs/error.logs`, uygulamanın **çalışma dizinine** göre hata sırasında oluşturulur (Git'e gönderilmez). Dosyaya yazılamazsa stderr'e bildirilir.

### Veritabanı geçişi

`JobPosting.CompanyId` veritabanında artık `NOT NULL`, `Description` ise isteğe bağlıdır. Yeni migration `20261112010000_EnforceJobPostingRules` olarak eklenmiştir. Mevcut ilanlarda `CompanyId IS NULL` veya `LEN(Title) > 120` varsa migration **bilinçli olarak durur**; önce bu eski kayıtları gerçek bir şirkete bağlayın / başlıkları düzeltin, veri otomatik değiştirilmez.

```bash
dotnet restore Konekto.sln
dotnet ef database update --project src/Konekto.DataAccess --startup-project src/Konekto.API
dotnet run --project src/Konekto.API
```

### Postman doğrulaması

`tests/postman/Konekto.Week6.postman_collection.json` koleksiyonunu Postman'e aktarın. `baseUrl` (varsayılan `http://localhost:5100`), `validCompanyId` ve `userId` değişkenlerini çalışan veritabanındaki geçerli kayıtlarla eşleştirin. Koleksiyonu **sırasıyla** çalıştırın: 400 doğrulama yanıtları, 404 neden mesajı, geçerli POST/PUT, kullanıcı alt rotası ve 500 yanıtı denetlenir. 500 testi için `ASPNETCORE_ENVIRONMENT=Development` kullanılmalıdır. Koleksiyonun eklenmesi, testlerin çalıştırıldığı anlamına gelmez.

GitHub Actions `.github/workflows/dotnet-build.yml` Pull Request'lerde .NET 9 restore/build denetimi yapar; Postman entegrasyon testleri için çalışan SQL Server hâlâ gereklidir.
