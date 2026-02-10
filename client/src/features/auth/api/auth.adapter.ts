import {
  AUTH_FAKE_DELAY_MS,
  FAKE_AUTH_CREDENTIALS,
  FAKE_AUTH_USER,
} from '@features/auth/model/auth.constants'
import type { AuthAdapter, AuthSession, LoginInput } from '@features/auth/model/auth.types'

export class InvalidCredentialsError extends Error {
  constructor() {
    super('Invalid credentials')
    this.name = 'InvalidCredentialsError'
  }
}

function sleep(milliseconds: number): Promise<void> {
  return new Promise((resolve) => {
    setTimeout(resolve, milliseconds)
  })
}

export async function loginWithMockAuth(input: LoginInput): Promise<AuthSession> {
  await sleep(AUTH_FAKE_DELAY_MS)

  const normalizedEmail = input.email.trim().toLowerCase()
  const isEmailValid = normalizedEmail === FAKE_AUTH_CREDENTIALS.email
  const isPasswordValid = input.password === FAKE_AUTH_CREDENTIALS.password

  if (!isEmailValid || !isPasswordValid) {
    throw new InvalidCredentialsError()
  }

  return {
    user: { ...FAKE_AUTH_USER },
    token: `mock-token-${Date.now()}`,
    issuedAt: new Date().toISOString(),
  }
}

export const authAdapter: AuthAdapter = {
  login: loginWithMockAuth,
}
