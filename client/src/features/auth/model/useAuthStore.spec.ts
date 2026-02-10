import { createPinia, setActivePinia } from 'pinia'
import { beforeEach, describe, expect, it, vi } from 'vitest'

import { AUTH_FAKE_DELAY_MS, AUTH_STORAGE_KEY, FAKE_AUTH_CREDENTIALS } from './auth.constants'
import { useAuthStore } from './useAuthStore'

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

  it('authenticates and persists login when credentials are valid', async () => {
    vi.useFakeTimers()

    const authStore = useAuthStore()
    const loginPromise = authStore.login({
      email: FAKE_AUTH_CREDENTIALS.email,
      password: FAKE_AUTH_CREDENTIALS.password,
      rememberMe: true,
    })

    vi.advanceTimersByTime(AUTH_FAKE_DELAY_MS)
    await loginPromise

    expect(authStore.isAuthenticated).toBe(true)
    expect(authStore.user?.email).toBe(FAKE_AUTH_CREDENTIALS.email)
    expect(authStore.rememberMe).toBe(true)
    expect(window.localStorage.getItem(AUTH_STORAGE_KEY)).toBeTruthy()
  })

  it('clears state and storage on logout', async () => {
    vi.useFakeTimers()

    const authStore = useAuthStore()
    const loginPromise = authStore.login({
      email: FAKE_AUTH_CREDENTIALS.email,
      password: FAKE_AUTH_CREDENTIALS.password,
      rememberMe: false,
    })

    vi.advanceTimersByTime(AUTH_FAKE_DELAY_MS)
    await loginPromise

    authStore.logout()

    expect(authStore.isAuthenticated).toBe(false)
    expect(authStore.user).toBeNull()
    expect(authStore.token).toBeNull()
    expect(window.localStorage.getItem(AUTH_STORAGE_KEY)).toBeNull()
    expect(window.sessionStorage.getItem(AUTH_STORAGE_KEY)).toBeNull()
  })

  it('rejects invalid credentials and keeps unauthenticated state', async () => {
    vi.useFakeTimers()

    const authStore = useAuthStore()
    const loginPromise = authStore.login({
      email: 'wrong@repair.com.br',
      password: 'wrong',
      rememberMe: false,
    })

    vi.advanceTimersByTime(AUTH_FAKE_DELAY_MS)
    await expect(loginPromise).rejects.toThrow('E-mail ou senha inválidos.')

    expect(authStore.isAuthenticated).toBe(false)
    expect(authStore.status).toBe('unauthenticated')
  })
})
