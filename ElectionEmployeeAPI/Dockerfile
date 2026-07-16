FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY ElectionEmployeeAPI/*.csproj ./ElectionEmployeeAPI/
RUN dotnet restore ./ElectionEmployeeAPI/ElectionEmployeeAPI.csproj
COPY . .
RUN dotnet publish ./ElectionEmployeeAPI/ElectionEmployeeAPI.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .
RUN mkdir -p wwwroot/uploads/exemptions
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "ElectionEmployeeAPI.dll"]