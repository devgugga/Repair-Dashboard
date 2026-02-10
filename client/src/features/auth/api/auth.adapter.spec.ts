import { describe, expect, it, vi } from 'vitest'

import { AUTH_FAKE_DELAY_MS, FAKE_AUTH_CREDENTIALS } from '@features/auth/model/auth.constants'

import { InvalidCredentialsError, loginWithMockAuth } from './auth.adapter'

describe('auth.adapter', () => {
  it('returns a fake session when credentials are valid', async () => {
    vi.useFakeTimers()

    const loginPromise = loginWithMockAuth({
      email: FAKE_AUTH_CREDENTIALS.email,
      password: FAKE_AUTH_CREDENTIALS.password,
      rememberMe: false,
    })

    vi.advanceTimersByTime(AUTH_FAKE_DELAY_MS)
    const session = await loginPromise

    expect(session.user.email).toBe(FAKE_AUTH_CREDENTIALS.email)
    expect(session.token.startsWith('mock-token-')).toBe(true)
    expect(session.issuedAt).toBeTruthy()
  })

  it('throws InvalidCredentialsError for invalid credentials', async () => {
    vi.useFakeTimers()

    const loginPromise = loginWithMockAuth({
      email: 'wrong@repair.com.br',
      password: 'wrong-pass',
      rememberMe: true,
    })

    vi.advanceTimersByTime(AUTH_FAKE_DELAY_MS)
    await expect(loginPromise).rejects.toBeInstanceOf(InvalidCredentialsError)
  })
})
