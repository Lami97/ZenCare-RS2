# ZenCare

ZenCare je RSII seminarski projekat za wellness centar. Rjesenje sadrzi .NET backend, WinForms administraciju, Flutter mobilnu aplikaciju za klijente, Worker servis, SQL Server bazu, RabbitMQ poruke i Stripe sandbox placanje.

## O projektu

ZenCare pokriva administraciju wellness usluga, proizvoda, korisnika, zaposlenika, termina, kupovina, recenzija, FAQ sadrzaja, dobavljaca, izvjestaja i preporuka. Klijentska mobilna aplikacija podrzava registraciju/prijavu, profil, proizvode, korpu, checkout, Stripe placanje/refund, termine, recenzije i preporuke.

## Tehnologije

- .NET 9 WebAPI, Services, Model i Common projekti
- Entity Framework Core 9 i SQL Server
- JWT Bearer autentifikacija i role-based autorizacija
- WinForms `net9.0-windows` administracija
- Flutter/Dart mobilna aplikacija
- Provider, Dio, SharedPreferences i flutter_stripe u mobilnoj aplikaciji
- RabbitMQ za backend event messaging
- Worker servis za pozadinsku obradu poruka
- Docker Compose za API, Worker, SQL Server i RabbitMQ
- Swagger/OpenAPI za testiranje API-ja
- AutoMapper za DTO mapiranja

## Arhitektura / projekti

| Projekat | Uloga |
| --- | --- |
| `ZenCare.WebAPI` | REST API, autentifikacija, autorizacija, Swagger, startup migracije i bootstrap admin |
| `ZenCare.Services` | EF Core DbContext, entiteti, poslovna logika, mapiranja, RabbitMQ i Stripe integracija |
| `ZenCare.Model` | DTO modeli, request/response klase, search objekti, enum-i i poruke |
| `ZenCare.Common` | Zajednicki .NET helper/common projekat |
| `ZenCare.WinUI` | WinForms administrativna desktop aplikacija |
| `ZenCare.Mobile` | Flutter mobilna aplikacija za klijente |
| `ZenCare.Worker` | BackgroundService koji konzumira RabbitMQ poruke |

## Funkcionalnosti

### Mobilna aplikacija

- Registracija i prijava klijenta
- JWT cuvanje sesije i automatski Bearer header
- Profil, edit profila, promjena lozinke i logout
- Pregled proizvoda, detalji proizvoda, korpa i historija kupovina
- Checkout iz korpe, Stripe PaymentIntent placanje, potvrda placanja, refund i otkazivanje neplacene narudzbe
- Pregled, kreiranje i otkazivanje vlastitih termina
- Kreiranje recenzija za dozvoljene termine/proizvode
- Pregled preporuka za proizvode i usluge sa objasnjenjem razloga

### WinUI administracija

WinUI se koristi za administratorski rad nad modulima:

- Product Categories, Product Types, Units, Products, Suppliers
- Service Categories, Services, Employees i Employee-Service veze kroz postojece forme
- Users, User Roles, Appointments, Reviews
- Purchases, Purchase Items, FAQ Categories, FAQ
- Reports sa prikazom statistika i PDF exportima

### Backend / Worker

- CRUD API za domenske module
- Klijentski `/My` endpointi za vlasnistvo nad terminima, kupovinama, korpom, placanjima, recenzijama, profilom i notifikacijama
- Centralizovana obrada `BusinessException` gresaka
- Automatske EF migracije pri startu API-ja
- Idempotentni bootstrap Admin korisnik za svjezu bazu
- RabbitMQ purchase event publisher i Worker consumer koji kreira notifikaciju za vlasnika kupovine

## Preduslovi

- .NET SDK 9
- Docker Desktop
- Flutter SDK, ako se pokrece mobilna aplikacija
- Android emulator ili fizicki uredjaj za Flutter Mobile
- Stripe test/sandbox nalog za placanje

## Konfiguracija okruzenja

Kreirati lokalni `.env` iz primjera:

```cmd
copy .env.example .env
```

Popuniti lokalne/test vrijednosti u `.env`. Fajl `.env` se ne smije commitovati.
Docker cita JWT vrijednosti iz `.env`, zato `JWT_SECRET_KEY` obavezno zamijeniti jakom lokalnom/test vrijednoscu. Za direktno lokalno pokretanje WebAPI-ja bez Dockera, `JwtToken__SecretKey` treba dati kroz environment variable ili .NET User Secrets, npr. `dotnet user-secrets set "JwtToken:SecretKey" "<your-local-secret>"`.

