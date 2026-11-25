# Konekto - University Communication and Alumni Tracking Platform

Welcome to **Konekto**, an enterprise university communication and alumni tracking platform designed to bridge the gap between universities, graduates, and current students.

> 📚 **Temel Proje Dokümantasyonu & Hızlı Erişim Bağlantıları:**  
> • 📊 **[Veri Tasarım Dokümanı (ddd.md)](./ddd.md)** — *Veritabanı şeması, ER diyagramları ve normalizasyon kuralları*  
> • 🎨 **[Marka ve Görsel Kimlik Kılavuzu (brand.md)](./brand.md)** — *Marka hikayesi, renk paleti, tipografi ve logo standartları*  
> • 📐 **[Tasarım Sistemi Spesifikasyonu (DESIGN.md)](./DESIGN.md)** — *VoltAgent & Refero formatında AI ve geliştirici tasarım sistemi*  
> • 🔌 **[Swagger API Arayüzü](http://localhost:5100/swagger)** — *REST API interaktif test dokümantasyonu*

---

## 📑 İçindekiler
* [🔐 Kimlik Doğrulama & Yetkilendirme (Authentication & Authorization)](#-kimlik-doğrulama--yetkilendirme-authentication--authorization)
* [🚀 Proje Özellikleri ve Mimarisi](#-4-hafta-proje-özellikleri-ve-mimarisi)
* [📊 Data Design Documentation (ddd.md)](#-data-design-documentation-ddd)
* [🎨 Marka ve Görsel Kimlik Dokümantasyonu (brand.md)](#-marka-ve-görsel-kimlik-dokümantasyonu-brand-guidelines)
* [📐 Tasarım Sistemi Spesifikasyonu (DESIGN.md)](#-tasarım-sistemi-spesifikasyonu-designmd)
* [🛠️ Teknoloji Yığını](#️-teknoloji-yığını)
* [📂 Dizin Yapısı](#-dizin-yapısı)
* [⚙️ Kurulum ve Çalıştırma](#️-kurulum-ve-çalıştırma)

---

## 🔐 Kimlik Doğrulama & Yetkilendirme (Authentication & Authorization)

Ders tahtasında belirtilen mimari doğrultusunda **Konekto**, hem MVC Razor görünümleri için **Cookie Authentication** hem de RESTful API istemcileri için **JWT Bearer Token** tabanlı çift katmanlı bir kimlik doğrulama altyapısına sahiptir.

### 1. İstemci-Sunucu (Client-Server) Token Akışı (Tahta Mimarisi)
```text
  [ Client (Tarayıcı / API İstemcisi) ]
             |
             |  1. POST /api/auth/login (Username + Password)
             v
  [ Server (Konekto Auth / API) ]
             |
             |  2. Şifre Doğrulama (PBKDF2) -> 200 OK + JWT Token / Cookie
             v
  [ Client ]
             |
             |  3. GET /api/... veya GET /admin (Header: Bearer Token / Cookie)
             v
  [ AuthMiddleware ] -> Token / Rol Doğrulama ('Admin' kontrolü)
             |
             |  4. 200 OK (Yetki Var) / 401 Unauthorized / 403 Forbidden
             v
  [ Client ]
```

### 2. Hazır Test Kullanıcı Hesapları (Seeded Test Accounts)
Sistemi test etmek için veritabanına otomatik olarak aşağıdaki hesaplar tohumlanmaktadır:

| Rol | E-Posta | Şifre | Yetkiler & Sayfalar |
| :--- | :--- | :--- | :--- |
| **Admin (Yönetici)** | `admin@konekto.edu.tr` | `Admin123!` | `/Admin` Yönetici Paneli, Rol Atama, Kullanıcı Dondurma/Silme, Tüm İçerik Moderasyonu |
| **Alumni (Mezun)** | `alumni@konekto.edu.tr` | `Alumni123!` | `/Account/Profile`, Duyuru Yayınlama (`/Announcements/Create`), Profil Düzenleme |
| **Student (Öğrenci)** | `student@konekto.edu.tr` | `Student123!` | `/Account/Profile`, Mezun Rehberi ve İlan İnceleme |

> 💡 *Not: Giriş sayfasında (`/Account/Login`) yer alan **"Hızlı Test Girişleri"** butonlarına tıklayarak bu hesap bilgilerini tek tıkla forma doldurabilirsiniz.*

### 3. Kullanıcı Kayıt & Giriş / Çıkış (Register, Login & Logout)
- **Kayıt Sayfası (`/Account/Register`):** Ad, Soyad, E-Posta, Şifre, Rol (`Alumni` veya `Student`), Bölüm seçimi, Mezuniyet Yılı ve Şirket/Unvan alanlarını içerir. Şifreler **PBKDF2-HMAC-SHA256** ile tuzlanarak (salt) güvenle saklanır.
- **Giriş Sayfası (`/Account/Login`):** Kullanıcıyı doğrular, Cookie oluşturur ve API için `konekto_token` üretir. Admin rolü doğrudan `/Admin` paneline yönlendirilir.
- **Çıkış (`/Account/Logout`):** Cookie ve token'ları temizleyerek güvenli oturum kapatma sağlar.

### 4. Özel Auth Middleware (`Konekto.API.Middlewares.AuthMiddleware`)
- Gelen her istekte `Authorization: Bearer <token>` başlığını, `X-Auth-Token` başlığını veya `konekto_token` çerezini denetler.
- `/admin` ve `/api/admin` rotalarını korur:
  - Giriş yapmamış kullanıcılara API için `401 Unauthorized` JSON, tarayıcı için `/Account/Login` yönlendirmesi yapar.
  - Rolü `Admin` olmayan kullanıcılara API için `403 Forbidden` JSON, tarayıcı için `/Account/AccessDenied` yönlendirmesi yapar.

### 5. Yönetici Paneli (`/Admin`) & Rol Atama (Assign Admin Role)
- Yalnızca `Admin` rolündeki kullanıcılar erişebilir.
- **Rol Atama (Assign Role):** Herhangi bir kullanıcının rolü tek tıkla `Admin`, `Alumni` veya `Student` olarak değiştirilebilir.
- **Hesap Durumu:** Kullanıcı hesapları anında dondurulabilir veya aktifleştirilebilir.
- **Veri Moderasyonu:** Tüm duyurular ve iş ilanları incelenip silinebilir.

### 6. Kullanıcı Rolüne Göre Dinamik Navigasyon (`_Layout.cshtml`)
- **Giriş Yapılmamışsa:** Üst menüde *"Giriş Yap"* ve *"Kayıt Ol"* butonları görüntülenir.
- **Giriş Yapılmışsa:** Kullanıcı adı ve rol rozeti (örn: `[Admin]`, `[Alumni]`, `[Student]`) ile modern açılır menü gösterilir.
- **Admin Rolü Varsa:** Üst menüde kırmızı kalkan ikonuyla vurgulanan **"Yönetici Paneli"** bağlantısı belirir.
- **Kullanıcıya Özel Profil:** `/Account/Profile` sayfasında kullanıcı kendi rol bilgilerini, şirket, unvan, LinkedIn ve biyografisini düzenleyebilir.

### 7. REST API Kimlik Doğrulama Uç Noktaları
- `POST /api/auth/login`: `{ email, password }` alır; `{ token, tokenType: "Bearer", user }` döner.
- `POST /api/auth/register`: Yeni kullanıcı kaydeder ve token döner.
- `GET /api/auth/me`: `Authorization: Bearer <token>` başlığıyla çağrıldığında doğrulanmış kullanıcı bilgilerini döner.

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
   - `/api/test-500`: 5xx Guardian test uç noktası (`GET`).

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

## 🎨 Marka ve Görsel Kimlik Dokümantasyonu (Brand Guidelines)

Konekto platformunun marka felsefesi, görsel kimliği, renk sistemi, tipografi hiyerarşisi ve UI bileşen standartları detaylı olarak belgelenmiştir:
👉 **[Brand Guidelines (brand.md)](./brand.md)**

---

## 📐 Tasarım Sistemi Spesifikasyonu (DESIGN.md)

Refero ([styles.refero.design](https://styles.refero.design)) ve [VoltAgent/awesome-design-md](https://github.com/VoltAgent/awesome-design-md) standartlarına uygun olarak hazırlanan, geliştiriciler ve AI kodlama ajanları için görsel referans kaynağı:
👉 **[Design System Specification (DESIGN.md)](./DESIGN.md)**

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
├── brand.md                    # Marka kimliği ve tasarım kılavuzu (Brand Guidelines)
├── DESIGN.md                   # Tasarım sistemi ve token spesifikasyonu (VoltAgent / Refero formatı)
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
