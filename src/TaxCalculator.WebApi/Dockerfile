FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
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

CMD ASPNETCORE_URLS=http://*:$PORT dotnet TaxCalculator.WebApi.dll