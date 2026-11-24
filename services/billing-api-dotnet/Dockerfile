# Billing API image (PoC testbed - not intended to be run)
FROM mcr.microsoft.com/dotnet/sdk:latest

WORKDIR /app
COPY src/ ./src/
RUN dotnet publish src/BillingApi/BillingApi.csproj -c Release -o /app/out

RUN useradd --create-home --shell /usr/sbin/nologin appuser
USER appuser

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "/app/out/BillingApi.dll"]
