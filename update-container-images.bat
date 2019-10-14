REM Builds new container images locally, and pushes them to the public repo

pushd src\Applications\GamingWebApp
dotnet build -c Debug GamingWebApp.csproj
dotnet publish -o "bin/Debug/netcoreapp2.2/publish"

docker build -f Dockerfile.local -t gamingwebapp:latest -t xpiritbv/gamingwebapp:latest .
popd


pushd src\Services\Leaderboard.WebAPI
dotnet build -c Debug Leaderboard.WebAPI.csproj
dotnet publish -o "bin/Debug/netcoreapp2.2/publish"

docker build -f Dockerfile.local -t leaderboard.webapi:latest -t xpiritbv/leaderboard.webapi:latest .
popd

docker push xpiritbv/gamingwebapp:latest
docker push xpiritbv/leaderboard.webapi:latest