# ZenCare Docker

This Docker Compose setup runs the ZenCare backend infrastructure:

- WebAPI
- Worker
- SQL Server
- RabbitMQ with Management UI

The compose setup uses environment variables for all secrets. The `.env` file must never be committed.

## First-Time Setup

Create a local `.env` file from the example:

```cmd
copy .env.example .env
```

Edit `.env` and replace every `CHANGE_ME` value with local development secrets.

## Validate Compose Configuration

```cmd
docker compose config
```

## Build Images

```cmd
docker compose build
```

## Start Services

```cmd
docker compose up -d
```

## Check Status

```cmd
docker compose ps
```

## View Logs

API logs:

```cmd
docker compose logs -f api
```

Worker logs:

```cmd
docker compose logs -f worker
```

## Stop Services

```cmd
docker compose down
```

Stop services and remove persisted SQL Server and RabbitMQ data:

```cmd
docker compose down -v
```

## Ports

- WebAPI / Swagger: `http://localhost:5281/swagger`
- SQL Server: `localhost,1433`
- RabbitMQ AMQP: `localhost:5672`
- RabbitMQ Management: `http://localhost:15672`

## Docker Images

- WebAPI build: `mcr.microsoft.com/dotnet/sdk:9.0`
- WebAPI runtime: `mcr.microsoft.com/dotnet/aspnet:9.0`
- Worker build: `mcr.microsoft.com/dotnet/sdk:9.0`
- Worker runtime: `mcr.microsoft.com/dotnet/runtime:9.0`
- SQL Server: `mcr.microsoft.com/mssql/server:2022-CU16-ubuntu-22.04`
- RabbitMQ: `rabbitmq:3.13.7-management`

## Required Environment Variables

The `.env.example` file contains the required keys:

```text
MSSQL_SA_PASSWORD=CHANGE_ME
RABBITMQ_USERNAME=guest
RABBITMQ_PASSWORD=guest
STRIPE_SECRET_KEY=sk_test_CHANGE_ME
STRIPE_CURRENCY=usd
JWT_ISSUER=ZenCare
JWT_AUDIENCE=ZenCareUsers
JWT_SECRET_KEY=CHANGE_ME_TO_A_LONG_RANDOM_SECRET
JWT_DURATION_IN_MINUTES=60
BOOTSTRAP_ADMIN_ENABLED=true
BOOTSTRAP_ADMIN_FIRST_NAME=System
BOOTSTRAP_ADMIN_LAST_NAME=Administrator
BOOTSTRAP_ADMIN_EMAIL=admin@zencare.local
BOOTSTRAP_ADMIN_USERNAME=admin
BOOTSTRAP_ADMIN_PASSWORD=CHANGE_ME_TO_A_STRONG_PASSWORD
```

## SQL Server

Host connection:

```text
Server=localhost,1433;Database=ZenCareDb;User Id=sa;Password=<MSSQL_SA_PASSWORD>;TrustServerCertificate=True;Encrypt=False
```

Inside Docker, API and Worker use:

```text
Server=sqlserver,1433;Database=ZenCareDb;User Id=sa;Password=<MSSQL_SA_PASSWORD>;TrustServerCertificate=True;Encrypt=False
```

The API applies existing EF migrations on startup with retry handling, so a fresh SQL Server container receives the current schema and migration seed data.

## RabbitMQ Management

Open:

```text
http://localhost:15672
```

Log in with values from:

- `RABBITMQ_USERNAME`
- `RABBITMQ_PASSWORD`

## Volumes

Named Docker volumes:

- `sqlserver-data`
- `rabbitmq-data`

## Network

All services share:

- `zencare-network`

