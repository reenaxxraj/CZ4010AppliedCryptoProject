FROM mcr.microsoft.com/dotnet/sdk:6.0 as builder
WORKDIR /app

COPY ./ApplicationServer/ApplicationServer.csproj ./Server/ApplicationServer.csproj
WORKDIR ./Server
RUN dotnet restore
COPY ./ApplicationServer .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0
ENV ASPNETCORE_ENVIRONMENT Production
ENV ASPNETCORE_URLS http://+
ENV ASPNETCORE_HTTP_PORT 80
WORKDIR /app
COPY --from=builder /app/Server/out ./Server/
WORKDIR ./Server
CMD dotnet ApplicationServer.dll
