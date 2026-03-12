# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy everything into the container
COPY . .

# Restore dependencies for the API project (this will also restore referenced projects)
RUN dotnet restore "OrderManagement.API/OrderManagement.API.csproj"

# Build
RUN dotnet build "OrderManagement.API/OrderManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
RUN dotnet publish "OrderManagement.API/OrderManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OrderManagement.API.dll"]