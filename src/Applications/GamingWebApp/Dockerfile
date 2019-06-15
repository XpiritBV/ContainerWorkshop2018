FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 14069
EXPOSE 44325

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY src/Applications/GamingWebApp/GamingWebApp.csproj src/Applications/GamingWebApp/
RUN dotnet restore src/Applications/GamingWebApp/GamingWebApp.csproj
COPY . .
WORKDIR /src/src/Applications/GamingWebApp
RUN dotnet build GamingWebApp.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish GamingWebApp.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "GamingWebApp.dll"]
