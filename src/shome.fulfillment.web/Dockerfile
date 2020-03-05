FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1.102-alpine AS build
WORKDIR /src
COPY ["src/shome.fulfillment.web/shome.fulfillment.web.csproj", "shome.fulfillment.web/"]
COPY ["src/shome.fulfillment.mqtt/shome.fulfillment.mqtt.csproj", "shome.fulfillment.mqtt/"]
COPY ["src/shome.fulfillment.mqtt.mqttnet/shome.fulfillment.mqtt.mqttnet.csproj", "shome.fulfillment.mqtt.mqttnet/"]
COPY ["src/shome.fulfillment.store/shome.fulfillment.store.csproj", "shome.fulfillment.store/"]
COPY ["src/shome.fulfillment.store.gcp.datastore/shome.fulfillment.store.gcp.datastore.csproj", "shome.fulfillment.store.gcp.datastore/"]
RUN dotnet restore "shome.fulfillment.web/shome.fulfillment.web.csproj"
COPY . .
WORKDIR "/src/src/shome.fulfillment.web"
RUN dotnet publish "shome.fulfillment.web.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.1-buster-slim AS final
#RUN apk add --update --no-cache curl libc6-compat
WORKDIR /app
COPY --from=build /app .
ENV GOOGLE_APPLICATION_CREDENTIALS /app/secrets/service_key.json
CMD dotnet shome.fulfillment.web.dll