# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["MovieReview.sln", "./"]
COPY ["MovieReview/MovieReview.csproj", "MovieReview/"]
COPY ["MovieReview.Tests/MovieReview.Tests.csproj", "MovieReview.Tests/"]

# Restore dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Build and publish the application
WORKDIR "/src/MovieReview"
RUN dotnet publish "MovieReview.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Copy the published app from the build stage
COPY --from=build /app/publish .

# Entry point
ENTRYPOINT ["dotnet", "MovieReview.dll"]