| Varijabla | Svrha | Required / Optional |
| --- | --- | --- |
| `MSSQL_SA_PASSWORD` | Lozinka za SQL Server `sa` korisnika u Dockeru | Required |
| `RABBITMQ_USERNAME` | RabbitMQ korisnicko ime | Required |
| `RABBITMQ_PASSWORD` | RabbitMQ lozinka | Required |
| `STRIPE_SECRET_KEY` | Stripe sandbox secret key za backend PaymentIntent/refund | Required za placanje |
| `STRIPE_CURRENCY` | Stripe valuta, default u compose je `usd` | Optional |
| `JWT_ISSUER` | JWT issuer | Required |
| `JWT_AUDIENCE` | JWT audience | Required |
| `JWT_SECRET_KEY` | JWT signing key | Required |
| `JWT_DURATION_IN_MINUTES` | Trajanje JWT tokena | Optional |
| `SWAGGER_ENABLED` | Ukljucuje Swagger u Docker Production okruzenju | Optional |
| `BOOTSTRAP_ADMIN_ENABLED` | Ukljucuje kreiranje prvog Admin korisnika | Optional |
| `BOOTSTRAP_ADMIN_FIRST_NAME` | Ime bootstrap Admin korisnika | Required ako je bootstrap ukljucen |
| `BOOTSTRAP_ADMIN_LAST_NAME` | Prezime bootstrap Admin korisnika | Required ako je bootstrap ukljucen |
| `BOOTSTRAP_ADMIN_EMAIL` | Email bootstrap Admin korisnika | Required ako je bootstrap ukljucen |
| `BOOTSTRAP_ADMIN_USERNAME` | Username bootstrap Admin korisnika | Required ako je bootstrap ukljucen |
| `BOOTSTRAP_ADMIN_PASSWORD` | Lozinka bootstrap Admin korisnika | Required ako je bootstrap ukljucen |

Mobilna aplikacija koristi compile-time konfiguraciju:

- `API_BASE_URL`, default: `http://10.0.2.2:5281` za Android emulator
- `STRIPE_PUBLISHABLE_KEY`, default placeholder: `pk_test_CHANGE_ME`

Za fizicki Android uredjaj koristiti host IP racunara, npr:

```cmd
flutter run -d <device-id> --dart-define=API_BASE_URL=http://<host-ip>:5281 --dart-define=STRIPE_PUBLISHABLE_KEY=<stripe-publishable-key>
```

## Brzo pokretanje projekta

### 1. Docker / backend

```cmd
copy .env.example .env
docker compose up -d --build
docker compose ps
```

API i Worker koriste SQL Server i RabbitMQ iz Docker Compose mreze. API automatski primjenjuje postojece EF migracije pri startu i nakon toga pokrece bootstrap Admin logiku ako je ukljucena.

### 2. Swagger

Swagger je dostupan na:

```text
http://localhost:5281/swagger
```

Docker API radi sa `ASPNETCORE_ENVIRONMENT=Production`, a Swagger se ukljucuje preko `SWAGGER_ENABLED` / `Swagger__Enabled`. Swagger ima Bearer JWT autorizaciju.

### 3. WinUI

Prvo mora raditi backend na `http://localhost:5281`.

```cmd
dotnet run --project ZenCare.WinUI
```

WinUI starta kroz `LoginForm` i koristi `http://localhost:5281` kao API base URL. Namijenjen je prvenstveno Admin korisniku.

### 4. Flutter Mobile

```cmd
cd ZenCare.Mobile
flutter pub get
flutter devices
flutter run -d <device-id>
```

Android emulator koristi default API URL `http://10.0.2.2:5281`. Za fizicki uredjaj pokrenuti aplikaciju sa `--dart-define=API_BASE_URL=http://<host-ip>:5281`.

## Demo korisnici

Migracije seedaju role `Admin`, `Employee` i `Client`, ali ne seedaju fiksne demo korisnike sa javno dokumentovanom lozinkom.

Za svjezu Docker bazu koristi se bootstrap Admin iz `.env` ako je `BOOTSTRAP_ADMIN_ENABLED=true`. Username, email i lozinka su vrijednosti koje unesete lokalno u `.env`.

Klijenti se mogu kreirati javnom registracijom kroz mobilnu aplikaciju ili endpoint `POST /Auth/Register`. Admin takodjer moze kreirati klijenta kroz `POST /User/Admin/create-client`.

