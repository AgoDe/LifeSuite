export default defineNuxtRouteMiddleware((to, from) => {
  const { isLoggedIn } = useAuth()
  
  // Se l'utente non è autenticato, reindirizza al login
  if (!isLoggedIn.value) {
    return navigateTo('/auth/login')
  }
})
