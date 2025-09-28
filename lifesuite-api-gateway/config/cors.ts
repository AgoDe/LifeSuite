import { defineConfig } from '@adonisjs/cors'
import env from '#start/env'

/**
 * Configuration options to tweak the CORS policy. The following
 * options are documented on the official documentation website.
 *
 * https://docs.adonisjs.com/guides/security/cors
 */
const corsConfig = defineConfig({
  enabled: true,
  origin: (origin) => {
    // Lista delle origin consentite
    const allowedOrigins = [
      'http://localhost:3000', // Nuxt dev
      'http://127.0.0.1:3000', // Nuxt dev alternativo
      'http://localhost:5173', // Vite dev (se userai)
      'http://localhost:3333', // Allow same origin requests
      'http://127.0.0.1:3333', // Allow same origin requests
      // Aggiungerai domini di produzione qui
    ]

    // In sviluppo, permetti tutte le origin per debug
    if (env.get('NODE_ENV') === 'development') {
      return true
    }

    if (origin && allowedOrigins.includes(origin)) {
      return true
    }
    return false
  },
  methods: ['GET', 'HEAD', 'POST', 'PUT', 'DELETE'],
  headers: true,
  exposeHeaders: [
    'X-Total-Count',        // Per paginazione
    'X-RateLimit-Limit',    // Per rate limiting futuro
    'X-RateLimit-Remaining'
  ],
  credentials: true,
  maxAge: 86400, // 24 ore
})

export default corsConfig
