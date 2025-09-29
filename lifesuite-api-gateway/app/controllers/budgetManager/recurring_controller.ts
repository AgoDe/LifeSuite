import type { HttpContext } from '@adonisjs/core/http'
import { callMicroservice } from '#services/microservice_client'
import { logger } from '#services/logging_service'

export default class RecurringController {

  public async index({ auth, request, response }: HttpContext) {
    const userId = auth.user?.id
    const correlationId = request.correlationId
    
    const context = logger.createContext({
      correlationId,
      userId: userId?.toString(),
      service: 'BudgetManager',
      method: 'GET',
      endpoint: '/recurring'
    })

    try {
      logger.info('🔄 Fetching recurring transactions from BudgetManager', context)

      const result = await callMicroservice({
        url: `${process.env.BUDGET_MANAGER_URL}/recurring`,
        method: 'get',
        userId: userId!,
        params: request.qs(),
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
      logger.error('❌ Failed to fetch recurring transactions', context, {
        errorMessage: error.message
      })

      throw error
    }
  }

  public async show({ auth, request, response, params }: HttpContext) {
    const userId = auth.user?.id
    const correlationId = request.correlationId
    const recurringId = params.id
    
    const context = logger.createContext({
      correlationId,
      userId: userId?.toString(),
      service: 'BudgetManager',
      method: 'GET',
      endpoint: `/recurring/${recurringId}`
    })

    try {
      logger.info(`🔄 Fetching recurring transaction ${recurringId} from BudgetManager`, context)

      const result = await callMicroservice({
        url: `${process.env.BUDGET_MANAGER_URL}/recurring/${recurringId}`,
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
      logger.error(`❌ Failed to fetch recurring transaction ${recurringId}`, context, {
        errorMessage: error.message,
        recurringId
      })

      throw error
    }
  }

  public async store({ auth, request, response }: HttpContext) {
    const userId = auth.user?.id
    const correlationId = request.correlationId
    
    const context = logger.createContext({
      correlationId,
      userId: userId?.toString(),
      service: 'BudgetManager',
      method: 'POST',
      endpoint: '/recurring'
    })

    try {
      logger.info('🔄 Creating new recurring transaction in BudgetManager', context)

      const result = await callMicroservice({
        url: `${process.env.BUDGET_MANAGER_URL}/recurring`,
        method: 'post',
        userId: userId!,
        data: request.body(),
        correlationId
      })

      return response.status(201).json({
        success: true,
        data: result.data,
        meta: {
          correlationId,
          timestamp: new Date().toISOString(),
          duration: result.duration
        }
      })

    } catch (error: any) {
      logger.error('❌ Failed to create recurring transaction', context, {
        errorMessage: error.message
      })

      throw error
    }
  }

  public async update({ auth, request, response, params }: HttpContext) {
    const userId = auth.user?.id
    const correlationId = request.correlationId
    const recurringId = params.id
    
    const context = logger.createContext({
      correlationId,
      userId: userId?.toString(),
      service: 'BudgetManager',
      method: 'PUT',
      endpoint: `/recurring/${recurringId}`
    })

    try {
      logger.info(`🔄 Updating recurring transaction ${recurringId} in BudgetManager`, context)

      const result = await callMicroservice({
        url: `${process.env.BUDGET_MANAGER_URL}/recurring/${recurringId}`,
        method: 'put',
        userId: userId!,
        data: request.body(),
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
      logger.error(`❌ Failed to update recurring transaction ${recurringId}`, context, {
        errorMessage: error.message,
        recurringId
      })

      throw error
    }
  }

  public async destroy({ auth, request, response, params }: HttpContext) {
    const userId = auth.user?.id
    const correlationId = request.correlationId
    const recurringId = params.id
    
    const context = logger.createContext({
      correlationId,
      userId: userId?.toString(),
      service: 'BudgetManager',
      method: 'DELETE',
      endpoint: `/recurring/${recurringId}`
    })

    try {
      logger.info(`🔄 Deleting recurring transaction ${recurringId} from BudgetManager`, context)

      const result = await callMicroservice({
        url: `${process.env.BUDGET_MANAGER_URL}/recurring/${recurringId}`,
        method: 'delete',
        userId: userId!,
        correlationId
      })

      return response.status(204).json({
        success: true,
        meta: {
          correlationId,
          timestamp: new Date().toISOString(),
          duration: result.duration
        }
      })

    } catch (error: any) {
      logger.error(`❌ Failed to delete recurring transaction ${recurringId}`, context, {
        errorMessage: error.message,
        recurringId
      })

      throw error
    }
  }
}