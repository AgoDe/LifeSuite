import axios, { AxiosRequestConfig, AxiosResponse } from 'axios'
import https from 'https'
import { logger } from '#services/logging_service'
import type { MicroserviceLogData } from '../types/logging.js'

interface MicroserviceCallConfig {
  url: string
  method?: 'get' | 'post' | 'put' | 'delete' | 'patch'
  data?: any
  userId: string | number
  params?: any
  headers?: Record<string, string>
  correlationId?: string
  timeout?: number
}

interface MicroserviceResponse<T = any> {
  data: T
  status: number
  headers: any
  duration: number
  correlationId: string
}

export class MicroserviceClient {
  private static instance: MicroserviceClient
  private axiosConfig: AxiosRequestConfig

  constructor() {
    // Configurazione base per tutti i microservizi
    this.axiosConfig = {
      timeout: 10000, // 10 secondi di timeout
      httpsAgent: new https.Agent({
        rejectUnauthorized: process.env.NODE_ENV === 'production'
      }),
      headers: {
        'Content-Type': 'application/json',
        'User-Agent': 'LifeSuite-Gateway/1.0'
      }
    }
  }

  static getInstance(): MicroserviceClient {
    if (!MicroserviceClient.instance) {
      MicroserviceClient.instance = new MicroserviceClient()
    }
    return MicroserviceClient.instance
  }

  /**
   * Chiama un microservizio con logging completo
   */
  async call<T = any>(config: MicroserviceCallConfig): Promise<MicroserviceResponse<T>> {
    const startTime = Date.now()
    const correlationId = config.correlationId || logger.generateCorrelationId()
    const microserviceName = this.extractMicroserviceName(config.url)

    // Prepara configurazione Axios
    const axiosConfig: AxiosRequestConfig = {
      ...this.axiosConfig,
      url: config.url,
      method: config.method || 'get',
      data: config.data,
      params: config.params,
      timeout: config.timeout || this.axiosConfig.timeout,
      headers: {
        ...this.axiosConfig.headers,
        ...config.headers,
        'x-user-id': config.userId.toString(),
        'x-correlation-id': correlationId,
        'x-gateway-timestamp': new Date().toISOString()
      }
    }

    // Log della richiesta
    const logData: Partial<MicroserviceLogData> = {
      microservice: microserviceName,
      endpoint: config.url,
      method: config.method || 'get',
      userId: config.userId,
      correlationId,
      requestData: this.sanitizeLogData(config.data)
    }

    try {
      logger.info(`🚀 Starting microservice call to ${microserviceName}`, 
        logger.createContext({
          correlationId,
          userId: config.userId.toString(),
          service: microserviceName,
          method: config.method || 'get',
          endpoint: config.url
        })
      )

      // Esegui la chiamata
      const response: AxiosResponse<T> = await axios(axiosConfig)
      const duration = Date.now() - startTime

      // Log del successo
      logger.logMicroserviceCall({
        ...logData as MicroserviceLogData,
        responseStatus: response.status,
        responseData: this.sanitizeLogData(response.data),
        duration
      })

      return {
        data: response.data,
        status: response.status,
        headers: response.headers,
        duration,
        correlationId
      }

    } catch (error: any) {
      const duration = Date.now() - startTime
      const status = error.response?.status || 0

      // Log dell'errore
      logger.logMicroserviceCall({
        ...logData as MicroserviceLogData,
        responseStatus: status,
        responseData: error.response?.data,
        duration,
        error: {
          message: error.message,
          code: error.code,
          response: error.response?.data
        }
      })

      // Rilancia l'errore con informazioni aggiuntive
      const enhancedError = new Error(`Microservice call failed: ${error.message}`)
      enhancedError.cause = error
      ;(enhancedError as any).correlationId = correlationId
      ;(enhancedError as any).microservice = microserviceName
      ;(enhancedError as any).duration = duration
      ;(enhancedError as any).status = status

      throw enhancedError
    }
  }

  /**
   * Estrae il nome del microservizio dall'URL
   */
  private extractMicroserviceName(url: string): string {
    try {
      const urlObj = new URL(url)
      const hostname = urlObj.hostname
      
      // Esempi: budget-manager.local -> BudgetManager
      if (hostname.includes('budget')) return 'BudgetManager'
      if (hostname.includes('meal')) return 'MealPlanner'
      if (hostname.includes('localhost')) {
        const port = urlObj.port
        if (port === '5070') return 'BudgetManager'
        if (port === '8001') return 'MealPlanner'
      }
      
      return hostname || 'Unknown'
    } catch {
      return 'Unknown'
    }
  }

  /**
   * Pulisce i dati sensibili dai log
   */
  private sanitizeLogData(data: any): any {
    if (!data) return data
    
    const sensitiveFields = ['password', 'token', 'secret', 'key', 'auth']
    
    if (typeof data === 'object') {
      const cleaned = { ...data }
      for (const field of sensitiveFields) {
        if (field in cleaned) {
          cleaned[field] = '[REDACTED]'
        }
      }
      return cleaned
    }
    
    return data
  }

  /**
   * Health check per un microservizio
   */
  async healthCheck(baseUrl: string, correlationId?: string): Promise<boolean> {
    try {
      const response = await this.call({
        url: `${baseUrl}/health`,
        method: 'get',
        userId: 'system',
        correlationId,
        timeout: 5000
      })
      
      return response.status === 200
    } catch {
      return false
    }
  }
}

// Export singleton e funzione di convenienza
export const microserviceClient = MicroserviceClient.getInstance()

export async function callMicroservice<T = any>(config: MicroserviceCallConfig): Promise<MicroserviceResponse<T>> {
  return microserviceClient.call<T>(config)
}