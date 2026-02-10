export type AuthRole = 'admin'

export type AuthStatus = 'idle' | 'loading' | 'authenticated' | 'unauthenticated'

export interface AuthUser {
  id: string
  name: string
  email: string
  role: AuthRole
}

export interface AuthSession {
  user: AuthUser
  token: string
  issuedAt: string
}

export interface LoginInput {
  email: string
  password: string
  rememberMe: boolean
}

export interface PersistedAuthState {
  version: number
  updatedAt: string
  rememberMe: boolean
  user: AuthUser
  token: string
}

export interface AuthAdapter {
  login(input: LoginInput): Promise<AuthSession>
}
