FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
RUN apt update && apt install -y net-tools curl
ENV ASPNETCORE_URLS=http://+:80
USER $APP_UID
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AuthService/AuthService.csproj", "AuthService/"]
RUN dotnet restore "AuthService/AuthService.csproj"
COPY . .
WORKDIR "/src/AuthService"
RUN dotnet build "AuthService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AuthService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthService.dll"]
