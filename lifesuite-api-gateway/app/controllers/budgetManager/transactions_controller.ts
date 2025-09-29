import type { HttpContext } from '@adonisjs/core/http'
import { callMicroservice } from '#services/microservice_client'
import { logger } from '#services/logging_service'

export default class TransactionsController {

  public async index({ auth, request, response }: HttpContext) {
    const userId = auth.user?.id
    const correlationId = request.correlationId
    
    const context = logger.createContext({
      correlationId,
      userId: userId?.toString(),
      service: 'BudgetManager',
      method: 'GET',
      endpoint: '/transactions'
    })

    try {
      logger.info('💰 Fetching transactions from BudgetManager', context)

      const result = await callMicroservice({
        url: `${process.env.BUDGET_MANAGER_URL}/transactions`,
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
      logger.error('❌ Failed to fetch transactions', context, {
        errorMessage: error.message
      })

      throw error
    }
  }

  public async show({ auth, request, response, params }: HttpContext) {
    const userId = auth.user?.id
    const correlationId = request.correlationId
    const transactionId = params.id
    
    const context = logger.createContext({
      correlationId,
      userId: userId?.toString(),
      service: 'BudgetManager',
      method: 'GET',
      endpoint: `/transactions/${transactionId}`
    })

    try {
      logger.info(`💰 Fetching transaction ${transactionId} from BudgetManager`, context)

      const result = await callMicroservice({
        url: `${process.env.BUDGET_MANAGER_URL}/transactions/${transactionId}`,
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
      logger.error(`❌ Failed to fetch transaction ${transactionId}`, context, {
        errorMessage: error.message,
        transactionId
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
      endpoint: '/transactions'
    })

    try {
      logger.info('💰 Creating new transaction in BudgetManager', context)

      const result = await callMicroservice({
        url: `${process.env.BUDGET_MANAGER_URL}/transactions`,
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
      logger.error('❌ Failed to create transaction', context, {
        errorMessage: error.message
      })

      throw error
    }
  }

  public async update({ auth, request, response, params }: HttpContext) {
    const userId = auth.user?.id
    const correlationId = request.correlationId
    const transactionId = params.id
    
    const context = logger.createContext({
      correlationId,
      userId: userId?.toString(),
      service: 'BudgetManager',
      method: 'PUT',
      endpoint: `/transactions/${transactionId}`
    })

    try {
      logger.info(`💰 Updating transaction ${transactionId} in BudgetManager`, context)

      const result = await callMicroservice({
        url: `${process.env.BUDGET_MANAGER_URL}/transactions/${transactionId}`,
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
      logger.error(`❌ Failed to update transaction ${transactionId}`, context, {
        errorMessage: error.message,
        transactionId
      })

      throw error
    }
  }

  public async destroy({ auth, request, response, params }: HttpContext) {
    const userId = auth.user?.id
    const correlationId = request.correlationId
    const transactionId = params.id
    
    const context = logger.createContext({
      correlationId,
      userId: userId?.toString(),
      service: 'BudgetManager',
      method: 'DELETE',
      endpoint: `/transactions/${transactionId}`
    })

    try {
      logger.info(`💰 Deleting transaction ${transactionId} from BudgetManager`, context)

      const result = await callMicroservice({
        url: `${process.env.BUDGET_MANAGER_URL}/transactions/${transactionId}`,
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
      logger.error(`❌ Failed to delete transaction ${transactionId}`, context, {
        errorMessage: error.message,
        transactionId
      })

      throw error
    }
  }

  public async withActiveRecurrings({ auth, request, response }: HttpContext) {
    const userId = auth.user?.id
    const correlationId = request.correlationId
    
    const context = logger.createContext({
      correlationId,
      userId: userId?.toString(),
      service: 'BudgetManager',
      method: 'GET',
      endpoint: '/transactions/with-active-recurrings'
    })

    try {
      logger.info('💰 Fetching transactions with active recurrings from BudgetManager', context)

      const result = await callMicroservice({
        url: `${process.env.BUDGET_MANAGER_URL}/transactions/with-active-recurrings`,
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
      logger.error('❌ Failed to fetch transactions with active recurrings', context, {
        errorMessage: error.message
      })

      throw error
    }
  }
}