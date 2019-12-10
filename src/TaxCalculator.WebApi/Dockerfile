FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["TaxCalculator.WebApi/TaxCalculator.WebApi.csproj", "TaxCalculator.WebApi/"]
RUN dotnet restore "TaxCalculator.WebApi/TaxCalculator.WebApi.csproj"
COPY . .
WORKDIR "/src/TaxCalculator.WebApi"
RUN dotnet build "TaxCalculator.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaxCalculator.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY ["Tax.Data/files/TaxDb.db", "/data/TaxDb.db"]
COPY --from=publish /app/publish .

# Do not run as root user
RUN chown -R www-data:www-data /app
USER www-data

ENTRYPOINT ["dotnet", "TaxCalculator.WebApi.dll"]