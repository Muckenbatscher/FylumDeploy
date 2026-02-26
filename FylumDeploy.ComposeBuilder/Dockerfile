# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS base

# required installations for podman-out-of-podman
RUN apt-get update && \
    apt-get install -y git podman && \
    rm -rf /var/lib/apt/lists/*
RUN curl -sSL https://aspire.dev/install.sh | bash

WORKDIR /app

# This stage is used to build and publish the project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["FylumDeploy.ComposeBuilder/FylumDeploy.ComposeBuilder.csproj", "FylumDeploy.ComposeBuilder/"]
COPY ["FylumDeploy.RabbitMqShared/FylumDeploy.RabbitMqShared.csproj", "FylumDeploy.RabbitMqShared/"]
COPY ["FylumDeploy.ServiceDefaults/FylumDeploy.ServiceDefaults.csproj", "FylumDeploy.ServiceDefaults/"]
RUN dotnet restore "./FylumDeploy.ComposeBuilder/FylumDeploy.ComposeBuilder.csproj"
COPY . .
WORKDIR "/src/FylumDeploy.ComposeBuilder"
RUN dotnet publish "./FylumDeploy.ComposeBuilder.csproj" -c $BUILD_CONFIGURATION -o /app/publish

# This stage is used in when running the image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir -p /output
ENTRYPOINT ["dotnet", "FylumDeploy.ComposeBuilder.dll"]