# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Presentation/SilentMoon.WebApi/SilentMoon.WebApi.csproj", "src/Presentation/SilentMoon.WebApi/"]
COPY ["src/Core/SilentMoon.Application/SilentMoon.Application.csproj", "src/Core/SilentMoon.Application/"]
COPY ["src/Shared/SilentMoon.SharedKernel/SilentMoon.SharedKernel.csproj", "src/Shared/SilentMoon.SharedKernel/"]
COPY ["src/Core/SilentMoon.Domain/SilentMoon.Domain.csproj", "src/Core/SilentMoon.Domain/"]
COPY ["src/Infrastructure/SilentMoon.Infrastructure.Persistence/SilentMoon.Infrastructure.Persistence.csproj", "src/Infrastructure/SilentMoon.Infrastructure.Persistence/"]
RUN dotnet restore "./src/Presentation/SilentMoon.WebApi/SilentMoon.WebApi.csproj"
COPY . .
WORKDIR "/src/src/Presentation/SilentMoon.WebApi"
RUN dotnet build "./SilentMoon.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SilentMoon.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SilentMoon.WebApi.dll"]