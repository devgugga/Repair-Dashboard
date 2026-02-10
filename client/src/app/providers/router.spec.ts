import { beforeEach, describe, expect, it } from 'vitest'

import { FAKE_AUTH_USER } from '@features/auth/model/auth.constants'
import { useAuthStore } from '@features/auth/model/useAuthStore'

import pinia from './pinia'
import router from './router'

describe.sequential('router auth guards', () => {
  beforeEach(async () => {
    const authStore = useAuthStore(pinia)
    authStore.$reset()
    window.localStorage.clear()
    window.sessionStorage.clear()

    await router.push('/login')
    await router.isReady()
  })

  it('redirects unauthenticated users from / to /login', async () => {
    await router.push('/')

    expect(router.currentRoute.value.path).toBe('/login')
    expect(router.currentRoute.value.query.redirect).toBe('/')
  })

  it('redirects authenticated users from /login to /', async () => {
    const authStore = useAuthStore(pinia)
    authStore.$patch({
      status: 'authenticated',
      initialized: true,
      user: FAKE_AUTH_USER,
      token: 'mock-token',
      rememberMe: true,
    })

    await router.push('/')
    await router.push('/login')

    expect(router.currentRoute.value.path).toBe('/')
  })

  it('preserves fullPath in redirect query', async () => {
    await router.push('/?tab=all')

    expect(router.currentRoute.value.path).toBe('/login')
    expect(router.currentRoute.value.query.redirect).toBe('/?tab=all')
  })
})
