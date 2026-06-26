# Mansari.Store.Basket/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["Mansari.Store.Basket.sln", "./"]
COPY ["src/Mansari.Store.Basket.API/Mansari.Store.Basket.API.csproj", "src/Mansari.Store.Basket.API/"]
COPY ["src/Mansari.Store.Basket.Application/Mansari.Store.Basket.Application.csproj", "src/Mansari.Store.Basket.Application/"]
COPY ["src/Mansari.Store.Basket.Domain/Mansari.Store.Basket.Domain.csproj", "src/Mansari.Store.Basket.Domain/"]
COPY ["src/Mansari.Store.Basket.Infrastructure/Mansari.Store.Basket.Infrastructure.csproj", "src/Mansari.Store.Basket.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/Mansari.Store.Basket.API/Mansari.Store.Basket.API.csproj"

# Copy all source files
COPY . .

# Build the project
WORKDIR "/src/src/Mansari.Store.Basket.API"
RUN dotnet build "Mansari.Store.Basket.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Mansari.Store.Basket.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mansari.Store.Basket.API.dll"]
