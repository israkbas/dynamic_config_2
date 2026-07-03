# Dynamic Config

Backend case'i için hazırladığım proje. Amaç, appsettings.json / web.config gibi dosyalarda tutulan config değerlerini dışarı çıkarıp, uygulamayı yeniden başlatmadan (deploy/restart gerekmeden) güncellenebilir hale getirmek.

Config kayıtları MongoDB'de tutuluyor. `ConfigReader` adında bir class library yazdım, bu kütüphane arka planda belirli aralıklarla Mongo'yu kontrol edip kendi cache'ini güncelliyor. Her servis sadece kendi `ApplicationName`'ine ait ve `IsActive: true` olan kayıtları görebiliyor, başka servisin kaydına erişemiyor.

## Proje yapısı

- `src/ConfigReader` — asıl kütüphane. `ConfigurationReader` sınıfı ve `GetValue<T>` metodu burada.
- `src/AdminApi` — kayıtları listelemek/eklemek/güncellemek için basit bir Web API, aynı zamanda `wwwroot/index.html` üzerinden tek sayfalık bir yönetim ekranı sunuyor.
- `src/ServiceA.Demo`, `src/ServiceB.Demo` — kütüphaneyi kullanan örnek konsol uygulamaları. İkisi de farklı `ApplicationName` ile çalışıp izolasyonu gösteriyor.
- `tests/ConfigReader.Tests` — tip dönüşümü (`ConfigValueConverter`) için unit testler.
- `mongo-init/init.js` — Mongo ilk açıldığında örnek verileri otomatik ekleyen script.

## Çalıştırma

Docker ve docker-compose yeterli, .NET SDK kurulu olmasına gerek yok:

```
docker compose up --build
```

Bu komut Mongo'yu, admin API'yi ve iki demo servisi ayağa kaldırıyor.

- Admin panel: http://localhost:5090
- API: http://localhost:5090/api/configs

Mongo bağlantı bilgisi `.env` dosyasından okunuyor. Repoyu ilk klonladığınızda `.env` bulunmaz (git'e dahil edilmiyor), `.env.example` dosyasını kopyalayıp `.env` olarak kaydetmeniz yeterli.

## Kütüphaneyi kullanmak

```csharp
var reader = new ConfigurationReader("SERVICE-A", "mongodb://localhost:27017", 5000);
var siteName = reader.GetValue<string>("SiteName");
var maxItemCount = reader.GetValue<int>("MaxItemCount");
```

Constructor 3 parametre alıyor: uygulama adı, Mongo connection string, kaç ms'de bir storage kontrol edilecek. Mongo'ya erişilemezse kütüphane elindeki son başarılı veriyle çalışmaya devam ediyor, bozulmuyor.

## Testler (Unit Test)

```
dotnet test tests/ConfigReader.Tests
```

## Bazı notlar

- Storage olarak MongoDB seçtim, çünkü Docker ile hızlıca ayağa kaldırılabiliyor ve daha önce docker ile bir deneyimim olmuştu.
- Mongo'da `Value` alanı hep string olarak tutuluyor, `GetValue<T>` çağrılırken istenen tipe (`string`/`int`/`double`/`bool`) çevriliyor. Bu dönüşüm `ConfigValueConverter` sınıfında, culture farklarından etkilenmeyecek şekilde yapılıyor (bunu unit testler yazarken fark ettim, bir ara sürpriz bir şekilde bozuluyordu).
- Cache güncellemesi `lock` ile korunuyor, arka plandaki timer ile aynı anda gelen `GetValue` çağrıları çakışmasın diye.
- Admin panelde kayıt ekleme/güncelleme var, isme göre filtreleme tamamen client-side (JS), sunucuya ekstra istek atmıyor.
