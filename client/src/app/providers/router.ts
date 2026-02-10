import { createRouter, createWebHistory } from 'vue-router'

import AppLayout from '@app/layouts/AppLayout.vue'
import pinia from '@app/providers/pinia'
import { useAuthStore } from '@features/auth/model/useAuthStore'
import DashboardPage from '@pages/dashboard/ui/DashboardPage.vue'
import LoginPage from '@pages/login/ui/LoginPage.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: LoginPage,
      meta: {
        guestOnly: true,
      },
    },
    {
      path: '/',
      component: AppLayout,
      meta: {
        requiresAuth: true,
      },
      children: [
        {
          path: '',
          name: 'dashboard',
          component: DashboardPage,
          meta: {
            title: 'Operations Overview',
            subtitle: 'Gestão completa de clientes, aparelhos, consertos e peças.',
          },
        },
      ],
    },
  ],
})

router.beforeEach((to) => {
  const authStore = useAuthStore(pinia)

  if (!authStore.initialized) {
    authStore.hydrate()
  }

  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    return {
      path: '/login',
      query: {
        redirect: to.fullPath,
      },
    }
  }

  if (to.meta.guestOnly && authStore.isAuthenticated) {
    return {
      path: '/',
    }
  }

  return true
})

export default router
