import { createRouter, createWebHistory } from 'vue-router'
import routes from './routes'
import store from '@/store'

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to, from, next) => {
  if (to.meta.requiredAuth && !store.getters['auth/isLoggedIn']) {
    return next({ name: 'Login' })
  }
  next()
})

export default router
