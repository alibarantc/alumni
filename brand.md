# Konekto - Marka Kimliği ve Tasarım Kılavuzu (Brand Guidelines)

Bu doküman, **Konekto** (Üniversite İletişim ve Mezun Takip Platformu) projesinin marka kimliğini, görsel tasarım standartlarını, renk paletini, tipografisini ve tasarım felsefesini standartlaştırmak amacıyla hazırlanmıştır.

---

## 1. Marka Özü ve Hikayesi

### 1.1 İsim ve Anlam
* **Marka Adı:** **Konekto**
* **Köken:** Esperanto dilinde *"Bağlantı / İletişim"* anlamına gelen sözcükten türetilmiştir.
* **Ana Mesaj:** Üniversite mezunları, öğrenciler ve akademisyenler arasındaki zamansal ve mekansal engelleri kaldırarak güçlü, güvenilir ve sürdürülebilir bir ekosistem kurmak.
* **Slogan (Tagline):**
  * *Türkçe:* *"Geleceğinizi Şekillendirin, Mezun Ağınızla Güçlenin."*
  * *İngilizce:* *"Connecting Alumni, Empowering Futures."*

### 1.2 Misyon & Vizyon
* **Misyon:** Mezunlarımızın bilgi birikimini ve sektörel gücünü üniversitemizin genç yetenekleriyle buluşturmak, kariyer ve staj fırsatlarını demokratikleştirmek.
* **Vizyon:** Türkiye'nin ve bölgenin en yenilikçi, modern ve kurumsal mezun takip ekosistemi olmak.

### 1.3 Marka Kişiliği ve Tonu
Konekto'nun iletişim dili şu 4 ana değer etrafında şekillenir:
1. **Profesyonel & Kurumsal:** Üniversitenin saygınlığını ve akademik ciddiyetini taşır.
2. **Kucaklayıcı & Samimi:** Mezunlar için sıcak bir "yuvaya dönüş" hissi yaratır.
3. **Kariyer Odaklı & Dinamik:** İş dünyasının hızına ve modern SaaS trendlerine uyum sağlar.
4. **Şeffaf & Güvenilir:** Doğrulanmış profiller ve güvenli veri koruma prensiplerini esas alır.

---

## 2. Logo ve Amblem Standartları

### 2.1 Logo Anatomisi
* **Sembol:** Akademik mezuniyet kepi simgesi (`fa-solid fa-graduation-cap`). Eğitimi, başarıyı ve ortak geçmişi temsil eder.
* **Yazı Karakteri:** `Plus Jakarta Sans` - ExtraBold (800 ağırlık).
* **Bileşim:**
  ```text
  [ 🎓 Kep İkonu ] Konekto
  ```

### 2.2 Kullanım Kuralları (Do's & Don'ts)
* ✅ **Do:** Marka adı her zaman ilk harfi büyük, geri kalanı küçük yazılmalıdır: `Konekto`.
* ✅ **Do:** Açık renkli zeminlerde ana marka rengimiz `#4F46E5` (Indigo) tercih edilmelidir.
* ✅ **Do:** Koyu veya degrade zeminlerde beyaz logo ve sarı vurgu rozeti kullanılmalıdır.
* ❌ **Don't:** Marka adını hepsi küçük (`konekto`) veya hepsi büyük (`KONEKTO`) yazmaktan kaçınınız.
* ❌ **Don't:** Logoyu döndürmeyiniz, sündürmeyiniz veya orantısız ölçeklemeyiniz.

---

## 3. Renk Sistemi (Color Palette)

Arayüzümüzde kullanılan renkler, modern **Stripe/Tailwind SaaS** tasarım sisteminden ilham alınarak kontrast ve erişilebilirlik (WCAG AA) standartlarına uygun olarak seçilmiştir:

### 3.1 Temel Renkler (Primary Palette)
| Renk Rolü | Renk Adı | HEX Kodu | RGB | Açıklama |
| :--- | :--- | :--- | :--- | :--- |
| **Primary** | Konekto Indigo | `#4F46E5` | `rgb(79, 70, 229)` | Ana marka rengi, butonlar, aktif sekmeler |
| **Primary Hover** | Deep Indigo | `#4338CA` | `rgb(67, 56, 202)` | Buton hover ve etkileşim durumları |
| **Primary Light** | Soft Indigo Tint | `#EEF2FF` | `rgb(238, 242, 255)` | Rozet zeminleri, menü aktif arka planı |
| **Secondary Accent**| Vibrant Blue | `#3B82F6` | `rgb(59, 130, 246)` | Gradyan geçişleri, kariyer vurguları |

