echo off
echo setup all containers
docker-compose up -d

:loop
for /f %%i in ('docker ps -qf "name=^payment-webapi"') do set containerId=%%i
echo %containerId%
If "%containerId%" == "" (
  echo "No Container running"
  timeout /t 5
  goto loop
)

echo run datagenerator
dotnet run ./eCommerceDataGenerator/eCommerceDataGenerator.csproj

echo Starting test
echo Payments request status:
curl -s -o /dev/null -w "%{http_code}" http://localhost:8080/api/payments

echo running webhook

echo stop containers
docker-compose down