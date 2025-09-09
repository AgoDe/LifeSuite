import type { User, LoginCredentials, AuthResponse } from '~/types/auth'

export const useAuth = () => {
  const { apiCall } = useApi()
  const router = useRouter()
  
  // Usa useState per mantenere lo stato tra le navigazioni
  const user = useState<User | null>('auth.user', () => null)
  const isLoggedIn = computed(() => !!user.value)
  
  // Login con tokensGuard
  const login = async (credentials: LoginCredentials) => {
    try {
      const response = await apiCall<AuthResponse>('auth/login', {
        method: 'POST',
        body: credentials,
        credentials: 'include'
      })
      
      // Gestisce diversi formati di risposta dal backend
      let userData = response.user
      if (!userData && (response as any).data?.user) {
        userData = (response as any).data.user
      }
      if (!userData && (response as any).id) {
        userData = response as any
      }
      
      if (!userData) {
        throw new Error('Formato risposta non valido')
      }
      
      // Il token verrà gestito automaticamente nei cookie HttpOnly
      user.value = userData
      
      // Aspetta un tick per assicurarsi che lo stato sia aggiornato
      await nextTick()
      
      await router.push('/dashboard')
      return response
    } catch (error: any) {
      console.error('Login error:', error)
      
      // Gestisci diversi tipi di errore dal backend
      if (error.status === 401) {
        throw new Error('Credenziali non valide')
      } else if (error.status === 422) {
        throw new Error('Dati non validi')
      } else if (error.status >= 500) {
        throw new Error('Errore del server. Riprova più tardi.')
      } else {
        throw new Error(error.data?.message || 'Errore durante il login')
      }
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
    } catch (error) {
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