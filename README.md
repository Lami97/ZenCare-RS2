# ZenCare

ZenCare is an RSII seminar project for managing a wellness center. It combines a .NET 9 REST API, a Flutter Windows administration application, a Flutter Android client application, SQL Server, RabbitMQ, a background Worker, Stripe test payments, reporting, notifications, and recommendations.

## Implemented Applications

| Project | Responsibility |
| --- | --- |
| `ZenCare.WebAPI` | REST API, JWT authentication/authorization, EF Core migrations, evaluator seed, Swagger, Stripe and RabbitMQ publishing |
| `ZenCare.Services` | Business logic, EF Core data access, validation, mappings, security and integrations |
| `ZenCare.Model` | Request/response DTOs, search objects, enums and shared contracts |
| `ZenCare.Common` | Shared .NET helpers |
| `ZenCare.Desktop` | Required Flutter Windows administration application (Admin role) |
| `ZenCare.Mobile` | Required Flutter Android client application (Client role) |
| `ZenCare.Worker` | Separate RabbitMQ consumer that persists purchase/workflow notifications |
| `ZenCare.WinUI` | Legacy administration client retained in the solution; it is not the required evaluator desktop application |

## Main Features

- Admin management of users, roles, employees, employee-service assignments, services, products, suppliers, appointments, purchases, reviews, FAQ data, and related reference data.
- Server-side search, filtering, pagination, validation, ownership checks, and workflow state transitions.
- Business analytics in Flutter Desktop, including printable/exportable PDF reports.
- Client registration, login, profile management, password reset, and server-side token revocation on logout.
- Mobile service discovery, appointment booking/cancellation, product browsing, cart, checkout, purchase history, reviews, recommendations, and notifications.
- Real Stripe test-mode PaymentIntent verification and refund workflow.
- RabbitMQ messages processed by a separate Worker container.
- Content-based product recommendations using purchase, view, category, supplier, and review signals.

## Prerequisites

- Git
- Docker Desktop with Docker Compose
- .NET 9 SDK (for local build verification or direct API development)
- Flutter SDK with Windows desktop support
- Visual Studio Windows desktop C++ workload required by Flutter Windows
- Android Studio/SDK and an Android emulator or physical Android device
- A Stripe test account to exercise payment and refund workflows
- An SMTP test account only if password-reset email delivery will be evaluated

## Configuration

Create the local Compose environment file from the tracked template:

```cmd
cd /d "C:\path\to\ZenCare"
copy .env.example .env
```

Edit `.env` and replace every `CHANGE_ME` value that applies to the intended test. The local `.env` file is ignored by Git and must never be committed or included unencrypted in a release.

### Docker / Compose Values

| Variable | Purpose |
| --- | --- |
| `MSSQL_SA_PASSWORD` | Local SQL Server `sa` password; use a password accepted by SQL Server complexity rules |
| `RABBITMQ_HOST`, `RABBITMQ_PORT` | RabbitMQ container host and AMQP port |
| `RABBITMQ_USERNAME`, `RABBITMQ_PASSWORD` | RabbitMQ and Management UI credentials |
| `JWT_ISSUER`, `JWT_AUDIENCE`, `JWT_SECRET_KEY`, `JWT_DURATION_IN_MINUTES` | JWT validation/signing settings; use a strong local test signing key |
| `STRIPE_SECRET_KEY`, `STRIPE_CURRENCY` | Backend Stripe test secret key and currency |
| `CORS_ALLOWED_ORIGIN` | Explicit allowed browser origin |
| `BUSINESS_TIME_ZONE_ID` | Business time zone; Compose defaults to `Europe/Sarajevo` |
| `PASSWORD_RESET_EXPIRY_MINUTES` | Password-reset token lifetime |
| `SMTP_*` | SMTP delivery settings; required only for a real password-reset email test |
| `SWAGGER_ENABLED` | Enables Swagger in the Compose Production environment |
| `BOOTSTRAP_ADMIN_*` | Optional first-admin bootstrap; evaluator accounts do not depend on it |

