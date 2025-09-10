import type { HttpContext } from '@adonisjs/core/http'
import { logger } from '#services/logging_service'
import type { ErrorLogData } from '../types/logging.js'

export default class ErrorHandlerMiddleware {
  async handle({ request, response, auth }: HttpContext, next: () => Promise<void>) {
    try {
      await next()
    } catch (error: any) {
      const correlationId = request.correlationId || logger.generateCorrelationId()
      const userId = auth.user?.id

      // Context per il log dell'errore
      const context = logger.createContext({
        correlationId,
        userId: userId?.toString(),
        method: request.method(),
        endpoint: request.url(),
        userAgent: request.header('user-agent'),
        ip: request.ip()
      })

      // Determina status code
      const statusCode = this.getStatusCode(error)

      // Log strutturato dell'errore
      const errorLogData: ErrorLogData = {
        error,
        context,
        stack: error.stack,
        statusCode,
        additionalData: {
          requestBody: this.sanitizeRequestBody(request.body()),
          params: request.params(),
          query: request.qs(),
          microservice: (error as any).microservice,
          duration: (error as any).duration
        }
      }

      logger.logError(errorLogData)

      // Risposta strutturata per il client
      const errorResponse = this.buildErrorResponse(error, correlationId, statusCode)
      
      response.status(statusCode).json(errorResponse)
    }
  }

  /**
   * Determina lo status code dall'errore
   */
  private getStatusCode(error: any): number {
    if (error.status) return error.status
    if (error.code === 'E_VALIDATION_FAILURE') return 422
    if (error.code === 'E_UNAUTHORIZED_ACCESS') return 401
    if (error.code === 'E_ROUTE_NOT_FOUND') return 404
    if (error.message?.includes('404')) return 404
    if (error.message?.includes('401')) return 401
    if (error.message?.includes('403')) return 403
    return 500
  }

  /**
   * Costruisce la risposta di errore standardizzata
   */
  private buildErrorResponse(error: any, correlationId: string, statusCode: number) {
    const isDevelopment = process.env.NODE_ENV === 'development'

    const baseResponse = {
      success: false,
      message: this.getSafeErrorMessage(error, statusCode),
      correlationId,
      timestamp: new Date().toISOString()
    }

    if (isDevelopment) {
      return {
        ...baseResponse,
        error: {
          name: error.name,
          message: error.message,
          code: error.code,
          stack: error.stack,
          microservice: (error as any).microservice,
          duration: (error as any).duration
        }
      }
    }

    return baseResponse
  }

  /**
   * Messaggio di errore sicuro per produzione
   */
  private getSafeErrorMessage(error: any, statusCode: number): string {
    const isDevelopment = process.env.NODE_ENV === 'development'
    
    if (isDevelopment) {
      return error.message || 'An error occurred'
    }

    // Messaggi generici per produzione
    switch (statusCode) {
      case 400: return 'Bad Request'
      case 401: return 'Unauthorized'
      case 403: return 'Forbidden'
      case 404: return 'Resource not found'
      case 422: return 'Validation failed'
      case 500: return 'Internal server error'
      default: return 'An error occurred'
    }
  }

  /**
   * Rimuove dati sensibili dal body della richiesta
   */
  private sanitizeRequestBody(body: any): any {
    if (!body || typeof body !== 'object') return body

    const sensitiveFields = ['password', 'token', 'secret', 'key', 'auth', 'authorization']
    const sanitized = { ...body }

    for (const field of sensitiveFields) {
      if (field in sanitized) {
        sanitized[field] = '[REDACTED]'
      }
    }

    return sanitized
  }
}