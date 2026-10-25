FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
# The Logdash version to check, such as 0.1.0; the latest by default.
ARG LOGDASH_VERSION=*
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore "-p:LogdashVersion=$LOGDASH_VERSION"

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out "-p:LogdashVersion=$LOGDASH_VERSION"

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app
COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "check-deployed-package.dll"] 