import type { HttpContext } from '@adonisjs/core/http'
import User from '#models/user'

export default class AuthController {
  async register({ request, response }: HttpContext) {
    const { email, password, fullName } = request.only(['email', 'password', 'fullName'])

    // Controlla se l'email è già registrata
    const exists = await User.findBy('email', email)
    if (exists) {
      return response.status(409).json({ message: 'Email già registrata' })
    }

    // Crea nuovo utente
    const user = await User.create({ email, password, fullName })

    return response.status(201).json({
      id: user.id,
      email: user.email,
      fullName: user.fullName
    })
  }
  async login({ request, response, auth }: HttpContext) {
    const { email, password } = request.only(['email', 'password'])

    try {
      // Verifica credenziali
      const user = await User.verifyCredentials(email, password)
      
      // Genera token (salva in database automaticamente)
      const token = await User.accessTokens.create(user, ['*'], {
        expiresIn: '7 days'
      })
      
      // Imposta cookie HttpOnly
      response.cookie('auth_token', token.value!.release(), {
        httpOnly: true,
        secure: process.env.NODE_ENV === 'production',
        sameSite: 'strict',
        maxAge: 7 * 24 * 60 * 60 * 1000 // 7 giorni
      })

      return response.json({
        user: {
          id: user.id,
          email: user.email,
          fullName: user.fullName
        },
        token: token.value!.release() // Per riferimento
      })
    } catch (error) {
      return response.status(400).json({
        message: 'Invalid credentials'
      })
    }
  }

  async logout({ response, auth }: HttpContext) {
    // Revoca il token dal database
    await User.accessTokens.delete(auth.user!, auth.user!.currentAccessToken.identifier)
    
    // Cancella cookie
    response.clearCookie('auth_token')
    
    return response.json({
      message: 'Logged out successfully'
    })
  }

  async me({ auth, response }: HttpContext) {
    const user = auth.user!
    return response.json({
      id: user.id,
      email: user.email,
      fullName: user.fullName
    })
  }
}