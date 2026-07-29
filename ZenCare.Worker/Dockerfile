FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["ZenCare.Worker/ZenCare.Worker.csproj", "ZenCare.Worker/"]
COPY ["ZenCare.Services/ZenCare.Services.csproj", "ZenCare.Services/"]
COPY ["ZenCare.Model/ZenCare.Model.csproj", "ZenCare.Model/"]
COPY ["ZenCare.Common/ZenCare.Common.csproj", "ZenCare.Common/"]

RUN dotnet restore "ZenCare.Worker/ZenCare.Worker.csproj"

COPY . .
RUN dotnet publish "ZenCare.Worker/ZenCare.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ZenCare.Worker.dll"]
