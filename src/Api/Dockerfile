# syntax=docker/dockerfile:1

# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first (better layer caching) by copying only project files.
COPY src/Shared/MarksBaseballCards.Shared.csproj src/Shared/
COPY src/Api/MarksBaseballCards.Api.csproj src/Api/
COPY src/Client/MarksBaseballCards.Client.csproj src/Client/
RUN dotnet restore src/Api/MarksBaseballCards.Api.csproj

# Copy the remaining source and publish (bundles the Blazor WASM client into wwwroot).
COPY . .
RUN dotnet publish src/Api/MarksBaseballCards.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Run as the non-root user provided by the base image.
USER $APP_UID

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MarksBaseballCards.Api.dll"]
