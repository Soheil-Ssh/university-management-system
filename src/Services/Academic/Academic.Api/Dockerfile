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
COPY ["src/Services/Academic/Academic.Api/Academic.Api.csproj", "src/Services/Academic/Academic.Api/"]
COPY ["src/SharedKernel/SharedKernel.Api/SharedKernel.Api.csproj", "src/SharedKernel/SharedKernel.Api/"]
COPY ["src/SharedKernel/SharedKernel.Domain/SharedKernel.Domain.csproj", "src/SharedKernel/SharedKernel.Domain/"]
COPY ["src/SharedKernel/SharedKernel.Identity/SharedKernel.Identity.csproj", "src/SharedKernel/SharedKernel.Identity/"]
COPY ["src/SharedKernel/SharedKernel.Observability/SharedKernel.Observability.csproj", "src/SharedKernel/SharedKernel.Observability/"]
COPY ["src/Services/Academic/Academic.Application/Academic.Application.csproj", "src/Services/Academic/Academic.Application/"]
COPY ["src/SharedKernel/SharedKernel.Abstractions/SharedKernel.Abstractions.csproj", "src/SharedKernel/SharedKernel.Abstractions/"]
COPY ["src/Services/Academic/Academic.Domain/Academic.Domain.csproj", "src/Services/Academic/Academic.Domain/"]
COPY ["src/Services/Academic/Academic.Infrastructure/Academic.Infrastructure.csproj", "src/Services/Academic/Academic.Infrastructure/"]
COPY ["src/SharedKernel/SharedKernel.Persistence/SharedKernel.Persistence.csproj", "src/SharedKernel/SharedKernel.Persistence/"]
RUN dotnet restore "./src/Services/Academic/Academic.Api/Academic.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/Academic/Academic.Api"
RUN dotnet build "./Academic.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Academic.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Academic.Api.dll"]