import router from '@adonisjs/core/services/router'
import { middleware } from './kernel.js'


router.group(() => {

    router.group(() => {
        router.get('/', '#controllers/budgetManager/accounts_controller.index').use(middleware.auth())

    }).prefix('/accounts')

}).prefix('/api/budget-manager')