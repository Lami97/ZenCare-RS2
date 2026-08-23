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
- Matching Stripe test keys from the protected evaluator configuration package to exercise payment and refund workflows
- Working SMTP test sender settings from the protected package only if password-reset email delivery will be evaluated

Verified development environment: Flutter 3.44.8 (stable), Dart 3.12.2, and .NET 9 SDK.

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
| `MSSQL_DATABASE` | Evaluator database name; defaults to the student index `210143` |
| `RABBITMQ_HOST`, `RABBITMQ_PORT` | RabbitMQ container host and AMQP port |
| `RABBITMQ_USERNAME`, `RABBITMQ_PASSWORD` | RabbitMQ and Management UI credentials |
| `JWT_ISSUER`, `JWT_AUDIENCE`, `JWT_SECRET_KEY`, `JWT_DURATION_IN_MINUTES` | JWT validation/signing settings; use a strong local test signing key |
| `STRIPE_SECRET_KEY`, `STRIPE_CURRENCY` | Backend Stripe test secret key and currency |
| `STRIPE_PUBLISHABLE_KEY` | Matching Mobile test publishable key; Compose ignores it, but it is supplied to Flutter with `--dart-define` |
| `CORS_ALLOWED_ORIGIN` | Explicit allowed browser origin |
| `BUSINESS_TIME_ZONE_ID` | Business time zone; Compose defaults to `Europe/Sarajevo` |
| `PASSWORD_RESET_EXPIRY_MINUTES` | Password-reset token lifetime |
| `SMTP_*` | SMTP delivery settings; required only for a real password-reset email test |
| `SWAGGER_ENABLED` | Enables Swagger in the Compose Production environment |
| `BOOTSTRAP_ADMIN_*` | Optional first-admin bootstrap; evaluator accounts do not depend on it |

The evaluator data seeder already creates an Admin account. For the standard evaluator scenario, either set `BOOTSTRAP_ADMIN_ENABLED=false` or provide a separate strong local password for the optional bootstrap Admin. Never leave a placeholder password enabled in a shared environment.

### Protected Evaluator Configuration

The official submission requires an encrypted `.env-tajne.zip` in the repository root, beside the location where `.env` is used. The archive must contain the complete working evaluator `.env`, including matching Stripe test keys and working SMTP test settings if SMTP delivery will be demonstrated. Protect the archive with the submission password and provide that password through DLWMS as instructed by the course.

Before submission, prepare the archive from the working local configuration. Do not commit the unencrypted `.env` or include it in the build ZIP or GitHub Release. SMTP credentials and passwords must remain in protected configuration and never be committed to Git. A dedicated test SMTP account is recommended, but an existing working SMTP account is also acceptable when its credentials are protected. The tracked `.env.example` remains placeholder-only.

### Direct Local WebAPI Development

The recommended evaluator path is Docker Compose. A direct `dotnet run` does not automatically load the root `.env`; provide `ConnectionStrings__DefaultConnection` through an environment variable or .NET User Secrets. In the Development environment, ZenCare creates an in-memory JWT development fallback when JWT values are absent. Production/Compose still requires the explicit JWT values above.

Example User Secrets configuration with placeholders:

```cmd
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=210143;User Id=sa;Password=<your-local-password>;TrustServerCertificate=True;Encrypt=False" --project ZenCare.WebAPI
dotnet user-secrets set "JwtToken:SecretKey" "<your-long-local-test-secret>" --project ZenCare.WebAPI
dotnet run --project ZenCare.WebAPI
```

Do not run the Compose `api` service and a direct local WebAPI simultaneously on port `5281`.

## Quick Start

### 1. Start Infrastructure, API, and Worker

For evaluator testing, extract `.env` from the supplied `.env-tajne.zip` into the repository root using the password provided through DLWMS. For ordinary development without the protected package, copy `.env.example` to `.env` and replace its placeholders.

Then run from the repository root:

```cmd
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
flutter run -d <android-device-id> --dart-define=API_BASE_URL=http://10.0.2.2:5281 --dart-define=STRIPE_PUBLISHABLE_KEY=<matching-test-publishable-key-from-.env-tajne.zip>
```

`10.0.2.2` is the Android emulator address for the host machine. For a physical device, replace it with the development computer's reachable LAN address, for example `http://192.168.1.20:5281`. The backend Stripe secret key and Mobile publishable key must be test keys from the same Stripe account. A real test publishable key is required for PaymentSheet; the tracked fallback is intentionally only `pk_test_CHANGE_ME`.

For the evaluator release APK, embed the same protected test publishable key at build time:

```cmd
flutter build apk --release --dart-define=API_BASE_URL=http://10.0.2.2:5281 --dart-define=STRIPE_PUBLISHABLE_KEY=<matching-test-publishable-key-from-.env-tajne.zip>
```

