export default defineNuxtRouteMiddleware(async (to, from) => {
  const { isLoggedIn } = useAuth()
  
  // Controllo semplice - se non c'è utente nello stato, reindirizza
  if (!isLoggedIn.value) {
    return navigateTo('/auth/login')
  }
})