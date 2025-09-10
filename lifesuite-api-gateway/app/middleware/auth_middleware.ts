import type { HttpContext } from '@adonisjs/core/http'
import type { NextFn } from '@adonisjs/core/types/http'
import { logger } from '#services/logging_service'

export default class AuthMiddleware {
  async handle({ auth, response, request }: HttpContext, next: NextFn) {
    const correlationId = request.correlationId || logger.generateCorrelationId()
    
    const context = logger.createContext({
      correlationId,
      method: request.method(),
      endpoint: request.url(),
      userAgent: request.header('user-agent'),
      ip: request.ip()
    })

    try {
      // Tenta l'autenticazione
      await auth.authenticate()
      
      const userId = auth.user?.id
      
      // Log successo autenticazione
      logger.logAuth('login', {
        ...context,
        userId: userId?.toString()
      }, {
        userEmail: auth.user?.email,
        loginMethod: 'jwt'
      })

      await next()
      
    } catch (error: any) {
      // Log tentativo non autorizzato
      logger.logAuth('unauthorized', context, {
        authHeader: !!request.header('authorization'),
        cookies: !!request.cookie('auth_token'),
        error: error.message
      })

      return response.status(401).json({
        success: false,
        message: 'Unauthorized',
        correlationId,
        timestamp: new Date().toISOString()
      })
    }
  }
}