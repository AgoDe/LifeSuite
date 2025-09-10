import type { HttpContext } from '@adonisjs/core/http'
import { callMicroservice } from '#services/microservice_client'
import { logger } from '#services/logging_service'

export default class AccountsController {


  public async index({ auth, request, response }: HttpContext) {
    const userId = auth.user?.id
    const correlationId = request.correlationId
    
    const context = logger.createContext({
      correlationId,
      userId: userId?.toString(),
      service: 'BudgetManager',
      method: 'GET',
      endpoint: '/accounts'
    })

    try {
      logger.info('🏦 Fetching user accounts from BudgetManager', context)

      const result = await callMicroservice({
        url: `${process.env.BUDGET_MANAGER_URL}/accounts`, // Nota: aggiunto /api/
        method: 'get',
        userId: userId!,
        params: request.qs(),
        correlationId
      })

      logger.info('✅ Accounts retrieved successfully', context, {
        accountCount: Array.isArray(result.data) ? result.data.length : 'unknown',
        duration: `${result.duration}ms`
      })

      return response.json({
        success: true,
        data: result.data,
        meta: {
          correlationId,
          timestamp: new Date().toISOString(),
          duration: result.duration
        }
      })

    } catch (error: any) {
      logger.error('❌ Failed to fetch accounts', context, {
        errorMessage: error.message,
        microservice: error.microservice,
        statusCode: error.status,
        duration: error.duration
      })

      // L'ErrorHandlerMiddleware gestirà il resto
      throw error
    }
  }

  public async show({ auth, request, response, params }: HttpContext) {
    const userId = auth.user?.id
    const correlationId = request.correlationId
    const accountId = params.id
    
    const context = logger.createContext({
      correlationId,
      userId: userId?.toString(),
      service: 'BudgetManager',
      method: 'GET',
      endpoint: `/accounts/${accountId}`
    })

    try {
      logger.info(`🏦 Fetching account ${accountId} from BudgetManager`, context)

      const result = await callMicroservice({
        url: `${process.env.BUDGET_MANAGER_URL}/accounts/${accountId}`,
        method: 'get',
        userId: userId!,
        correlationId
      })

      return response.json({
        success: true,
        data: result.data,
        meta: {
          correlationId,
          timestamp: new Date().toISOString(),
          duration: result.duration
        }
      })

    } catch (error: any) {
      logger.error(`❌ Failed to fetch account ${accountId}`, context, {
        errorMessage: error.message,
        accountId
      })

      throw error
    }
  }
}