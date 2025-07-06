FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TP4SCS.Solution/TP4SCS.API/TP4SCS.API.csproj", "TP4SCS.API/"]
COPY ["TP4SCS.Solution/TP4SCS.API/appsettings.json", "TP4SCS.API/"]
COPY ["TP4SCS.Solution/TP4SCS.Service/TP4SCS.Services.csproj", "TP4SCS.Service/"]
COPY ["TP4SCS.Solution/TP4SCS.Repositry/TP4SCS.Repository.csproj", "TP4SCS.Repositry/"]
COPY ["TP4SCS.Solution/TP4SCS.Library/TP4SCS.Library.csproj", "TP4SCS.Library/"]
COPY ["tp4scs-firebase.json", "./tp4scs-firebase.json"]
COPY ["WordBlacklist.json", "./WordBlacklist.json"]
RUN dotnet restore "TP4SCS.API/TP4SCS.API.csproj"

WORKDIR "/src/TP4SCS.API"
COPY . .

RUN dotnet build "./TP4SCS.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage để publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TP4SCS.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
# Stage cuối cùng cho image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TP4SCS.API.dll"]
