import type { HttpContext } from '@adonisjs/core/http'

export default class TestController {
  // Health check pubblico
  async health({ response }: HttpContext) {
    return response.json({
      status: 'ok',
      service: 'LifeSuite API Gateway',
      version: '1.0.0',
      timestamp: new Date().toISOString(),
      cors: 'enabled'
    })
  }
  
  // Test CORS con preflight
  async corsTest({ request, response }: HttpContext) {
    return response.json({
      message: 'CORS funziona!',
      origin: request.header('origin'),
      userAgent: request.header('user-agent'),
      method: request.method(),
      headers: request.headers()
    })
  }
  
  // Test auth protetto
  async authTest({ auth, response }: HttpContext) {
    const user = auth.user!
    return response.json({
      message: 'Auth + CORS funzionano!',
      user: {
        id: user.id,
        email: user.email
      },
      timestamp: new Date().toISOString()
    })
  }
}