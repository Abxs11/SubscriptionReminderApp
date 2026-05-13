# 🚀 Abonelik & Otomatik Ödeme Hatırlatıcı

Bu proje, kullanıcıların çeşitli aboneliklerini (GSM, İnternet, Su, Elektrik vb.) tek bir merkezden yönetmelerini, borçlarını sorgulamalarını ve ödeme yapmalarını sağlayan bir SaaS uygulamasıdır. Ayrıca, ödenmemiş borçlar için arka planda çalışan otomatik bir e-posta hatırlatma sistemine sahiptir.

## ✨ Özellikler

- **Abonelik Yönetimi:** Farklı kurumlar için abone numarası bazlı tanımlama.
- **Dönem Bazlı Borç Sorgulama:** Abonelik başlangıcından itibaren tüm aylar için borç durumunu şeffaf bir şekilde görebilme.
- **Kolay Ödeme:** Tek tıkla ödeme simülasyonu (External Mock API entegrasyonu).
- **Ödeme Geçmişi:** Yapılan tüm işlemlerin detaylı listesi ve durum takibi.
- **Profil Yönetimi:** Kişisel bilgilerin güncellenmesi ve şifre değiştirme.
- **Hesap Silme:** Güvenli bir şekilde hesabı ve tüm ilişkili verileri kalıcı olarak silme.
- **Dashboard:** Ödeme istatistikleri ve ödenmemiş fatura özetleri.
- **Otomatik Hatırlatıcı:** `MailKit` ve `.NET BackgroundService` ile ödenmemiş borçlar için periyodik mail bildirimi.
- **Güvenlik:** JWT tabanlı kimlik doğrulama ve `User Secrets` ile hassas veri yönetimi.

## 🛠️ Teknoloji Yığını

- **Backend:** .NET 8 (C#), Entity Framework Core
- **Frontend:** React 18, Vite, TypeScript, Vanilla CSS
- **Veritabanı:** PostgreSQL
- **Kütüphaneler:** MailKit, Lucide React, BCrypt.Net

## 🚀 Kurulum

### 1. Backend Hazırlığı
1. `SubscriptionReminder.Api` dizinine gidin.
2. `appsettings.json` içindeki PostgreSQL bağlantı dizesini güncelleyin.
3. Veritabanını oluşturun: `dotnet ef database update`
4. SMTP ayarlarını (User Secrets) yapılandırın:
   ```bash
   dotnet user-secrets set "EmailSettings:SmtpUser" "email@gmail.com"
   dotnet user-secrets set "EmailSettings:SmtpPass" "app-password"
   ```
5. Uygulamayı çalıştırın: `dotnet run`

### 2. Frontend Hazırlığı
1. `SubscriptionReminder.UI` dizinine gidin.
2. Paketleri kurun: `npm install`
3. Uygulamayı başlatın: `npm run dev`

---

## 📂 Dokümantasyon
Detaylı **ER Diyagramı**, **API Listesi** ve **İş Akışları** için [PROJECT_DOCS.md](./PROJECT_DOCS.md) dosyasını inceleyebilirsiniz.