The evaluator data seeder already creates an Admin account. For the standard evaluator scenario, either set `BOOTSTRAP_ADMIN_ENABLED=false` or provide a separate strong local password for the optional bootstrap Admin. Never leave a placeholder password enabled in a shared environment.

### Direct Local WebAPI Development

The recommended evaluator path is Docker Compose. A direct `dotnet run` does not automatically load the root `.env`; provide `ConnectionStrings__DefaultConnection` through an environment variable or .NET User Secrets. In the Development environment, ZenCare creates an in-memory JWT development fallback when JWT values are absent. Production/Compose still requires the explicit JWT values above.

Example User Secrets configuration with placeholders:

```cmd
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=ZenCareDb;User Id=sa;Password=<your-local-password>;TrustServerCertificate=True;Encrypt=False" --project ZenCare.WebAPI
dotnet user-secrets set "JwtToken:SecretKey" "<your-long-local-test-secret>" --project ZenCare.WebAPI
dotnet run --project ZenCare.WebAPI
```

Do not run the Compose `api` service and a direct local WebAPI simultaneously on port `5281`.

## Quick Start

### 1. Start Infrastructure, API, and Worker

From the repository root:

```cmd
copy .env.example .env
docker compose config --quiet
docker compose up -d --build
docker compose ps
```

Compose starts all four required services:

| Service | Address / port |
| --- | --- |
| API and Swagger | `http://localhost:5281` / `http://localhost:5281/swagger` |
| SQL Server | `localhost,1433` |
| RabbitMQ AMQP | `localhost:5672` |
| RabbitMQ Management | `http://localhost:15672` |
| Worker | No host port; runs as `zencare-worker` |

The Worker is already started by Compose and must not also be started manually. Useful checks:

```cmd
docker compose logs -f api
docker compose logs -f worker
```

### 2. Start Flutter Desktop

The API must already be reachable at `http://localhost:5281`.

```cmd
cd ZenCare.Desktop
flutter pub get
flutter run -d windows --dart-define=API_BASE_URL=http://localhost:5281
```

`API_BASE_URL` is mandatory for `ZenCare.Desktop`. Sign in with the seeded Admin account from the credentials table below.

### 3. Start Flutter Mobile

List devices and use the reported Android device identifier:

```cmd
cd ZenCare.Mobile
flutter pub get
flutter devices
flutter run -d <android-device-id> --dart-define=API_BASE_URL=http://10.0.2.2:5281 --dart-define=STRIPE_PUBLISHABLE_KEY=<your-stripe-test-publishable-key>
```

`10.0.2.2` is the Android emulator address for the host machine. For a physical device, replace it with the development computer's reachable LAN address, for example `http://192.168.1.20:5281`. The backend Stripe secret key and Mobile publishable key must be test keys from the same Stripe account. A real test publishable key is required for PaymentSheet; the tracked fallback is intentionally only `pk_test_CHANGE_ME`.

## Evaluator Accounts

The API runs an idempotent evaluator seed after applying migrations. These deterministic test accounts are created automatically on a fresh database:

| Application / context | Role | Username | Password |
| --- | --- | --- | --- |
| Flutter Desktop | Admin | `evaluator.admin` | `Demo123!` |
| API/Swagger role testing | Employee | `evaluator.employee` | `Demo123!` |
| API/Swagger role testing | Employee | `evaluator.employee2` | `Demo123!` |
| Flutter Mobile | Client | `evaluator.client` | `Demo123!` |
| Flutter Mobile | Client | `evaluator.client2` | `Demo123!` |

Flutter Desktop intentionally accepts only the Admin role. Flutter Mobile is the Client application. Employee accounts support the Employee-authorized API/appointment scenarios; ZenCare does not claim a separate Employee client application.

## Seeded Demo Data