## Autentifikacija i logout

Login koristi `POST /Auth/Login` i vraca JWT sa `ClaimTypes.NameIdentifier`, `ClaimTypes.Name`, `ClaimTypes.Role` za svaku rolu i `jti` identifikatorom tokena. Endpointi su zasticeni kombinacijom JWT autentifikacije, rola i `/My` ownership provjera.

Logout koristi `POST /Auth/Logout`. Token koji je poslan na logout se upisuje u `RevokedTokens`; isti JWT nakon toga vise ne prolazi autorizaciju, dok novi login izdaje novi vazeci JWT.

## RabbitMQ

Docker Compose pokrece RabbitMQ na:

- AMQP: `localhost:5672`
- Management UI: `http://localhost:15672`

Backend deklarise durable direct exchange `zencare.events` i queue-ove:

- `appointment-events`
- `purchase-events`
- `payment-events`
- `notification-events`

Checkout nakon uspjesnog commit-a baze objavljuje `PurchaseCreatedMessage` na routing key `purchase`. `ZenCare.Worker` konzumira `purchase-events`, validira poruku i kreira notifikaciju tipa `Purchase` za vlasnika kupovine. Ako RabbitMQ nije dostupan, API loguje upozorenje i ne vraca infrastrukturne detalje klijentu.

## Placanje / Stripe

Stripe sandbox integracija je backend + mobile tok:

- `POST /Payment/My/create-intent/{purchaseId}` kreira ili vraca postojeci PaymentIntent za vlastitu kupovinu
- `POST /Payment/My/confirm/{purchaseId}` cita status PaymentIntent-a iz Stripe-a i azurira purchase/payment statuse
- `POST /Payment/My/refund/{purchaseId}` pokrece puni Stripe refund za placenu vlastitu kupovinu

Backend nikada ne prima iznos placanja od klijenta; iznos se cita iz `Purchase.TotalAmount`. Secret key se cita iz `Stripe__SecretKey` / `STRIPE_SECRET_KEY`, a mobilna aplikacija treba publishable key preko `STRIPE_PUBLISHABLE_KEY`.

Za testiranje koristiti Stripe test/sandbox podatke iz vlastitog Stripe naloga i sluzbene Stripe test-mode dokumentacije. Ne upisivati tajne kljuceve u repozitorij.

## Sistem preporuke

ZenCare ima hibridni weighted-rule sistem preporuke za proizvode i usluge:

- historija kupovina/termina za content preference signale
- popularnost kroz PurchaseItems i Appointments
- boost iz odobrenih recenzija/ocjena
- objasnjiv `Reason` za svaku preporuku
- logovanje isporucenih preporuka u `RecommendationLogs`
- mobilni prikaz preporucenih proizvoda/usluga

Detalji algoritma, tezina, fallback logike i ogranicenja su u dokumentu:

[Detaljna dokumentacija sistema preporuke](recommender-dokumentacija.md)

## Preporuceni testni scenarij

1. Pokrenuti Docker infrastrukturu: `docker compose up -d --build`.
2. Otvoriti Swagger na `http://localhost:5281/swagger`.
3. Prijaviti se bootstrap Admin korisnikom iz lokalnog `.env`.
4. U WinUI provjeriti administraciju proizvoda, usluga, korisnika, zaposlenika, termina, recenzija, kupovina, FAQ-a, dobavljaca i izvjestaja.
5. U mobilnoj aplikaciji registrovati novog Client korisnika ili se prijaviti postojecim client nalogom.
6. Pregledati proizvode, dodati proizvod u korpu i uraditi checkout.
7. Kreirati Stripe PaymentIntent, testirati uspjesno placanje i refund u sandbox rezimu.
8. Kreirati termin koristeci Service Category -> Service -> Employee tok.
9. Otkazati dozvoljeni buduci termin i provjeriti poruku.
10. Kreirati recenziju kada backend pravila dozvole eligibilnost.
11. Otvoriti preporuke i provjeriti naziv, score i `Reason`.
12. Testirati logout: JWT radi prije logouta, `POST /Auth/Logout` ga revoke-uje, isti JWT poslije logouta vraca 401.

## Dodatna dokumentacija

- [Docker dokumentacija](DOCKER.md)
- [Detaljna dokumentacija sistema preporuke](recommender-dokumentacija.md)

## Zaustavljanje projekta

```cmd
docker compose down
```

Za brisanje Docker SQL Server i RabbitMQ podataka:

```cmd
docker compose down -v
```
