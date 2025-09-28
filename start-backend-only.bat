@echo off
echo 🚀 Avviando servizi backend (senza frontend)...
echo.

REM Ferma tutti i servizi
docker-compose down

REM Avvia solo i servizi backend
docker-compose up -d postgres redis api-gateway budget-manager pgadmin

echo.
echo ✅ Servizi backend avviati!
echo 📊 pgAdmin: http://localhost:8080 (admin@lifesuite.com / admin123)
echo 🔧 API Gateway: http://localhost:3333
echo 💰 Budget Manager: http://localhost:5001
echo.
echo 💡 Per avviare il frontend localmente:
echo    cd lifesuite-frontend
echo    npm install
echo    npm run dev
echo.
pause