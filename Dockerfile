# Stage 1: Build the solution
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution file and project files first for optimal caching
COPY ["IdPicture.sln", "./"]
COPY ["IdPicture/IdPicture.csproj", "IdPicture/"]
COPY ["MyId.Application/MyId.Application.csproj", "MyId.Application/"]
COPY ["MyId.Infrastructure/MyId.Infrastructure.csproj", "MyId.Infrastructure/"]
COPY ["MyId.Persistence/MyId.Persistence.csproj", "MyId.Persistence/"]

# Restore NuGet packages for the solution
RUN dotnet restore "IdPicture.sln"

# Copy the rest of the source code files
COPY . .

# Build and publish the main web project
WORKDIR "/src/IdPicture"
RUN dotnet publish "IdPicture.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published output from build stage
COPY --from=build /app/publish .

# Render requires web services to listen on port 10000
ENV PORT=10000
ENV ASPNETCORE_URLS=http://0.0.0.0:10000

ENTRYPOINT ["dotnet", "IdPicture.dll"]