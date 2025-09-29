import router from '@adonisjs/core/services/router'
import { middleware } from './kernel.js'

// Test endpoints
router.get('/api/health', '#controllers/test_controller.health')
router.post('/api/test/cors', '#controllers/test_controller.corsTest')
router.get('/api/test/auth', '#controllers/test_controller.authTest').use(middleware.auth())

// Auth routes
router.group(() => {
  router.post('/register', '#controllers/auth_controller.register')
  router.post('/login', '#controllers/auth_controller.login')
  router.post('/logout', '#controllers/auth_controller.logout')
  router.get('/me', '#controllers/auth_controller.me').use(middleware.auth())
}).prefix('/api/auth')

//budget manager routes
router.group(() => {

    // Accounts routes
    router.group(() => {
        router.get('/', '#controllers/budgetManager/accounts_controller.index').use(middleware.auth())
        router.get('/:id', '#controllers/budgetManager/accounts_controller.show').use(middleware.auth())
        router.post('/', '#controllers/budgetManager/accounts_controller.store').use(middleware.auth())
        router.put('/:id', '#controllers/budgetManager/accounts_controller.update').use(middleware.auth())
        router.delete('/:id', '#controllers/budgetManager/accounts_controller.destroy').use(middleware.auth())
        router.get('/select-options', '#controllers/budgetManager/accounts_controller.selectOptions').use(middleware.auth())
        router.patch('/:id/balance', '#controllers/budgetManager/accounts_controller.updateBalance').use(middleware.auth())
    }).prefix('/accounts')

    // Categories routes
    router.group(() => {
        router.get('/', '#controllers/budgetManager/categories_controller.index').use(middleware.auth())
        router.get('/:id', '#controllers/budgetManager/categories_controller.show').use(middleware.auth())
        router.post('/', '#controllers/budgetManager/categories_controller.store').use(middleware.auth())
        router.put('/:id', '#controllers/budgetManager/categories_controller.update').use(middleware.auth())
        router.delete('/:id', '#controllers/budgetManager/categories_controller.destroy').use(middleware.auth())
        router.get('/select-options', '#controllers/budgetManager/categories_controller.selectOptions').use(middleware.auth())
    }).prefix('/categories')

    // Transactions routes
    router.group(() => {
        router.get('/', '#controllers/budgetManager/transactions_controller.index').use(middleware.auth())
        router.get('/:id', '#controllers/budgetManager/transactions_controller.show').use(middleware.auth())
        router.post('/', '#controllers/budgetManager/transactions_controller.store').use(middleware.auth())
        router.put('/:id', '#controllers/budgetManager/transactions_controller.update').use(middleware.auth())
        router.delete('/:id', '#controllers/budgetManager/transactions_controller.destroy').use(middleware.auth())
        router.get('/with-active-recurrings', '#controllers/budgetManager/transactions_controller.withActiveRecurrings').use(middleware.auth())
    }).prefix('/transactions')

    // Recurring transactions routes
    router.group(() => {
        router.get('/', '#controllers/budgetManager/recurring_controller.index').use(middleware.auth())
        router.get('/:id', '#controllers/budgetManager/recurring_controller.show').use(middleware.auth())
        router.post('/', '#controllers/budgetManager/recurring_controller.store').use(middleware.auth())
        router.put('/:id', '#controllers/budgetManager/recurring_controller.update').use(middleware.auth())
        router.delete('/:id', '#controllers/budgetManager/recurring_controller.destroy').use(middleware.auth())
    }).prefix('/recurring')

}).prefix('/api/budget-manager')