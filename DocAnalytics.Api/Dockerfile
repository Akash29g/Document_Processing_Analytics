# ---- build stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# copy csproj files first for layer-cached restore
COPY DocAnalytics.Api/DocAnalytics.Api.csproj          DocAnalytics.Api/
COPY DocAnalytics.Service/DocAnalytics.Service.csproj  DocAnalytics.Service/
COPY DocAnalytics.Data/DocAnalytics.Data.csproj        DocAnalytics.Data/
COPY DocAnalytics.Domain/DocAnalytics.Domain.csproj    DocAnalytics.Domain/
RUN dotnet restore DocAnalytics.Api/DocAnalytics.Api.csproj

# copy the rest and publish
COPY DocAnalytics.Api/     DocAnalytics.Api/
COPY DocAnalytics.Service/ DocAnalytics.Service/
COPY DocAnalytics.Data/    DocAnalytics.Data/
COPY DocAnalytics.Domain/  DocAnalytics.Domain/
RUN dotnet publish DocAnalytics.Api/DocAnalytics.Api.csproj -c Release -o /app --no-restore

# ---- runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0
RUN apt-get update && apt-get upgrade -y && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "DocAnalytics.Api.dll"]
