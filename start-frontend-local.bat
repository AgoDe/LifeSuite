@echo off
echo 🎨 Avviando frontend Nuxt.js in locale...
echo.

cd lifesuite-frontend

echo 📦 Verificando dipendenze...
if not exist "node_modules" (
    echo 📥 Installando dipendenze...
    npm install
)

echo 🚀 Avviando server di sviluppo...
npm run dev