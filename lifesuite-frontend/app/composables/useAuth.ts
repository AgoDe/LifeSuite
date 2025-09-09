import type { User, LoginCredentials, AuthResponse } from '~/types/auth'

export const useAuth = () => {
  const { apiCall } = useApi()
  const router = useRouter()
  
  const user = ref<User | null>(null)
  const isLoggedIn = computed(() => !!user.value)
  
  // Login con tokensGuard
  const login = async (credentials: LoginCredentials) => {
    try {
      const response = await apiCall<AuthResponse>('auth/login', {
        method: 'POST',
        body: credentials,
        credentials: 'include'
      })
      
      // Il token verrà gestito automaticamente nei cookie HttpOnly
      user.value = response.user
      
      await router.push('/dashboard')
      return response
    } catch (error) {
      throw new Error('Login failed')
    }
  }
  
  // Il resto rimane uguale...
  const logout = async () => {
    try {
      await apiCall('auth/logout', {
        method: 'POST',
        credentials: 'include'
      })
    } catch (error) {
      console.warn('Logout API failed')
    } finally {
      user.value = null
      await router.push('/auth/login')
    }
  }
  
  const checkAuth = async () => {
    try {
      const userData = await apiCall<User>('auth/me', {
        credentials: 'include'
      })
      user.value = userData
      return true
    } catch {
      user.value = null
      return false
    }
  }
  
  return {
    user: readonly(user),
    isLoggedIn,
    login,
    logout,
    checkAuth
  }
}