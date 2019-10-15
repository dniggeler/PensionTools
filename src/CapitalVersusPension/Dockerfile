FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["CapitalVersusPension/CapitalVersusPension.csproj", "CapitalVersusPension/"]
RUN dotnet restore "CapitalVersusPension/CapitalVersusPension.csproj"
COPY . .
WORKDIR "/src/CapitalVersusPension"
RUN dotnet build "CapitalVersusPension.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CapitalVersusPension.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CapitalVersusPension.dll"]