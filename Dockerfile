# Use .NET 8 SDK to build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY . .

# Restore dependencies
RUN dotnet restore MyBudgetTracker.csproj

# Build and publish
RUN dotnet publish MyBudgetTracker.csproj -c Release -o /app

# Use .NET 8 runtime to run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app
COPY --from=build /app ./

ENTRYPOINT ["dotnet", "MyBudgetTracker.dll"]