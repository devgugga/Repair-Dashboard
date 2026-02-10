import { mount } from '@vue/test-utils'
import { nextTick } from 'vue'
import { afterEach, describe, expect, it } from 'vitest'
import PrimeVue from 'primevue/config'

import App from '@app/App.vue'
import pinia from '@app/providers/pinia'
import router from '@app/providers/router'
import { FAKE_AUTH_USER } from '@features/auth/model/auth.constants'
import { useAuthStore } from '@features/auth/model/useAuthStore'

describe.sequential('App layout routing', () => {
  let wrapper: ReturnType<typeof mount> | null = null

  afterEach(() => {
    wrapper?.unmount()
    wrapper = null
    document.body.innerHTML = ''
  })

  function mountApp() {
    wrapper = mount(App, {
      attachTo: document.body,
      global: {
        plugins: [
          pinia,
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

    return wrapper
  }

  it('does not render the app header on /login', async () => {
    const authStore = useAuthStore(pinia)
    authStore.$reset()
    window.localStorage.clear()
    window.sessionStorage.clear()

    await router.push('/login')
    await router.isReady()

    const wrapper = mountApp()

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

    const wrapper = mountApp()

    expect(wrapper.text()).toContain('Repair Control')
    expect(wrapper.text()).toContain('Dashboard')
    expect(wrapper.find('button[aria-label="Expandir menu lateral"]').exists()).toBe(true)
  })

  it('opens sidebar drawer on mobile trigger', async () => {
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

    const wrapper = mountApp()
    const openNavButton = wrapper.find('button[aria-label="Abrir navegação"]')

    await openNavButton.trigger('click')
    await nextTick()

    expect(document.body.querySelector('.p-drawer')).not.toBeNull()
  })
})
