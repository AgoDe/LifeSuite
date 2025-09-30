export const useApi = () => {
  const config = useRuntimeConfig()
  const baseURL = config.public.apiBaseUrl
  
  const apiCall = async <T>(
    endpoint: string, 
    options: any = {}
  ): Promise<T> => {
    try {
      const fullUrl = `${baseURL}/api/${endpoint}`
      console.log('🚀 API Call:', fullUrl)
      
      // Prepara gli headers di base
      const headers: Record<string, string> = {
        'Content-Type': 'application/json',
        ...options.headers,
      }
      
      // Se abbiamo un token salvato, aggiungilo agli headers
      if (process.client) {
        const token = localStorage.getItem('auth_token')
        if (token) {
          headers['Authorization'] = `Bearer ${token}`
        }
      }
      
      const response = await $fetch<T>(fullUrl, {
        ...options,
        //credentials: 'include', // Include automaticamente i cookie
        headers,
      })
      return response
    } catch (error) {
      console.error('API Error:', error)
      throw error
    }
  }

  return {
    apiCall,
    baseURL
  }
}