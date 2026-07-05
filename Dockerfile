# Zobacz https://aka.ms/customizecontainer aby dowiedzieć się, jak dostosować kontener debugowania i jak program Visual Studio używa tego pliku Dockerfile do kompilowania obrazów w celu szybszego debugowania.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Boksi.Api/Boksi.Api.csproj", "Boksi.Api/"]
COPY ["Boksi.Application/Boksi.Application.csproj", "Boksi.Application/"]
COPY ["Boksi.Domain/Boksi.Domain.csproj", "Boksi.Domain/"]
COPY ["Boksi.Infrastructure/Boksi.Infrastructure.csproj", "Boksi.Infrastructure/"]
RUN dotnet restore "./Boksi.Api/Boksi.Api.csproj"
COPY . .
WORKDIR "/src/Boksi.Api"
RUN dotnet build "./Boksi.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Boksi.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Boksi.Api.dll"]
