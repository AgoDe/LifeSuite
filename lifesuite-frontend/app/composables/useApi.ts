export const useApi = () => {
  const config = useRuntimeConfig()
  const baseURL = config.public.apiBaseUrl
  
  const apiCall = async <T>(
    endpoint: string, 
    options: any = {}
  ): Promise<T> => {
    try {
      const response = await $fetch<T>(`${baseURL}/api/${endpoint}`, {
        ...options,
        credentials: 'include', // Include automaticamente i cookie
        headers: {
          'Content-Type': 'application/json',
          ...options.headers,
        },
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