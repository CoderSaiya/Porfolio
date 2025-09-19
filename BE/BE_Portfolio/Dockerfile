FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8082

# Tạo và copy chứng chỉ trước khi đổi USER
RUN mkdir -p /app/Certificates
COPY ./BE_Portfolio/Certificates/aspnetapp.pfx /app/Certificates/aspnetapp.pfx
# Cho phép user khác đọc
RUN chmod 644 /app/Certificates/aspnetapp.pfx

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BE_Portfolio/BE_Portfolio.csproj", "BE_Portfolio/"]
RUN dotnet restore "BE_Portfolio/BE_Portfolio.csproj"
COPY . .
WORKDIR "/src/BE_Portfolio"
RUN dotnet build "BE_Portfolio.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BE_Portfolio.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS="http://+:8082"
ENV Kestrel__Certificates__Default__Path=/app/Certificates/aspnetapp.pfx

USER $APP_UID
ENTRYPOINT ["dotnet", "BE_Portfolio.dll"]
