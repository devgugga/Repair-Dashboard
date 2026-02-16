import { defineStore } from 'pinia'

import { AuthApiError, authAdapter } from '@features/auth/api/auth.adapter'
import { ACCESS_TOKEN_EXPIRY_SKEW_MS, AUTH_STORAGE_VERSION } from './auth.constants'
import {
  clearPersistedAuthState,
  loadPersistedAuthState,
  savePersistedAuthState,
} from './auth.persistence'
import type { AuthSession, AuthStatus, AuthUser, LoginInput, PersistedAuthState } from './auth.types'

interface AuthState {
  status: AuthStatus
  initialized: boolean
  rememberMe: boolean
  user: AuthUser | null
  permissions: string[]
  profileLoaded: boolean
  accessToken: string | null
  expiresAt: string | null
  tokenType: string | null
}

const INVALID_CREDENTIALS_MESSAGE = 'Usuário ou senha inválidos.'
const LOGIN_FAILURE_MESSAGE = 'Não foi possível entrar no sistema. Tente novamente.'

function isTokenExpired(expiresAt: string | null): boolean {
  if (!expiresAt) {
    return true
  }

  const expiresAtMs = new Date(expiresAt).getTime()
  if (Number.isNaN(expiresAtMs)) {
    return true
  }

  return expiresAtMs <= Date.now() + ACCESS_TOKEN_EXPIRY_SKEW_MS
}

export const useAuthStore = defineStore('auth', {
  state: (): AuthState => ({
    status: 'idle',
    initialized: false,
    rememberMe: false,
    user: null,
    permissions: [],
    profileLoaded: false,
    accessToken: null,
    expiresAt: null,
    tokenType: null,
  }),
  getters: {
    isAuthenticated: (state): boolean =>
      state.status === 'authenticated' &&
      !!state.user &&
      !!state.accessToken &&
      !!state.expiresAt &&
      !isTokenExpired(state.expiresAt),
    hasSessionContext: (state): boolean =>
      !!state.user && !!state.accessToken && !!state.expiresAt && !!state.tokenType,
    isAccessTokenExpired: (state): boolean => isTokenExpired(state.expiresAt),
    displayName: (state): string => state.user?.userName ?? '',
    authorizationHeader: (state): string | null =>
      state.accessToken && state.tokenType ? `${state.tokenType} ${state.accessToken}` : null,
  },
  actions: {
    applySession(session: AuthSession, rememberMe: boolean): void {
      this.user = session.user
      this.permissions = []
      this.profileLoaded = false
      this.accessToken = session.accessToken
      this.expiresAt = session.expiresAt
      this.tokenType = session.tokenType
      this.rememberMe = rememberMe
      this.status = 'authenticated'
      this.initialized = true

      const persisted: PersistedAuthState = {
        version: AUTH_STORAGE_VERSION,
        updatedAt: new Date().toISOString(),
        rememberMe,
        accessToken: session.accessToken,
        expiresAt: session.expiresAt,
        tokenType: session.tokenType,
        user: session.user,
      }

      savePersistedAuthState(persisted)
    },
    clearSession(): void {
      this.user = null
      this.permissions = []
      this.profileLoaded = false
      this.accessToken = null
      this.expiresAt = null
      this.tokenType = null
      this.rememberMe = false
      this.status = 'unauthenticated'
      this.initialized = true
      clearPersistedAuthState()
    },
    hydrate(): void {
      if (this.initialized) {
        return
      }

      const persisted = loadPersistedAuthState()

      if (persisted) {
        this.user = persisted.user
        this.permissions = []
        this.profileLoaded = false
        this.accessToken = persisted.accessToken
        this.expiresAt = persisted.expiresAt
        this.tokenType = persisted.tokenType
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
        this.applySession(session, input.rememberMe)

        const loaded = await this.loadCurrentUser()
        if (!loaded) {
          this.clearSession()
          throw new Error(LOGIN_FAILURE_MESSAGE)
        }
      } catch (error) {
        this.clearSession()

        if (error instanceof AuthApiError && error.status === 401) {
          throw new Error(INVALID_CREDENTIALS_MESSAGE)
        }

        throw new Error(LOGIN_FAILURE_MESSAGE)
      }
    },
    async refreshSession(): Promise<boolean> {
      this.status = 'loading'

      try {
        const session = await authAdapter.refresh()
        this.applySession(session, this.rememberMe)
        return true
      } catch {
        this.clearSession()
        return false
      }
    },
    async loadCurrentUser(): Promise<boolean> {
      if (!this.hasSessionContext) {
        return false
      }

      try {
        const currentUser = await authAdapter.me()
        this.user = currentUser.user
        this.permissions = currentUser.permissions
        this.profileLoaded = true
        this.status = 'authenticated'
        this.initialized = true

        const persisted: PersistedAuthState = {
          version: AUTH_STORAGE_VERSION,
          updatedAt: new Date().toISOString(),
          rememberMe: this.rememberMe,
          accessToken: this.accessToken as string,
          expiresAt: this.expiresAt as string,
          tokenType: this.tokenType as string,
          user: currentUser.user,
        }

        savePersistedAuthState(persisted)
        return true
      } catch {
        return false
      }
    },
    async ensureSession(): Promise<boolean> {
      if (!this.initialized) {
        this.hydrate()
      }

      if (!this.hasSessionContext) {
        return false
      }

      if (this.isAccessTokenExpired) {
        const refreshed = await this.refreshSession()
        if (!refreshed) {
          return false
        }
      }

      if (!this.profileLoaded) {
        let loaded = await this.loadCurrentUser()
        if (!loaded) {
          const refreshed = await this.refreshSession()
          if (!refreshed) {
            return false
          }

          loaded = await this.loadCurrentUser()
          if (!loaded) {
            this.clearSession()
            return false
          }
        }
      }

      return this.isAuthenticated
    },
    async logout(): Promise<void> {
      if (!this.hasSessionContext) {
        this.clearSession()
        return
      }

      try {
        await authAdapter.logout()
      } catch {
        // Ignore API logout failures and ensure local logout still succeeds.
      } finally {
        this.clearSession()
      }
    },
  },
})
