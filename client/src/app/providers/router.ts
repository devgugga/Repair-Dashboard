import { createRouter, createWebHistory } from 'vue-router'

import AppLayout from '@app/layouts/AppLayout.vue'
import pinia from '@app/providers/pinia'
import { useAuthStore } from '@features/auth/model/useAuthStore'
import PlaceholderPage from '@pages/common/ui/PlaceholderPage.vue'
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
            title: 'Dashboard',
            subtitle: 'Gestão completa de clientes, aparelhos, consertos e peças.',
          },
        },
        {
          path: 'clientes',
          name: 'customers',
          component: PlaceholderPage,
          meta: {
            title: 'Clientes',
            subtitle: 'Gerencie cadastro, contatos e histórico de atendimento dos clientes.',
          },
        },
        {
          path: 'aparelhos',
          name: 'devices',
          component: PlaceholderPage,
          meta: {
            title: 'Aparelhos',
            subtitle: 'Controle todos os equipamentos vinculados aos clientes.',
          },
        },
        {
          path: 'ordens-servico',
          name: 'service-orders',
          component: PlaceholderPage,
          meta: {
            title: 'Ordens de Serviço',
            subtitle: 'Acompanhe abertura, execução e conclusão de consertos.',
          },
        },
        {
          path: 'historico-consertos',
          name: 'repair-history',
          component: PlaceholderPage,
          meta: {
            title: 'Histórico de Consertos',
            subtitle: 'Consulte os serviços realizados por cliente e aparelho.',
          },
        },
        {
          path: 'pecas',
          name: 'parts',
          component: PlaceholderPage,
          meta: {
            title: 'Peças',
            subtitle: 'Gerencie catálogo e disponibilidade de peças.',
          },
        },
        {
          path: 'movimentacoes-estoque',
          name: 'stock-movements',
          component: PlaceholderPage,
          meta: {
            title: 'Movimentações de Estoque',
            subtitle: 'Registre entradas, saídas e ajustes de estoque.',
          },
        },
        {
          path: 'fornecedores',
          name: 'suppliers',
          component: PlaceholderPage,
          meta: {
            title: 'Fornecedores',
            subtitle: 'Controle parceiros e condições de compra.',
          },
        },
        {
          path: 'caixa',
          name: 'cashflow',
          component: PlaceholderPage,
          meta: {
            title: 'Caixa',
            subtitle: 'Acompanhe fluxo diário de entradas e saídas.',
          },
        },
        {
          path: 'contas-receber',
          name: 'receivables',
          component: PlaceholderPage,
          meta: {
            title: 'Contas a Receber',
            subtitle: 'Gerencie títulos abertos e vencimentos.',
          },
        },
        {
          path: 'relatorios-financeiros',
          name: 'finance-reports',
          component: PlaceholderPage,
          meta: {
            title: 'Relatórios Financeiros',
            subtitle: 'Visualize indicadores e consolidado financeiro.',
          },
        },
        {
          path: 'usuarios-permissoes',
          name: 'users-permissions',
          component: PlaceholderPage,
          meta: {
            title: 'Usuários e Permissões',
            subtitle: 'Defina acessos e perfis de utilização do sistema.',
          },
        },
        {
          path: 'parametros-sistema',
          name: 'system-settings',
          component: PlaceholderPage,
          meta: {
            title: 'Parâmetros do Sistema',
            subtitle: 'Configure preferências e regras globais da aplicação.',
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
