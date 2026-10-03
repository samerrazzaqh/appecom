FROM mcr.microsoft.com/dotnet/sdk:11.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AppProject.Client/AppProject.Client.csproj", "AppProject.Client/"]
RUN dotnet restore "./AppProject.Client/AppProject.Client.csproj"
COPY . .
WORKDIR "/src/AppProject.Client"
RUN dotnet build "./AppProject.Client.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AppProject.Client.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
RUN sed -i 's/location \/ {/location \/ { try_files $uri $uri\/ \/index.html;/g' /etc/nginx/conf.d/default.conf