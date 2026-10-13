# Stage 1: Build the solution
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all repository files into the container
COPY . .

# Restore NuGet packages using the solution file at the root
RUN dotnet restore "IdPicture.sln"

# Build and publish the web project from the root workdir
RUN dotnet publish "IdPicture/IdPicture.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/publish .

# Render requires web services to listen on port 10000
ENV PORT=10000
ENV ASPNETCORE_URLS=http://0.0.0.0:10000

ENTRYPOINT ["dotnet", "IdPicture.dll"]