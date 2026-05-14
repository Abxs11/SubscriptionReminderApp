# 📝 Proje Teknik Dokümantasyonu

Bu doküman, **Abonelik & Otomatik Ödeme Hatırlatıcı** projesinin mimarisini, veri yapısını ve iş akışlarını detaylandırmaktadır.

---

## 1. ER Diagram (Varlık İlişki Diyagramı)

Sistem; müşteriler, kullanıcılar, abonelikler ve bu aboneliklere bağlı ödeme/hatırlatma kayıtları üzerine kuruludur.

```mermaid
erDiagram
    CUSTOMER ||--|| USER : "has account"
    CUSTOMER ||--o{ SUBSCRIPTION : "owns"
    CUSTOMER ||--o{ SAVED_CARD : "saves"
    SUBSCRIPTION ||--o{ PAYMENT : "has"
    SUBSCRIPTION ||--o{ DEBT_INQUIRY : "has queries"
    SUBSCRIPTION ||--o{ REMINDER_LOG : "has notifications"

    CUSTOMER {
        int Id PK
        string FirstName
        string LastName
        string Tckn UK
        string Email UK
        string PhoneNumber
    }

    SAVED_CARD {
        int Id PK
        int CustomerId FK
        string CardHolderName
        string MaskedCardNumber
        string ExpiryDate
        datetime CreatedAtUtc
    }

    USER {
        int Id PK
        int CustomerId FK
        string Email UK
        string PasswordHash
        string Role
    }

    SUBSCRIPTION {
        int Id PK
        int CustomerId FK
        string Type "GSM, Internet, etc."
        string ProviderName
        string SubscriberNumber
        string Status "Active/Passive"
        datetime CreatedAtUtc
    }

    PAYMENT {
        int Id PK
        int SubscriptionId FK
        decimal Amount
        string Period "yyyy-MM"
        datetime PaymentDateUtc
        string Status "Success/Failed"
        string ExternalTransactionId
    }

    DEBT_INQUIRY {
        int Id PK
        int SubscriptionId FK
        decimal Amount
        string Period
        datetime DueDate
        datetime QueriedAtUtc
    }

    REMINDER_LOG {
        int Id PK
        int SubscriptionId FK
        string Period
        datetime SentAtUtc
        string Status
        string Message
    }
```

---

## 2. API Endpoint Listesi

Uygulama RESTful prensiplerine uygun olarak tasarlanmıştır. Tüm endpointler (Auth hariç) `Bearer Token` gerektirir.

### 🔐 Kimlik Doğrulama (Auth)
- `POST /api/Auth/register` - Yeni kullanıcı kaydı.
- `POST /api/Auth/login` - Giriş ve JWT token üretimi.

### 👥 Müşteri Yönetimi (Customers)
- `GET /api/Customers` - Tüm müşterileri listeler (Admin).
- `GET /api/Customers/{id}` - Müşteri detayı.
- `POST /api/Customers` - Yeni müşteri oluşturur.
- `DELETE /api/Customers/me` - Mevcut kullanıcının hesabını ve tüm verilerini siler.
- `POST /api/Customers/update-password` - Şifre değiştirme işlemi.

### 📡 Abonelik Yönetimi (Subscriptions)
- `POST /api/Subscriptions` - Yeni abonelik tanımlar.
- `GET /api/Subscriptions/customer/{customerId}` - Müşterinin aboneliklerini getirir.
- `PUT /api/Subscriptions/{id}` - Abonelik günceller.
- `DELETE /api/Subscriptions/{id}` - Abonelik siler (Cascading).

### 💸 Borç ve Ödeme (Inquiries & Payments)
- `POST /api/DebtInquiries/{id}/query` - Anlık borç sorgular (External Mock API).
- `GET /api/DebtInquiries/{id}/status/{period}` - Belirli bir ayın ödeme durumunu sorgular.
- `POST /api/Payments` - Ödeme gerçekleştirir (External Mock API).
- `GET /api/Payments/history` - Kullanıcının tüm ödeme geçmişini getirir.
- `GET /api/Summaries/dashboard` - Genel istatistikleri getirir.
- `GET /api/Summaries/unpaid` - Ödenmemiş faturaları listeler.
- `GET /api/Summaries/reminder-logs` - Giden son hatırlatma maillerini listeler.

---

## 3. İş Akış Diyagramı

Borç sorgulama, ödeme ve otomatik hatırlatma süreçlerinin uçtan uca akışı:

```mermaid
sequenceDiagram
    participant User as Kullanıcı / Admin
    participant BG as Background Service
    participant API as Web API
    participant DB as PostgreSQL
    participant Ext as Mock External API
    participant Mail as Email Service (MailKit)

    Note over User, Ext: Borç Sorgulama ve Ödeme Akışı
    User->>API: Borç Sorgula (SubscriptionId)
    API->>Ext: QueryDebtAsync(SubNo, Period)
    Ext-->>API: Borç Detayları (Amount, DueDate)
    API->>DB: DebtInquiry Kaydet
    API-->>User: Borç Bilgisini Göster
    User->>API: Ödeme Yap (Card Details)
    API->>Ext: ProcessPaymentAsync(Amount)
    Ext-->>API: Success (TransactionId)
    API->>DB: Payment Kaydet
    API-->>User: Ödeme Başarılı Mesajı

    Note over BG, Mail: Otomatik Hatırlatma Akışı
    loop 10 Saniyede Bir (Test)
        BG->>DB: Aktif Abonelikleri Getir
        DB-->>BG: Abonelik Listesi
        loop Her Abonelik İçin
            BG->>DB: Ödenmemiş Ay Var mı?
            DB-->>BG: Var (Örn: 2026-05)
            BG->>DB: Bugün Mail Atıldı mı?
            DB-->>BG: Hayır
            BG->>Mail: Hatırlatma Maili Gönder
            Mail-->>BG: Başarılı
            BG->>DB: ReminderLog Kaydet
        end
    end
```
