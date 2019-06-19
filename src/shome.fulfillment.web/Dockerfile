FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["src/shome.fulfillment.web/shome.fulfillment.web.csproj", "shome.fulfillment.web/"]
RUN dotnet restore "shome.fulfillment.web/shome.fulfillment.web.csproj"
COPY . .
WORKDIR "/src/src/shome.fulfillment.web"
RUN dotnet build "shome.fulfillment.web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "shome.fulfillment.web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "shome.fulfillment.web.dll"]