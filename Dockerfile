FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy solution and csproj files for layer caching
COPY *.sln .
COPY src/Konekto.API/*.csproj src/Konekto.API/
COPY src/Konekto.Business/*.csproj src/Konekto.Business/
COPY src/Konekto.DataAccess/*.csproj src/Konekto.DataAccess/
COPY src/Konekto.Domain/*.csproj src/Konekto.Domain/
RUN dotnet restore

# Copy all source files and publish
COPY src/ src/
RUN dotnet publish src/Konekto.API/Konekto.API.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Konekto.API.dll"]
