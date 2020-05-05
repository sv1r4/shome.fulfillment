FROM mcr.microsoft.com/azure-functions/dotnet:3.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1.201-buster AS build
WORKDIR /src
COPY ["src/shome.fulfillment.azure.function/shome.fulfillment.azure.function.csproj", "src/shome.fulfillment.azure.function/"]
COPY ["src/shome.fulfillment.mqtt.mqttnet/shome.fulfillment.mqtt.mqttnet.csproj", "src/shome.fulfillment.mqtt.mqttnet/"]
COPY ["src/shome.fulfillment.mqtt/shome.fulfillment.mqtt.csproj", "src/shome.fulfillment.mqtt/"]
COPY ["src/shome.fulfillment.store.gcp.datastore/shome.fulfillment.store.gcp.datastore.csproj", "src/shome.fulfillment.store.gcp.datastore/"]
COPY ["src/shome.fulfillment.store/shome.fulfillment.store.csproj", "src/shome.fulfillment.store/"]
RUN dotnet restore "src/shome.fulfillment.azure.function/shome.fulfillment.azure.function.csproj"
COPY . .
WORKDIR "/src/src/shome.fulfillment.azure.function"
RUN dotnet build "shome.fulfillment.azure.function.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "shome.fulfillment.azure.function.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/app