FROM mcr.microsoft.com/dotnet/runtime:5.0.101-alpine3.12-amd64 AS base
WORKDIR /app
EXPOSE 8100

RUN apk add --no-cache \
        bash \
		icu-libs 
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

FROM mcr.microsoft.com/dotnet/sdk:5.0.101-alpine3.12-amd64 AS build
WORKDIR /src
COPY [".", "AlertProxy/"]
RUN dotnet restore "AlertProxy/AlertProxy.csproj"
COPY . .
WORKDIR "/src/AlertProxy"
RUN dotnet build "AlertProxy.csproj" -c Release -o /app


WORKDIR "/src/AlertProxy"
FROM build AS publish
RUN dotnet publish "AlertProxy.csproj" -c Release -o /app


FROM base AS final
WORKDIR /app
COPY --from=publish /app .
RUN rm appsettings.json
WORKDIR /app

ENTRYPOINT ["dotnet", "AlertProxy.dll"]

