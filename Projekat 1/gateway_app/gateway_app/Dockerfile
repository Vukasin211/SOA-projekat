#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["gateway_app/gateway_app.csproj", "gateway_app/"]
RUN dotnet restore "gateway_app/gateway_app.csproj"
COPY . .
WORKDIR "/src/gateway_app"
RUN dotnet build "gateway_app.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "gateway_app.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "gateway_app.dll"]

EXPOSE 49155
EXPOSE 49153
EXPOSE 80
EXPOSE 443