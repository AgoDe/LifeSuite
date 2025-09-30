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
    logger.info(auth.user?.id.toString() || 'unknown', context)

    try {
      logger.info('🏦 Fetching user accounts from BudgetManager', context)
      console.log('DEBUG: BUDGET_MANAGER_URL =', process.env.BUDGET_MANAGER_URL)
      console.log('DEBUG: Full URL =', `${process.env.BUDGET_MANAGER_URL}/api/Account/user`)

      const result = await callMicroservice({
        url: `${process.env.BUDGET_MANAGER_URL}/api/Account/user`,
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

  public async store({ auth, request, response }: HttpContext) {
    const userId = auth.user?.id
    const correlationId = request.correlationId
    
    const context = logger.createContext({
      correlationId,
      userId: userId?.toString(),
      service: 'BudgetManager',
      method: 'POST',
      endpoint: '/accounts'
    })

    try {
      logger.info('🏦 Creating new account in BudgetManager', context)

      const result = await callMicroservice({
        url: `${process.env.BUDGET_MANAGER_URL}/accounts`,
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
      logger.error('❌ Failed to create account', context, {
        errorMessage: error.message
      })

      throw error
    }
  }

  public async update({ auth, request, response, params }: HttpContext) {
    const userId = auth.user?.id
    const correlationId = request.correlationId
    const accountId = params.id
    
    const context = logger.createContext({
      correlationId,
      userId: userId?.toString(),
      service: 'BudgetManager',
      method: 'PUT',
      endpoint: `/accounts/${accountId}`
    })

    try {
      logger.info(`🏦 Updating account ${accountId} in BudgetManager`, context)

      const result = await callMicroservice({
        url: `${process.env.BUDGET_MANAGER_URL}/accounts/${accountId}`,
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
      logger.error(`❌ Failed to update account ${accountId}`, context, {
        errorMessage: error.message,
        accountId
      })

      throw error
    }
  }

  public async destroy({ auth, request, response, params }: HttpContext) {
    const userId = auth.user?.id
    const correlationId = request.correlationId
    const accountId = params.id
    
    const context = logger.createContext({
      correlationId,
      userId: userId?.toString(),
      service: 'BudgetManager',
      method: 'DELETE',
      endpoint: `/accounts/${accountId}`
    })

    try {
      logger.info(`🏦 Deleting account ${accountId} from BudgetManager`, context)

      const result = await callMicroservice({
        url: `${process.env.BUDGET_MANAGER_URL}/accounts/${accountId}`,
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
      logger.error(`❌ Failed to delete account ${accountId}`, context, {
        errorMessage: error.message,
        accountId
      })

      throw error
    }
  }

  public async selectOptions({ auth, request, response }: HttpContext) {
    const userId = auth.user?.id
    const correlationId = request.correlationId
    
    const context = logger.createContext({
      correlationId,
      userId: userId?.toString(),
      service: 'BudgetManager',
      method: 'GET',
      endpoint: '/accounts/select-options'
    })

    try {
      logger.info('🏦 Fetching account select options from BudgetManager', context)

      const result = await callMicroservice({
        url: `${process.env.BUDGET_MANAGER_URL}/accounts/select-options`,
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
      logger.error('❌ Failed to fetch account select options', context, {
        errorMessage: error.message
      })

      throw error
    }
  }

  public async updateBalance({ auth, request, response, params }: HttpContext) {
    const userId = auth.user?.id
    const correlationId = request.correlationId
    const accountId = params.id
    
    const context = logger.createContext({
      correlationId,
      userId: userId?.toString(),
      service: 'BudgetManager',
      method: 'PATCH',
      endpoint: `/accounts/${accountId}/balance`
    })

    try {
      logger.info(`🏦 Updating balance for account ${accountId} in BudgetManager`, context)

      const result = await callMicroservice({
        url: `${process.env.BUDGET_MANAGER_URL}/accounts/${accountId}/balance`,
        method: 'patch',
        userId: userId!,
        data: request.body(),
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
      logger.error(`❌ Failed to update account balance ${accountId}`, context, {
        errorMessage: error.message,
        accountId
      })

      throw error
    }
  }
}