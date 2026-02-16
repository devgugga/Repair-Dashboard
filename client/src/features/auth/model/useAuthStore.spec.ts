import { createPinia, setActivePinia } from 'pinia'
import { beforeEach, describe, expect, it, vi } from 'vitest'

import { AUTH_STORAGE_KEY } from './auth.constants'
import { useAuthStore } from './useAuthStore'

function createAuthSuccessResponse(overrides?: Record<string, unknown>): Response {
  return new Response(
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
      ...overrides,
    }),
    { status: 200 },
  )
}

function createMeSuccessResponse(overrides?: Record<string, unknown>): Response {
  return new Response(
    JSON.stringify({
      user: {
        id: 'u-1',
        userName: 'admin',
        email: 'admin@repair.com.br',
        role: 'admin',
        lastLogin: '2026-02-16T00:00:00.000Z',
      },
      permissions: ['users.read', 'users.manage_roles'],
      ...overrides,
    }),
    { status: 200 },
  )
}

describe('useAuthStore', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    window.localStorage.clear()
    window.sessionStorage.clear()
  })

  it('hydrates as unauthenticated when no persisted session is available', () => {
    const authStore = useAuthStore()

    authStore.hydrate()

    expect(authStore.initialized).toBe(true)
    expect(authStore.isAuthenticated).toBe(false)
    expect(authStore.status).toBe('unauthenticated')
  })

  it('authenticates and persists login when API credentials are valid', async () => {
    const fetchSpy = vi.spyOn(globalThis, 'fetch')
    fetchSpy.mockResolvedValueOnce(createAuthSuccessResponse())
    fetchSpy.mockResolvedValueOnce(createMeSuccessResponse())

    const authStore = useAuthStore()
    await authStore.login({
      userName: 'admin',
      password: 'secret',
      rememberMe: true,
    })

    expect(authStore.isAuthenticated).toBe(true)
    expect(authStore.user?.userName).toBe('admin')
    expect(authStore.permissions).toContain('users.read')
    expect(authStore.profileLoaded).toBe(true)
    expect(authStore.accessToken).toBe('jwt-token')
    expect(window.localStorage.getItem(AUTH_STORAGE_KEY)).toBeTruthy()
  })

  it('refreshes session and updates access token', async () => {
    const fetchSpy = vi.spyOn(globalThis, 'fetch')
    fetchSpy.mockResolvedValueOnce(createAuthSuccessResponse())
    fetchSpy.mockResolvedValueOnce(createMeSuccessResponse())
    fetchSpy.mockResolvedValueOnce(createAuthSuccessResponse({ accessToken: 'jwt-token-2' }))

    const authStore = useAuthStore()
    await authStore.login({
      userName: 'admin',
      password: 'secret',
      rememberMe: false,
    })

    const refreshed = await authStore.refreshSession()

    expect(refreshed).toBe(true)
    expect(authStore.accessToken).toBe('jwt-token-2')
    expect(window.sessionStorage.getItem(AUTH_STORAGE_KEY)).toBeTruthy()
  })

  it('clears state when refresh fails', async () => {
    const fetchSpy = vi.spyOn(globalThis, 'fetch')
    fetchSpy.mockResolvedValueOnce(createAuthSuccessResponse())
    fetchSpy.mockResolvedValueOnce(createMeSuccessResponse())
    fetchSpy.mockResolvedValueOnce(
      new Response(JSON.stringify({ status: 401, title: 'Authentication failed.' }), {
        status: 401,
      }),
    )

    const authStore = useAuthStore()
    await authStore.login({
      userName: 'admin',
      password: 'secret',
      rememberMe: false,
    })

    const refreshed = await authStore.refreshSession()

    expect(refreshed).toBe(false)
    expect(authStore.isAuthenticated).toBe(false)
    expect(authStore.user).toBeNull()
    expect(authStore.accessToken).toBeNull()
  })

  it('rejects invalid credentials and keeps unauthenticated state', async () => {
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

    const authStore = useAuthStore()

    await expect(
      authStore.login({
        userName: 'wrong',
        password: 'wrong',
        rememberMe: false,
      }),
    ).rejects.toThrow('Usuário ou senha inválidos.')

    expect(authStore.isAuthenticated).toBe(false)
    expect(authStore.status).toBe('unauthenticated')
  })

  it('loads current user and permissions with existing session', async () => {
    const fetchSpy = vi.spyOn(globalThis, 'fetch')
    fetchSpy.mockResolvedValueOnce(createMeSuccessResponse())

    const authStore = useAuthStore()
    authStore.$patch({
      initialized: true,
      status: 'authenticated',
      rememberMe: true,
      user: {
        id: 'u-1',
        userName: 'admin',
        email: 'admin@repair.com.br',
        role: 'admin',
        lastLogin: '2026-02-16T00:00:00.000Z',
      },
      accessToken: 'jwt-token',
      expiresAt: '2030-01-01T00:00:00.000Z',
      tokenType: 'Bearer',
    })

    const loaded = await authStore.loadCurrentUser()

    expect(loaded).toBe(true)
    expect(authStore.permissions).toContain('users.manage_roles')
    expect(authStore.profileLoaded).toBe(true)
  })
})
