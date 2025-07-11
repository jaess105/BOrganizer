FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BOrganizer.Inertia/BOrganizer.Inertia.csproj", "BOrganizer.Inertia/"]
RUN dotnet restore "BOrganizer.Inertia/BOrganizer.Inertia.csproj"
COPY . .
WORKDIR "/src/BOrganizer.Inertia"
RUN dotnet build "./BOrganizer.Inertia.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BOrganizer.Inertia.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BOrganizer.Inertia.dll"]
