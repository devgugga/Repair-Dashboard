import { mount } from '@vue/test-utils'
import { nextTick } from 'vue'
import { beforeEach, describe, expect, it } from 'vitest'
import PrimeVue from 'primevue/config'
import { createMemoryHistory, createRouter } from 'vue-router'

import AppSidebar from './AppSidebar.vue'

function buildRouter() {
  return createRouter({
    history: createMemoryHistory(),
    routes: [
      { path: '/', name: 'dashboard', component: { template: '<div />' } },
      { path: '/clientes', name: 'customers', component: { template: '<div />' } },
      { path: '/aparelhos', name: 'devices', component: { template: '<div />' } },
      { path: '/os', name: 'service-orders', component: { template: '<div />' } },
      { path: '/historico', name: 'repair-history', component: { template: '<div />' } },
      { path: '/pecas', name: 'parts', component: { template: '<div />' } },
      { path: '/mov', name: 'stock-movements', component: { template: '<div />' } },
      { path: '/forn', name: 'suppliers', component: { template: '<div />' } },
      { path: '/caixa', name: 'cashflow', component: { template: '<div />' } },
      { path: '/receber', name: 'receivables', component: { template: '<div />' } },
      { path: '/rel', name: 'finance-reports', component: { template: '<div />' } },
      { path: '/users', name: 'users-permissions', component: { template: '<div />' } },
      { path: '/config', name: 'system-settings', component: { template: '<div />' } },
    ],
  })
}

describe.sequential('AppSidebar', () => {
  let router: ReturnType<typeof buildRouter>

  beforeEach(async () => {
    router = buildRouter()
    await router.push('/')
    await router.isReady()
  })

  function mountSidebar(collapsed = false) {
    return mount(AppSidebar, {
      props: { collapsed },
      global: {
        plugins: [
          router,
          [
            PrimeVue,
            {
              ripple: true,
            },
          ],
        ],
      },
    })
  }

  it('renders categories in expanded mode', async () => {
    const wrapper = mountSidebar(false)

    expect(wrapper.text()).toContain('Operação')
    expect(wrapper.text()).toContain('Estoque')
    expect(wrapper.text()).toContain('Financeiro')
    expect(wrapper.text()).toContain('Configurações')
  })

  it('emits toggle-collapse when collapse button is clicked', async () => {
    const wrapper = mountSidebar(false)
    const button = wrapper.find('button[aria-label="Recolher menu lateral"]')

    await button.trigger('click')

    expect(wrapper.emitted('toggle-collapse')).toHaveLength(1)
  })

  it('navigates and emits navigate in collapsed mode', async () => {
    const wrapper = mountSidebar(true)
    const dashboardButton = wrapper
      .findAll('button')
      .find((buttonWrapper) => buttonWrapper.find('i.pi-home').exists())

    if (!dashboardButton) {
      throw new Error('Dashboard icon button not found in collapsed sidebar')
    }

    await dashboardButton.trigger('click')

    expect(wrapper.emitted('navigate')).toHaveLength(1)
    expect(router.currentRoute.value.name).toBe('dashboard')
  })

  it('navigates and emits navigate in expanded mode', async () => {
    const wrapper = mountSidebar(false)
    await nextTick()

    const customersItem = wrapper.findAll('a').find((node) => node.attributes('href') === '/clientes')

    if (!customersItem) {
      throw new Error('Clientes item not found in expanded sidebar')
    }

    await customersItem.trigger('click')

    expect(wrapper.emitted('navigate')).toHaveLength(1)
    expect(customersItem.attributes('href')).toBe('/clientes')
  })
})
