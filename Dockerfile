# Stage 1: Build Angular Frontend
FROM node:22-alpine AS frontend-build
WORKDIR /app/frontend

# Copy package files and install dependencies
COPY src/ExpensesCalculator.UI/package*.json ./
RUN npm ci --legacy-peer-deps

# Copy Angular source files
COPY src/ExpensesCalculator.UI/ ./

# Build Angular in docker mode (browser only, no SSR)
RUN npm run build -- --configuration=docker

# Stage 2: Build .NET Backend
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS backend-build
WORKDIR /app/backend

# Copy csproj and restore dependencies
COPY src/ExpensesCalculator.WebAPI/*.csproj ./
RUN dotnet restore

# Copy backend source files
COPY src/ExpensesCalculator.WebAPI/ ./

# Copy Angular dist files to wwwroot
COPY --from=frontend-build /app/frontend/dist/expenses-calculator.ui/browser ./wwwroot/

# Copy index.csr.html to index.html for compatibility
RUN cp ./wwwroot/index.csr.html ./wwwroot/index.html

# Build backend in Release mode
RUN dotnet publish -c Release -o /app/publish

# Stage 3: Final Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published backend with embedded frontend
COPY --from=backend-build /app/publish ./

# Expose port 8080 (Azure App Service default)
EXPOSE 8080

# Configure ASP.NET Core to listen on port 8080
ENV ASPNETCORE_URLS=http://+:8080

# Start the application
ENTRYPOINT ["dotnet", "ExpensesCalculator.WebAPI.dll"]
