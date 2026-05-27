FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY backend/WarehouseInventory.Api/WarehouseInventory.Api.csproj backend/WarehouseInventory.Api/
COPY backend/WarehouseInventory.Api.Tests/WarehouseInventory.Api.Tests.csproj backend/WarehouseInventory.Api.Tests/
RUN dotnet restore backend/WarehouseInventory.Api/WarehouseInventory.Api.csproj
RUN dotnet restore backend/WarehouseInventory.Api.Tests/WarehouseInventory.Api.Tests.csproj

COPY backend/ backend/
WORKDIR /src/backend/WarehouseInventory.Api.Tests
RUN dotnet test WarehouseInventory.Api.Tests.csproj -c Release --logger:console

WORKDIR /src/backend/WarehouseInventory.Api
RUN dotnet publish WarehouseInventory.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "WarehouseInventory.Api.dll"]
