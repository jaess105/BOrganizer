FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BOrganizer/DB.Init/DB.Init.fsproj", "BOrganizer/DB.Init/"]
RUN dotnet restore "BOrganizer/DB.Init/DB.Init.fsproj"
COPY . .
WORKDIR "/src/BOrganizer/DB.Init"
RUN dotnet build "./DB.Init.fsproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DB.Init.fsproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DB.Init.dll"]
