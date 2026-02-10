import { beforeEach, describe, expect, it } from 'vitest'

import { AUTH_STORAGE_KEY, AUTH_STORAGE_VERSION, FAKE_AUTH_USER } from './auth.constants'
import {
  clearPersistedAuthState,
  loadPersistedAuthState,
  savePersistedAuthState,
} from './auth.persistence'
import type { PersistedAuthState } from './auth.types'

const validState: PersistedAuthState = {
  version: AUTH_STORAGE_VERSION,
  updatedAt: '2026-02-10T00:00:00.000Z',
  rememberMe: false,
  user: FAKE_AUTH_USER,
  token: 'mock-token',
}

describe('auth.persistence', () => {
  beforeEach(() => {
    window.localStorage.clear()
    window.sessionStorage.clear()
  })

  it('stores session in sessionStorage when rememberMe is false', () => {
    savePersistedAuthState({ ...validState, rememberMe: false })

    expect(window.sessionStorage.getItem(AUTH_STORAGE_KEY)).toBeTruthy()
    expect(window.localStorage.getItem(AUTH_STORAGE_KEY)).toBeNull()
  })

  it('stores session in localStorage when rememberMe is true', () => {
    savePersistedAuthState({ ...validState, rememberMe: true })

    expect(window.localStorage.getItem(AUTH_STORAGE_KEY)).toBeTruthy()
    expect(window.sessionStorage.getItem(AUTH_STORAGE_KEY)).toBeNull()
  })

  it('loads localStorage first when both have payloads', () => {
    window.localStorage.setItem(
      AUTH_STORAGE_KEY,
      JSON.stringify({ ...validState, rememberMe: true, token: 'local-token' }),
    )
    window.sessionStorage.setItem(
      AUTH_STORAGE_KEY,
      JSON.stringify({ ...validState, rememberMe: false, token: 'session-token' }),
    )

    const loaded = loadPersistedAuthState()

    expect(loaded?.token).toBe('local-token')
  })

  it('clears auth key from both storages', () => {
    window.localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(validState))
    window.sessionStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(validState))

    clearPersistedAuthState()

    expect(window.localStorage.getItem(AUTH_STORAGE_KEY)).toBeNull()
    expect(window.sessionStorage.getItem(AUTH_STORAGE_KEY)).toBeNull()
  })
})
