# Use the official .NET 9 SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy the project files
COPY . ./

# Restore dependencies
RUN dotnet restore

# Build the application
RUN dotnet publish -c Release -o /out

# Use the official .NET 9 runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

# Set the working directory inside the container
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /out .

# Expose the port the application runs on
EXPOSE 80
EXPOSE 443

# Set the entry point for the container
ENTRYPOINT ["dotnet", "AIBridges.dll"]