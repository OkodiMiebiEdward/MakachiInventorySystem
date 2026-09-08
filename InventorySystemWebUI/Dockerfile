# Linux-friendly multistage Dockerfile for .NET 8
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Install Node.js (required by Microsoft.TypeScript.MSBuild)
RUN apt-get update && \
    curl -fsSL https://deb.nodesource.com/setup_20.x | bash - && \
    apt-get install -y nodejs && \
    rm -rf /var/lib/apt/lists/*

# Copy only csproj to leverage Docker layer caching for NuGet restore
COPY ["InventorySystemWebUI/InventorySystemWebUI.csproj", "InventorySystemWebUI/"]
RUN dotnet restore "InventorySystemWebUI/InventorySystemWebUI.csproj"

# Copy rest of the solution and publish
COPY . .
WORKDIR "/src/InventorySystemWebUI"
RUN dotnet publish "InventorySystemWebUI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "InventorySystemWebUI.dll"]
