export default defineNuxtRouteMiddleware((to, from) => {
  const { isLoggedIn } = useAuth()
  
  // If user is not logged in and trying to access protected route
  if (!isLoggedIn.value) {
    return navigateTo('/auth/login')
  }
})