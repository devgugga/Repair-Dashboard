import { createPinia, setActivePinia } from 'pinia'
import { mount } from '@vue/test-utils'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import PrimeVue from 'primevue/config'

import pinia from '@app/providers/pinia'
import router from '@app/providers/router'
import { useAuthStore } from '@features/auth/model/useAuthStore'

import LoginPage from './LoginPage.vue'

async function mountLoginPage() {
  await router.isReady()

  return mount(LoginPage, {
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
}

describe.sequential('LoginPage', () => {
  beforeEach(async () => {
    setActivePinia(createPinia())
    useAuthStore(pinia).$reset()
    window.localStorage.clear()
    window.sessionStorage.clear()
    await router.push('/login')
  })

  it('shows validation message when email/password are empty', async () => {
    const wrapper = await mountLoginPage()

    await wrapper.find('form').trigger('submit.prevent')

    expect(wrapper.text()).toContain('Informe e-mail e senha para continuar.')
  })

  it('shows invalid credentials message when login fails', async () => {
    vi.useFakeTimers()
    const wrapper = await mountLoginPage()

    await wrapper.find('input[type="email"]').setValue('wrong@repair.com.br')
    await wrapper.find('input[type="password"]').setValue('wrong-pass')
    await wrapper.find('form').trigger('submit.prevent')

    vi.runAllTimers()
    await vi.dynamicImportSettled()
    await wrapper.vm.$nextTick()

    expect(wrapper.text()).toContain('E-mail ou senha inválidos.')
  })

  it('redirects to query redirect target after successful login', async () => {
    vi.useFakeTimers()
    await router.push('/login?redirect=%2F%3Ftab%3Dall')
    const wrapper = await mountLoginPage()

    await wrapper.find('input[type="email"]').setValue('admin@repair.com.br')
    await wrapper.find('input[type="password"]').setValue('mypass@132')
    await wrapper.find('form').trigger('submit.prevent')

    vi.runAllTimers()
    await vi.dynamicImportSettled()
    await wrapper.vm.$nextTick()

    expect(router.currentRoute.value.fullPath).toBe('/?tab=all')
  })
})
