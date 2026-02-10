import { mount } from '@vue/test-utils'
import { describe, expect, it } from 'vitest'
import Aura from '@primeuix/themes/aura'
import PrimeVue from 'primevue/config'

import App from '@app/App.vue'
import pinia from '@app/providers/pinia'
import router from '@app/providers/router'
import { FAKE_AUTH_USER } from '@features/auth/model/auth.constants'
import { useAuthStore } from '@features/auth/model/useAuthStore'

describe.sequential('App layout routing', () => {
  it('does not render the app header on /login', async () => {
    const authStore = useAuthStore(pinia)
    authStore.$reset()
    window.localStorage.clear()
    window.sessionStorage.clear()

    await router.push('/login')
    await router.isReady()

    const wrapper = mount(App, {
      global: {
        plugins: [
          pinia,
          router,
          [
            PrimeVue,
            {
              theme: {
                preset: Aura,
                options: {
                  darkModeSelector: '[data-theme="dark"]',
                },
              },
            },
          ],
        ],
      },
    })

    expect(wrapper.text()).toContain('Acesse sua conta')
    expect(wrapper.text()).not.toContain('Repair Control')
  })

  it('renders the app header on authenticated routes', async () => {
    const authStore = useAuthStore(pinia)
    authStore.$patch({
      status: 'authenticated',
      initialized: true,
      user: FAKE_AUTH_USER,
      token: 'mock-token',
      rememberMe: true,
    })

    await router.push('/')
    await router.isReady()

    const wrapper = mount(App, {
      global: {
        plugins: [
          pinia,
          router,
          [
            PrimeVue,
            {
              theme: {
                preset: Aura,
                options: {
                  darkModeSelector: '[data-theme="dark"]',
                },
              },
            },
          ],
        ],
      },
    })

    expect(wrapper.text()).toContain('Repair Control')
    expect(wrapper.text()).toContain('Operations Overview')
  })
})
