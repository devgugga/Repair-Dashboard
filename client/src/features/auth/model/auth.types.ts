export type AuthRole = string

export type AuthStatus = 'idle' | 'loading' | 'authenticated' | 'unauthenticated'

export interface AuthUser {
  id: string
  userName: string
  email: string
  role: AuthRole
  lastLogin: string
}

export interface AuthSession {
  accessToken: string
  expiresAt: string
  tokenType: string
  user: AuthUser
}

export interface AuthCurrentUser {
  user: AuthUser
  permissions: string[]
}

export interface LoginInput {
  userName: string
  password: string
  rememberMe: boolean
}

export interface PersistedAuthState {
  version: number
  updatedAt: string
  rememberMe: boolean
  accessToken: string
  expiresAt: string
  tokenType: string
  user: AuthUser
}

export interface AuthAdapter {
  login(input: LoginInput): Promise<AuthSession>
  refresh(): Promise<AuthSession>
  me(): Promise<AuthCurrentUser>
  logout(): Promise<boolean>
}
