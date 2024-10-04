#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Vertr.QLib.ConsoleDemo/Vertr.QLib.ConsoleDemo.csproj", "src/Vertr.QLib.ConsoleDemo/"]
RUN dotnet restore "./src/Vertr.QLib.ConsoleDemo/Vertr.QLib.ConsoleDemo.csproj"
COPY . .
WORKDIR "/src/src/Vertr.QLib.ConsoleDemo"
RUN dotnet build "./Vertr.QLib.ConsoleDemo.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Vertr.QLib.ConsoleDemo.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Vertr.QLib.ConsoleDemo.dll"]