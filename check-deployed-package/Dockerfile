FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build-env
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore ./Logdash.Check.csproj
RUN dotnet add package Logdash

COPY . .
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine
WORKDIR /home
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Logdash.Check.dll"]