### 3.2 Zemin ve Tipografi Renkleri (Neutral Palette)
| Renk Rolü | HEX Kodu | Açıklama |
| :--- | :--- | :--- |
| **Canvas Background** | `#F8FAFC` | Ferah, modern sayfa arka planı (Slate-50) |
| **Card / Surface** | `#FFFFFF` | Beyaz kart zeminleri ve içerik alanları |
| **Heading / Dark** | `#0F172A` | Başlıklar, logo ve yüksek kontrastlı metinler (Slate-900) |
| **Body Text** | `#334155` | Standart paragraf ve gövde metinleri (Slate-700) |
| **Muted Text** | `#64748B` | İkincil metinler, tarihler, açıklamalar (Slate-500) |
| **Border / Divider** | `#E2E8F0` | Kart çerçeveleri ve ayırıcı çizgiler (Slate-200) |

### 3.3 Fonksiyonel ve Kategori Renkleri (Semantic Palette)
| Anlam / Kategori | HEX Kodu | Kullanım Alanı |
| :--- | :--- | :--- |
| **Kariyer & Başarı** | `#10B981` | İş ilanları, başarılı durumlar, "Full-time" rozetleri |
| **Etkinlik & Uyarı** | `#F59E0B` | Üniversite etkinlikleri, dikkat çeken rozetler |
| **Akademik & Bilgi** | `#0EA5E9` | Akademik duyurular, fakülte bilgilendirmeleri |
| **Hata / İptal** | `#EF4444` | 404 / 500 hata sayfaları, geçersiz istek uyarıları |

---

## 4. Tipografi (Typography)

Sistem genelinde temiz, modern ve okunabilirliği yüksek olan **Google Fonts - Plus Jakarta Sans** ailesi kullanılır:

```html
<link href="https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&display=swap" rel="stylesheet">
```

### 4.1 Hiyerarşi Tablosu
| Öğe | Ağırlık (Weight) | Boyut | Kullanım Yeri |
| :--- | :--- | :--- | :--- |
| **Display 1 (Hero)** | 800 (ExtraBold) | 2.5rem – 3.2rem | Ana sayfa karşılama sloganı |
| **H1 - H2 (Başlıklar)**| 800 (ExtraBold) | 1.8rem – 2.2rem | Sayfa ana başlıkları, bölüm başlıkları |
| **H3 - H5 (Kartlar)** | 700 (Bold) | 1.1rem – 1.4rem | Duyuru ve mezun kartı başlıkları |
| **Body / Paragraf** | 400 (Regular) – 500 (Medium) | 0.95rem – 1rem | Standart gövde metinleri |
| **Rozet / Buton** | 600 (SemiBold) | 0.85rem – 0.9rem | Aksiyon butonları, kategori hapları |
| **Small / Meta** | 500 (Medium) | 0.75rem – 0.8rem | Tarihler, yazarlar, alt dipnotlar |

---

## 5. UI Bileşenleri ve Tasarım Dili

### 5.1 Kartlar (Cards)
* **Kenarlık Yuvarlama:** `12px` - `18px` (`border-radius: 14px;`)
* **Kenarlık:** `1px solid #E2E8F0`
* **Gölge:** Hafif ve yayvan mikro-gölge (`box-shadow: 0 1px 3px rgba(0,0,0,0.05);`)
* **Hover Etkisi:** Kart üzerine gelindiğinde yukarı doğru `translateY(-3px)` hareketi ve yumuşak derinlik artışı.

### 5.2 Butonlar (Buttons)
* **Konekto Ana Buton (`.btn-konekto`):**
  * Zemin: `#4F46E5`
  * Yazı: Beyaz, `font-weight: 600`
  * Yuvarlatma: `8px` veya hap stili `50px` (`rounded-pill`)
  * Hover: `background-color: #4338CA; box-shadow: 0 4px 12px rgba(79, 70, 229, 0.3);`

### 5.3 Rozetler (Badges & Pills)
* Kategori ve durum bildirimlerinde pastel zemin üzerine canlı metin kombinasyonu (subtle badge deseni) kullanılır:
  * `bg-primary-subtle text-primary`
  * `bg-success-subtle text-success`
  * `bg-warning-subtle text-dark`

---

## 6. Hedef Kitle ve Kullanıcı Personaları

1. **Yeni Mezunlar & Tecrübeli Mezunlar:**
   * Kariyer ağını genişletmek, dönem arkadaşlarıyla irtibatta kalmak ve şirketlerine üniversiteden stajyer/çalışan almak isterler.
2. **Aktif Üniversite Öğrencileri:**
   * Sektördeki mezunların tecrübelerinden faydalanmak, mentorluk almak ve iş ilanlarına başvurmak isterler.
3. **Kariyer Merkezi ve Üniversite Yönetimi:**
   * Mezun başarı oranlarını izlemek, üniversite bültenlerini ve etkinlik duyurularını tek kanaldan iletmek isterler.
4. **Partner Şirketler:**
   * Nitelikli yeni mezun ve öğrenci havuzuna doğrudan erişip ilan yayınlamak isterler.

---

## 7. İlgili Belgeler & Bağlantılar

* 📊 **[Veri Tasarım Dokümanı (ddd.md)](./ddd.md)**
* 📐 **[Tasarım Sistemi Spesifikasyonu (DESIGN.md)](./DESIGN.md)**
* 📘 **[Ana Dokümantasyon ve Kurulum (README.md)](./README.md)**
