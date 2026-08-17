FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
USER root
RUN apk add --no-cache icu-libs icu-data-full
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/DotCruz.Garage.Api/DotCruz.Garage.Api.csproj", "src/DotCruz.Garage.Api/"]
COPY ["src/DotCruz.Garage.Application/DotCruz.Garage.Application.csproj", "src/DotCruz.Garage.Application/"]
COPY ["src/DotCruz.Garage.Domain/DotCruz.Garage.Domain.csproj", "src/DotCruz.Garage.Domain/"]
COPY ["src/DotCruz.Garage.Infrastructure/DotCruz.Garage.Infrastructure.csproj", "src/DotCruz.Garage.Infrastructure/"]
COPY ["src/DotCruz.Garage.CrossCutting/DotCruz.Garage.CrossCutting.csproj", "src/DotCruz.Garage.CrossCutting/"]
RUN dotnet restore "./src/DotCruz.Garage.Api/DotCruz.Garage.Api.csproj"
COPY . .
WORKDIR "/src/src/DotCruz.Garage.Api"
RUN dotnet build "./DotCruz.Garage.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DotCruz.Garage.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DotCruz.Garage.Api.dll"]
