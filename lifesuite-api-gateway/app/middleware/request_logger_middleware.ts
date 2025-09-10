import type { HttpContext } from '@adonisjs/core/http'
import type { NextFn } from '@adonisjs/core/types/http'
import { logger } from '#services/logging_service'
import type { RequestLogData } from '../types/logging.js'

export default class RequestLoggerMiddleware {
  async handle({ request, response, auth }: HttpContext, next: NextFn) {
    const startTime = Date.now()
    const correlationId = logger.generateCorrelationId()
    
    // Aggiungi correlation ID al request per uso downstream
    request.correlationId = correlationId
    
    // Log della richiesta in entrata
    const requestData: Partial<RequestLogData> = {
      method: request.method(),
      url: request.url(),
      correlationId,
      userAgent: request.header('user-agent'),
      ip: request.ip()
    }

    logger.info(`📨 Incoming request: ${request.method()} ${request.url()}`, 
      logger.createContext({
        correlationId,
        method: request.method(),
        endpoint: request.url(),
        userAgent: request.header('user-agent'),
        ip: request.ip()
      })
    )

    try {
      // Esegui la richiesta
      await next()
      
      const duration = Date.now() - startTime
      const userId = auth?.user?.id

      // Log della risposta
      logger.logRequest({
        ...requestData as RequestLogData,
        userId,
        duration,
        statusCode: response.getStatus()
      })

      // Log performance se lenta
      if (duration > 500) {
        logger.logPerformance(
          `${request.method()} ${request.url()}`,
          duration,
          logger.createContext({
            correlationId,
            userId: userId?.toString(),
            method: request.method(),
            endpoint: request.url()
          })
        )
      }

    } catch (error: any) {
      const duration = Date.now() - startTime
      const userId = auth?.user?.id

      // Log dell'errore
      logger.logRequest({
        ...requestData as RequestLogData,
        userId,
        duration,
        statusCode: error.status || 500,
        error: {
          message: error.message,
          stack: error.stack
        }
      })

      throw error // Rilancia per il middleware degli errori
    }
  }
}

// Estendi il tipo Request per includere correlationId
declare module '@adonisjs/core/http' {
  interface Request {
    correlationId: string
  }
}