FROM mcr.microsoft.com/dotnet/aspnet:11.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:11.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AppProject.API/AppProject.API.csproj", "AppProject.API/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Core/Core.csproj", "Core/"]
RUN dotnet restore "AppProject.API/AppProject.API.csproj"
COPY . .
WORKDIR "/src/AppProject.API"
RUN dotnet build "./AppProject.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AppProject.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .


# إنشاء مجلد الصور وإعطاؤه الصلاحيات الكاملة
USER root
RUN mkdir -p /app/wwwroot/Images && chmod -R 777 /app/wwwroot/Images

ENTRYPOINT ["dotnet", "AppProject.API.dll"]