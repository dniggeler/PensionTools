FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
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
COPY ["Tax.Data/files/TaxDb.db", "data/TaxDb.db"]
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaxCalculator.WebApi.dll"]