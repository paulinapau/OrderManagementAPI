# Use ASP.NET runtime image for production
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# Use .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["OrderManagement.API.csproj", "./"]
RUN dotnet restore "OrderManagement.API.csproj"

# Copy the rest of the project files
COPY . . 

# Build the project
RUN dotnet build "OrderManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the project
FROM build AS publish
RUN dotnet publish "OrderManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderManagementAPI.dll"]