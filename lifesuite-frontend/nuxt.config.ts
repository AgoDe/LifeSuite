export default defineNuxtConfig({
  // Estende i layers
  extends: [
    './layers/budget-manager'
  ],

  // App metadata
  app: {
    head: {
      title: 'LifeSuite - Personal Life Management',
      meta: [
        { charset: 'utf-8' },
        { name: 'viewport', content: 'width=device-width, initial-scale=1' },
        { name: 'description', content: 'Piattaforma modulare per la gestione della vita quotidiana' }
      ],
      link: [
        { rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' }
      ]
    }
  },

  srcDir: 'app/',

  // Modalità SPA per iniziare (cambierai in SSR quando impari)
  ssr: false,

  // CSS globali
  css: [
    'vuetify/styles',
    '@mdi/font/css/materialdesignicons.css',
    '~/assets/styles/variables.scss',
    '~/assets/styles/main.css'
  ],

  // Build configuration per Vuetify
  build: {
    transpile: ['vuetify']
  },

  // Plugins
  plugins: [
    '~/plugins/vuetify.ts'
  ],

  // Moduli Nuxt (aggiungerai altri quando serviranno)
  modules: [
    // '@pinia/nuxt', // per state management
    // '@nuxtjs/tailwindcss', // se vuoi usare anche Tailwind
  ],

  // Runtime config per variabili ambiente
  runtimeConfig: {
    // Private keys (solo server-side)
    apiSecret: '',
    
    // Public keys (esposte al client)
    public: {
      apiBaseUrl: 'http://localhost:3333', // API Gateway AdonisJS
      apiVersion: 'v1',
      appName: 'LifeSuite',
      environment: 'development'
    }
  },

  // Development server
  devServer: {
    port: 3000,
    host: 'localhost'
  },

  // TypeScript configuration
  typescript: {
    strict: true,
    typeCheck: true
  },

  // Experimental features (per performance)
  experimental: {
    payloadExtraction: false
  },

  // Nitro configuration (server engine di Nuxt)
  nitro: {
    esbuild: {
      options: {
        target: 'node18'
      }
    }
  }
})