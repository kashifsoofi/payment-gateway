FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /build

# copy csproj and restore as distinct layers
COPY *.sln ./

# Copy the main source project files
COPY src/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done

# Restore to cache the layers
RUN dotnet restore ./src/AcquiringBank.Api/AcquiringBank.Api.csproj

# Copy all the source code and build
COPY ./src ./src

RUN dotnet build ./src/AcquiringBank.Api/AcquiringBank.Api.csproj -c Release --no-restore

FROM build AS publish
RUN dotnet publish ./src/AcquiringBank.Api/AcquiringBank.Api.csproj -c Release -o /app --no-restore --no-build

# App image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS runtime
WORKDIR /app
COPY --from=publish /app ./
ENTRYPOINT ["dotnet", "AcquiringBank.Api.dll"]