FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 13995
EXPOSE 44369

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY src/Services/Leaderboard.WebAPI/Leaderboard.WebAPI.csproj src/Services/Leaderboard.WebAPI/
RUN dotnet restore src/Services/Leaderboard.WebAPI/Leaderboard.WebAPI.csproj
COPY . .
WORKDIR /src/src/Services/Leaderboard.WebAPI
RUN dotnet build Leaderboard.WebAPI.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Leaderboard.WebAPI.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Leaderboard.WebAPI.dll"]
