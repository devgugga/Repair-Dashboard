import type { AuthUser } from './auth.types'

export const AUTH_STORAGE_KEY = 'repair-dashboard:auth'
export const AUTH_STORAGE_VERSION = 1
export const ACCESS_TOKEN_EXPIRY_SKEW_MS = 30_000

export const FAKE_AUTH_USER: AuthUser = {
  id: 'u-admin-001',
  userName: 'Administrador',
  email: 'admin@repair.com.br',
  role: 'admin',
  lastLogin: '2026-02-10T00:00:00.000Z',
}
