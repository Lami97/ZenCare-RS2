FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["ZenCare.WebAPI/ZenCare.WebAPI.csproj", "ZenCare.WebAPI/"]
COPY ["ZenCare.Services/ZenCare.Services.csproj", "ZenCare.Services/"]
COPY ["ZenCare.Model/ZenCare.Model.csproj", "ZenCare.Model/"]
COPY ["ZenCare.Common/ZenCare.Common.csproj", "ZenCare.Common/"]

RUN dotnet restore "ZenCare.WebAPI/ZenCare.WebAPI.csproj"

COPY . .
RUN dotnet publish "ZenCare.WebAPI/ZenCare.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ZenCare.WebAPI.dll"]