The idempotent evaluator scenario includes:

- 5 users across Admin, Employee, and Client roles
- 2 available employees and 4 employee-service assignments
- 3 active wellness services and 4 active products
- 2 suppliers and Client carts
- 5 appointments covering Completed, Cancelled, and NoShow history
- 4 purchases covering Completed/Succeeded and Cancelled states
- eligible appointment/product reviews, product-view signals, FAQ entries, and Client notifications
- data used by reports, analytics, and recommendations

The seeded completed purchases support history, reports, review eligibility, and recommendations. To verify the real Stripe integration, create a new cart/checkout purchase and complete it with Stripe test-mode payment data.

## Suggested Evaluation Flow

### Admin / Flutter Desktop

1. Sign in as `evaluator.admin`.
2. Verify searchable/paginated management modules and employee-service assignments.
3. Inspect appointments, purchase fulfillment, reviews, notifications, and seeded history.
4. Open business analytics and generate the available PDF reports.

### Client / Flutter Mobile

1. Sign in as `evaluator.client` or register a new Client.
2. Browse/search services and products; inspect details and recommendations.
3. Book and cancel an eligible future appointment through the supported workflow.
4. Add a product to the cart, checkout, and complete payment with Stripe test data.
5. Inspect purchase history, reviews, profile, and notifications.

### Employee / API

Use `evaluator.employee` in Swagger to verify only the Employee-authorized appointment/status operations. Employee-service assignment writes remain Admin-only.

## External Integrations

### Stripe

- Backend: configure `STRIPE_SECRET_KEY` in `.env` with a Stripe test secret key.
- Mobile: provide the matching test publishable key through `--dart-define=STRIPE_PUBLISHABLE_KEY=...`.
- The Client checkout uses Stripe PaymentSheet; the backend retrieves and verifies Stripe state before marking payment successful.
- Refunds use the Stripe test API. No real Stripe key belongs in Git.

### RabbitMQ and Worker

Compose starts `rabbitmq:3.13.7-management` and the separate `zencare-worker` container. The Worker consumes purchase and notification events and persists user notifications; it performs real database work rather than log-only processing.

Open `http://localhost:15672` and sign in with `RABBITMQ_USERNAME` / `RABBITMQ_PASSWORD` from the local `.env`. More container details are in [DOCKER.md](DOCKER.md).

### SMTP / Password Reset

Registration and login do not require SMTP. To test password-reset email delivery, replace the `SMTP_*` placeholders with credentials for a safe test SMTP account. Do not commit those credentials.

## Database Initialization

- Engine: Microsoft SQL Server 2022 in Docker.
- Compose database: `ZenCareDb`.
- The API waits/retries for SQL Server, applies existing EF Core migrations, then runs optional bootstrap Admin logic and the idempotent evaluator seed.
- SQL Server data persists in the `sqlserver-data` named volume; RabbitMQ data persists in `rabbitmq-data`.
- A direct local API uses whichever database is supplied through `ConnectionStrings__DefaultConnection`.

The current Compose database name is `ZenCareDb`. The official RSII index-based database naming check remains a separate pre-submission item and is intentionally not changed by this documentation update.

## Build and Verification

From the repository root:

```cmd
dotnet build ZenCare.sln
docker compose config --quiet
```

Flutter checks:

```cmd
cd ZenCare.Desktop
flutter analyze
flutter test

cd ..\ZenCare.Mobile
flutter analyze
flutter test
```

## Stop or Reset Local Containers

Stop containers while preserving data:

```cmd
docker compose down
```

Remove containers and the named database/RabbitMQ volumes only when an intentional fresh-database test is required:

```cmd
docker compose down -v
```

`docker compose down -v` permanently removes local Docker data. The next API startup recreates the schema and evaluator data through migrations and seeding.

## Additional Documentation

- [Docker setup](DOCKER.md)
- [Recommendation system](recommender-dokumentacija.md)
