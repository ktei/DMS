  
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as builder

COPY . /app
WORKDIR /app
RUN dotnet publish src/PingAI.DialogManagementService.Api -c Release

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

WORKDIR /app
COPY --from=builder /app/src/PingAI.DialogManagementService.Api/bin/Release/netcoreapp3.1/publish/ /app
EXPOSE 5000
ENTRYPOINT ["dotnet", "PingAI.DialogManagementService.Api.dll"]
