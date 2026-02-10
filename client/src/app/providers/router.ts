import { createRouter, createWebHistory } from 'vue-router'
import DashboardPage from '@pages/dashboard/ui/DashboardPage.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'dashboard',
      component: DashboardPage,
    },
  ],
})

export default router
