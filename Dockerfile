FROM mcr.microsoft.com/playwright/dotnet:v1.55.0-noble AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS publish
WORKDIR /src
COPY ["azimuth.csproj", "."]
RUN dotnet restore "azimuth.csproj"
COPY . .
RUN dotnet publish "azimuth.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Playwright browsers are already installed in the base image
ENTRYPOINT ["dotnet", "azimuth.dll"]