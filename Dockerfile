# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY *.sln ./
COPY Directory.Build.props ./
COPY src/TransferenciaMateriais.Api/TransferenciaMateriais.Api.csproj src/TransferenciaMateriais.Api/
COPY src/TransferenciaMateriais.Domain/TransferenciaMateriais.Domain.csproj src/TransferenciaMateriais.Domain/
COPY src/TransferenciaMateriais.Application/TransferenciaMateriais.Application.csproj src/TransferenciaMateriais.Application/
COPY src/TransferenciaMateriais.Infrastructure/TransferenciaMateriais.Infrastructure.csproj src/TransferenciaMateriais.Infrastructure/

# Restore dependencies (apenas projetos src, não tests)
RUN dotnet restore src/TransferenciaMateriais.Api/TransferenciaMateriais.Api.csproj

# Copy all source files
COPY src/ ./src/

# Build
WORKDIR /src/src/TransferenciaMateriais.Api
RUN dotnet build -c Release -o /app/build

# Publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published files
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080
EXPOSE 8081

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Run the application
ENTRYPOINT ["dotnet", "TransferenciaMateriais.Api.dll"]
