import { mount } from '@vue/test-utils'
import { describe, expect, it } from 'vitest'
import Aura from '@primeuix/themes/aura'
import PrimeVue from 'primevue/config'

import App from '@app/App.vue'
import pinia from '@app/providers/pinia'
import router from '@app/providers/router'
import { useAuthStore } from '@features/auth/model/useAuthStore'

describe('App smoke', () => {
  it('renders login page when user is not authenticated', async () => {
    const authStore = useAuthStore(pinia)
    authStore.$reset()
    window.localStorage.clear()
    window.sessionStorage.clear()

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

    expect(wrapper.text()).toContain('Acesse sua conta')
  })
})
