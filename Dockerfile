FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/ConfigurationReader.API/ConfigurationReader.API.csproj ./ConfigurationReader.API.csproj
RUN dotnet restore ./ConfigurationReader.API.csproj

COPY . .

RUN dotnet publish src/ConfigurationReader.API/ConfigurationReader.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5000

ENTRYPOINT ["dotnet", "ConfigurationReader.API.dll"]