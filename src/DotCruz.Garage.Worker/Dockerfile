FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
USER root
RUN apk add --no-cache icu-libs icu-data-full
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/DotCruz.Garage.Worker/DotCruz.Garage.Worker.csproj", "src/DotCruz.Garage.Worker/"]
COPY ["src/DotCruz.Garage.Application/DotCruz.Garage.Application.csproj", "src/DotCruz.Garage.Application/"]
COPY ["src/DotCruz.Garage.Domain/DotCruz.Garage.Domain.csproj", "src/DotCruz.Garage.Domain/"]
COPY ["src/DotCruz.Garage.Infrastructure/DotCruz.Garage.Infrastructure.csproj", "src/DotCruz.Garage.Infrastructure/"]
COPY ["src/DotCruz.Garage.CrossCutting/DotCruz.Garage.CrossCutting.csproj", "src/DotCruz.Garage.CrossCutting/"]
RUN dotnet restore "./src/DotCruz.Garage.Worker/DotCruz.Garage.Worker.csproj"
COPY . .
WORKDIR "/src/src/DotCruz.Garage.Worker"
RUN dotnet build "./DotCruz.Garage.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DotCruz.Garage.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DotCruz.Garage.Worker.dll"]
