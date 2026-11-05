# Zobacz https://aka.ms/customizecontainer, aby dowiedzieć się, jak dostosować kontener debugowania i jak program Visual Studio używa tego pliku Dockerfile do kompilowania obrazów w celu szybszego debugowania.

# Ten etap jest używany podczas uruchamiania z programu VS w trybie szybkim (domyślnie dla konfiguracji debugowania)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 7150
EXPOSE 5240


# Ten etap służy do kompilowania projektu usługi
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MachinesDataAnalyzingSystem.WebAPI/MachinesDataAnalyzingSystem.WebAPI.csproj", "MachinesDataAnalyzingSystem.WebAPI/"]
COPY ["MachinesDataAnalyzingSystem.Core/MachinesDataAnalyzingSystem.Core.csproj", "MachinesDataAnalyzingSystem.Core/"]
COPY ["MachinesDataAnalyzingSystem.Shared/MachinesDataAnalyzingSystem.Shared.csproj", "MachinesDataAnalyzingSystem.Shared/"]
COPY ["MachinesDataAnalyzingSystem.Infrastructure/MachinesDataAnalyzingSystem.Infrastructure.csproj", "MachinesDataAnalyzingSystem.Infrastructure/"]
RUN dotnet restore "./MachinesDataAnalyzingSystem.WebAPI/MachinesDataAnalyzingSystem.WebAPI.csproj"
COPY . .
WORKDIR "/src/MachinesDataAnalyzingSystem.WebAPI"
RUN dotnet build "./MachinesDataAnalyzingSystem.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Ten etap służy do publikowania projektu usługi do skopiowania do etapu końcowego
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MachinesDataAnalyzingSystem.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Ten etap jest używany w środowisku produkcyjnym lub w przypadku uruchamiania z programu VS w trybie regularnym (domyślnie, gdy nie jest używana konfiguracja debugowania)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MachinesDataAnalyzingSystem.WebAPI.dll"]