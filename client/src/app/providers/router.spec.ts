import { beforeEach, describe, expect, it, vi } from 'vitest'

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
      permissions: [],
      profileLoaded: true,
      accessToken: 'mock-token',
      expiresAt: '2030-01-01T00:00:00.000Z',
      tokenType: 'Bearer',
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

  it('tries refresh when access token is expired and keeps route on success', async () => {
    const authStore = useAuthStore(pinia)
    authStore.$patch({
      status: 'authenticated',
      initialized: true,
      user: FAKE_AUTH_USER,
      permissions: [],
      profileLoaded: false,
      accessToken: 'expired-token',
      expiresAt: '2020-01-01T00:00:00.000Z',
      tokenType: 'Bearer',
      rememberMe: true,
    })

    const fetchSpy = vi.spyOn(globalThis, 'fetch')
    fetchSpy.mockResolvedValueOnce(
      new Response(
        JSON.stringify({
          accessToken: 'new-token',
          expiresAt: '2030-01-01T00:00:00.000Z',
          tokenType: 'Bearer',
          user: FAKE_AUTH_USER,
        }),
        { status: 200 },
      ),
    )
    fetchSpy.mockResolvedValueOnce(
      new Response(
        JSON.stringify({
          user: FAKE_AUTH_USER,
          permissions: ['users.read'],
        }),
        { status: 200 },
      ),
    )

    await router.push('/')

    expect(router.currentRoute.value.path).toBe('/')
    expect(authStore.accessToken).toBe('new-token')
    expect(authStore.profileLoaded).toBe(true)
  })
})
