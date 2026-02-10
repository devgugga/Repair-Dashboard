import { defineStore } from 'pinia'

import { authAdapter } from '@features/auth/api/auth.adapter'
import { AUTH_STORAGE_VERSION } from './auth.constants'
import {
  clearPersistedAuthState,
  loadPersistedAuthState,
  savePersistedAuthState,
} from './auth.persistence'
import type { AuthStatus, AuthUser, LoginInput, PersistedAuthState } from './auth.types'

interface AuthState {
  status: AuthStatus
  initialized: boolean
  rememberMe: boolean
  user: AuthUser | null
  token: string | null
}

const INVALID_CREDENTIALS_MESSAGE = 'E-mail ou senha inválidos.'

export const useAuthStore = defineStore('auth', {
  state: (): AuthState => ({
    status: 'idle',
    initialized: false,
    rememberMe: false,
    user: null,
    token: null,
  }),
  getters: {
    isAuthenticated: (state): boolean =>
      state.status === 'authenticated' && !!state.user && !!state.token,
    displayName: (state): string => state.user?.name ?? '',
  },
  actions: {
    hydrate(): void {
      if (this.initialized) {
        return
      }

      const persisted = loadPersistedAuthState()

      if (persisted) {
        this.user = persisted.user
        this.token = persisted.token
        this.rememberMe = persisted.rememberMe
        this.status = 'authenticated'
      } else {
        this.status = 'unauthenticated'
      }

      this.initialized = true
    },
    async login(input: LoginInput): Promise<void> {
      this.status = 'loading'

      try {
        const session = await authAdapter.login(input)

        this.user = session.user
        this.token = session.token
        this.rememberMe = input.rememberMe
        this.status = 'authenticated'
        this.initialized = true

        const persisted: PersistedAuthState = {
          version: AUTH_STORAGE_VERSION,
          updatedAt: new Date().toISOString(),
          rememberMe: input.rememberMe,
          user: session.user,
          token: session.token,
        }

        savePersistedAuthState(persisted)
      } catch {
        this.user = null
        this.token = null
        this.rememberMe = false
        this.status = 'unauthenticated'
        this.initialized = true
        clearPersistedAuthState()
        throw new Error(INVALID_CREDENTIALS_MESSAGE)
      }
    },
    logout(): void {
      this.user = null
      this.token = null
      this.rememberMe = false
      this.status = 'unauthenticated'
      this.initialized = true
      clearPersistedAuthState()
    },
  },
})
