export default defineNuxtPlugin(async () => {
  const { checkAuth } = useAuth()
  
  // Verifica l'autenticazione all'avvio dell'app (solo lato client)
  if (process.client) {
    try {
      await checkAuth()
    } catch (error) {
      console.warn('Controllo autenticazione fallito:', error)
    }
  }
})
