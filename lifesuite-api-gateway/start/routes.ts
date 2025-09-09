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