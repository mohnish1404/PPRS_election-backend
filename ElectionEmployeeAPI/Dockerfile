# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and project files
COPY ElectionEmployeeAPI/ElectionEmployeeAPI/*.csproj ./ElectionEmployeeAPI/

# Restore dependencies
RUN dotnet restore ./ElectionEmployeeAPI/ElectionEmployeeAPI.csproj

# Copy all source code
COPY ElectionEmployeeAPI/. ./ElectionEmployeeAPI/

# Build and publish
RUN dotnet publish ./ElectionEmployeeAPI/ElectionEmployeeAPI.csproj -c Release -o /out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

# Create uploads folder
RUN mkdir -p wwwroot/uploads/exemptions

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "ElectionEmployeeAPI.dll"]