The publishable key is client-side configuration and is embedded in the APK. The Stripe secret key remains backend-only in the protected `.env`. When the protected package and prebuilt APK are supplied, the evaluator does not need to create a Stripe account.

## Evaluator Accounts

The API runs an idempotent evaluator seed after applying migrations. These deterministic test accounts are created automatically on a fresh database:

| Application / context | Role | Username | Password |
| --- | --- | --- | --- |
| Flutter Desktop | Admin | `evaluator.admin` | `Demo123!` |
| API/Swagger role testing | Employee | `evaluator.employee` | `Demo123!` |
| API/Swagger role testing | Employee | `evaluator.employee2` | `Demo123!` |
| Flutter Mobile | Client | `evaluator.client` | `Demo123!` |
| Flutter Mobile | Client | `evaluator.client2` | `Demo123!` |

Flutter Desktop intentionally accepts only the Admin role. Flutter Mobile is the Client application. Employee accounts are included for role-authorization verification; ZenCare does not claim a separate Employee client application.

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

### Employee Authorization / API

Use `evaluator.employee` in Swagger to verify that administrative Appointment, Notification, and EmployeeService write endpoints return `403 Forbidden`. Employee accounts do not have a separate client application.

## External Integrations

### Stripe

- Backend: extract the protected evaluator `.env`; Compose receives `STRIPE_SECRET_KEY` from it.
- Mobile source run: provide the matching `STRIPE_PUBLISHABLE_KEY` from the protected package through `--dart-define`.
- Mobile release: build the APK with that publishable key as shown above; never embed the backend secret key.
- The Client checkout uses Stripe PaymentSheet; the backend retrieves and verifies Stripe state before marking payment successful.
- Refunds use the Stripe test API. No real Stripe key belongs in Git.
- Both values must be test-mode keys from the same Stripe account. The evaluator should not substitute an unrelated publishable key.

### RabbitMQ and Worker

Compose starts `rabbitmq:3.13.7-management` and the separate `zencare-worker` container. The Worker consumes purchase and notification events and persists user notifications; it performs real database work rather than log-only processing.

Open `http://localhost:15672` and sign in with `RABBITMQ_USERNAME` / `RABBITMQ_PASSWORD` from the local `.env`. More container details are in [DOCKER.md](DOCKER.md).

### SMTP / Password Reset

ZenCare uses SMTP to deliver reset tokens for the required Mobile Forgot/Reset Password flow. Mailtrap Email Sandbox is the recommended evaluator and development option because it captures outgoing messages without delivering them to a real Gmail, Outlook, or other recipient inbox. Domain verification is not required for Email Sandbox testing.

In Mailtrap, obtain the credentials from **Email Testing / Email Sandbox -> Sandbox -> SMTP / Integration credentials**. Configure the protected evaluator `.env` with the following ZenCare variables:

```text
SMTP_HOST=sandbox.smtp.mailtrap.io
SMTP_PORT=2525
SMTP_USERNAME=<mailtrap-sandbox-username>
SMTP_PASSWORD=<mailtrap-sandbox-password>
SMTP_USE_SSL=true
SMTP_FROM_ADDRESS=<configured-test-sender>
SMTP_FROM_NAME=ZenCare
```

The username and password must remain only in protected configuration and must never be committed to Git. `.env.example` remains placeholder-only. The supplied protected SMTP configuration may be used, or the evaluator may substitute credentials from their own Mailtrap Sandbox. A personal email account or app password is not required.

The reset request email must belong to an existing ZenCare Client account, even though Mailtrap captures the message instead of delivering it to that address. To verify the complete flow:

1. Start the ZenCare backend with working SMTP configuration.
2. Open ZenCare Mobile.
3. Register a temporary Client with an email address.
4. Log out.
5. Select **Forgot Password**.
6. Enter the same email used by the registered Client.
7. Open the Mailtrap Email Sandbox inbox.
8. Open the captured ZenCare password-reset email.
9. Copy the reset token.
10. Enter the token and a new password in ZenCare Mobile.
11. Confirm that the reset succeeds.
12. Sign in using the new password.

The neutral response, `If an account exists, password reset instructions have been sent.`, intentionally protects against account enumeration. It does not by itself prove that an email was sent; verify delivery in the Mailtrap Sandbox inbox.

## Database Initialization

- Engine: Microsoft SQL Server 2022 in Docker.
- Compose database: `210143` (the student index without the `IB` prefix).
- The API waits/retries for SQL Server, applies existing EF Core migrations, then runs optional bootstrap Admin logic and the idempotent evaluator seed.
- SQL Server data persists in the `sqlserver-data` named volume; RabbitMQ data persists in `rabbitmq-data`.
- A direct local API uses whichever database is supplied through `ConnectionStrings__DefaultConnection`.

The Compose database name is configured through `MSSQL_DATABASE` and defaults to the required student index `210143`.

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
