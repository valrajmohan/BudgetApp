# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore
COPY MyBudgetTracker/*.csproj ./MyBudgetTracker/
RUN dotnet restore ./MyBudgetTracker/MyBudgetTracker.csproj

# Copy rest of the app
COPY . ./

WORKDIR /app/MyBudgetTracker
RUN dotnet publish -c Release -o /app/out

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "MyBudgetTracker.dll"]