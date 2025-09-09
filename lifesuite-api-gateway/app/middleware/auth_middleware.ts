import type { HttpContext } from '@adonisjs/core/http'
import type { NextFn } from '@adonisjs/core/types/http'

export default class AuthMiddleware {
  async handle({ auth, response }: HttpContext, next: NextFn) {
    try {
      // AdonisJS gestisce automaticamente sia Authorization header che cookie
      await auth.authenticate()
      await next()
    } catch (error) {
      return response.status(401).json({
        message: 'Unauthorized'
      })
    }
  }
}