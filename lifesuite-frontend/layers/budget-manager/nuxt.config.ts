export default defineNuxtConfig({
  // Auto-import specifici per questo layer
  imports: {
    dirs: [
      'composables',
      'types'
    ]
  },

  // Componenti specifici del budget manager
  components: {
    dirs: [
      {
        path: '~/components/budget',
        prefix: 'Budget'
      }
    ]
  },

  // Configurazione runtime specifica per budget manager
  runtimeConfig: {
    public: {
      budgetManager: {
        enabled: true,
        features: {
          accounts: true,
          transactions: true,
          categories: true,
          recurring: true,
          reports: true
        },
        endpoints: {
          accounts: '/budget-manager/accounts',
          transactions: '/budget-manager/transactions',
          categories: '/budget-manager/categories',
          recurring: '/budget-manager/recurring'
        }
      }
    }
  },

  // CSS specifici per il budget manager
  css: [
    '~/assets/styles/budget-manager.scss'
  ],

  // TypeScript configuration per il layer
  typescript: {
    includeWorkspace: true
  }
})