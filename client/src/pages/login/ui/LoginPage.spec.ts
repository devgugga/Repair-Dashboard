import { createPinia, setActivePinia } from 'pinia'
import { flushPromises, mount } from '@vue/test-utils'
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

  it('shows validation message when username/password are empty', async () => {
    const wrapper = await mountLoginPage()

    await wrapper.find('form').trigger('submit.prevent')

    expect(wrapper.text()).toContain('Informe usuário e senha para continuar.')
  })

  it('shows invalid credentials message when login fails', async () => {
    vi.spyOn(globalThis, 'fetch').mockResolvedValue(
      new Response(
        JSON.stringify({
          status: 401,
          title: 'Authentication failed.',
          detail: 'Invalid username or password',
        }),
        { status: 401 },
      ),
    )
    const wrapper = await mountLoginPage()

    await wrapper.find('input[type="text"]').setValue('wrong-user')
    await wrapper.find('input[type="password"]').setValue('wrong-pass')
    await wrapper.find('form').trigger('submit')

    await flushPromises()

    expect(wrapper.text()).toContain('Usuário ou senha inválidos.')
  })

  it('redirects to query redirect target after successful login', async () => {
    const fetchSpy = vi.spyOn(globalThis, 'fetch')
    fetchSpy.mockResolvedValueOnce(
      new Response(
        JSON.stringify({
          accessToken: 'jwt-token',
          expiresAt: '2030-01-01T00:00:00.000Z',
          tokenType: 'Bearer',
          user: {
            id: 'u-1',
            userName: 'admin',
            email: 'admin@repair.com.br',
            role: 'admin',
            lastLogin: '2026-02-16T00:00:00.000Z',
          },
        }),
        { status: 200 },
      ),
    )
    fetchSpy.mockResolvedValueOnce(
      new Response(
        JSON.stringify({
          user: {
            id: 'u-1',
            userName: 'admin',
            email: 'admin@repair.com.br',
            role: 'admin',
            lastLogin: '2026-02-16T00:00:00.000Z',
          },
          permissions: ['users.read'],
        }),
        { status: 200 },
      ),
    )
    await router.push('/login?redirect=%2F%3Ftab%3Dall')
    const wrapper = await mountLoginPage()

    await wrapper.find('input[type="text"]').setValue('admin')
    await wrapper.find('input[type="password"]').setValue('secret')
    await wrapper.find('form').trigger('submit')

    await flushPromises()

    expect(router.currentRoute.value.fullPath).toBe('/?tab=all')
  })
})
