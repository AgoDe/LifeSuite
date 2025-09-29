import type { HttpContext } from '@adonisjs/core/http'
import { callMicroservice } from '#services/microservice_client'
import { logger } from '#services/logging_service'

export default class CategoriesController {

  public async index({ auth, request, response }: HttpContext) {
    const userId = auth.user?.id
    const correlationId = request.correlationId
    
    const context = logger.createContext({
      correlationId,
      userId: userId?.toString(),
      service: 'BudgetManager',
      method: 'GET',
      endpoint: '/categories'
    })

    try {
      logger.info('📂 Fetching categories from BudgetManager', context)

      const result = await callMicroservice({
        url: `${process.env.BUDGET_MANAGER_URL}/categories`,
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
      logger.error('❌ Failed to fetch categories', context, {
        errorMessage: error.message
      })

      throw error
    }
  }

  public async show({ auth, request, response, params }: HttpContext) {
    const userId = auth.user?.id
    const correlationId = request.correlationId
    const categoryId = params.id
    
    const context = logger.createContext({
      correlationId,
      userId: userId?.toString(),
      service: 'BudgetManager',
      method: 'GET',
      endpoint: `/categories/${categoryId}`
    })

    try {
      logger.info(`📂 Fetching category ${categoryId} from BudgetManager`, context)

      const result = await callMicroservice({
        url: `${process.env.BUDGET_MANAGER_URL}/categories/${categoryId}`,
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
      logger.error(`❌ Failed to fetch category ${categoryId}`, context, {
        errorMessage: error.message,
        categoryId
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
      endpoint: '/categories'
    })

    try {
      logger.info('📂 Creating new category in BudgetManager', context)

      const result = await callMicroservice({
        url: `${process.env.BUDGET_MANAGER_URL}/categories`,
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
      logger.error('❌ Failed to create category', context, {
        errorMessage: error.message
      })

      throw error
    }
  }

  public async update({ auth, request, response, params }: HttpContext) {
    const userId = auth.user?.id
    const correlationId = request.correlationId
    const categoryId = params.id
    
    const context = logger.createContext({
      correlationId,
      userId: userId?.toString(),
      service: 'BudgetManager',
      method: 'PUT',
      endpoint: `/categories/${categoryId}`
    })

    try {
      logger.info(`📂 Updating category ${categoryId} in BudgetManager`, context)

      const result = await callMicroservice({
        url: `${process.env.BUDGET_MANAGER_URL}/categories/${categoryId}`,
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
      logger.error(`❌ Failed to update category ${categoryId}`, context, {
        errorMessage: error.message,
        categoryId
      })

      throw error
    }
  }

  public async destroy({ auth, request, response, params }: HttpContext) {
    const userId = auth.user?.id
    const correlationId = request.correlationId
    const categoryId = params.id
    
    const context = logger.createContext({
      correlationId,
      userId: userId?.toString(),
      service: 'BudgetManager',
      method: 'DELETE',
      endpoint: `/categories/${categoryId}`
    })

    try {
      logger.info(`📂 Deleting category ${categoryId} from BudgetManager`, context)

      const result = await callMicroservice({
        url: `${process.env.BUDGET_MANAGER_URL}/categories/${categoryId}`,
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
      logger.error(`❌ Failed to delete category ${categoryId}`, context, {
        errorMessage: error.message,
        categoryId
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
      endpoint: '/categories/select-options'
    })

    try {
      logger.info('📂 Fetching category select options from BudgetManager', context)

      const result = await callMicroservice({
        url: `${process.env.BUDGET_MANAGER_URL}/categories/select-options`,
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
      logger.error('❌ Failed to fetch category select options', context, {
        errorMessage: error.message
      })

      throw error
    }
  }
}