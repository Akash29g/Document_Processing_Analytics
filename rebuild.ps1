docker compose up --build -d
Start-Sleep -Seconds 5
Start-Process "http://localhost:4200"
docker compose logs -f
