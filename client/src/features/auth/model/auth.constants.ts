import type { AuthUser } from './auth.types'

export const AUTH_STORAGE_KEY = 'repair-dashboard:auth'
export const AUTH_STORAGE_VERSION = 1
export const AUTH_FAKE_DELAY_MS = 650

export const FAKE_AUTH_CREDENTIALS = {
  email: 'admin@repair.com.br',
  password: 'mypass@132',
} as const

export const FAKE_AUTH_USER: AuthUser = {
  id: 'u-admin-001',
  name: 'Administrador',
  email: FAKE_AUTH_CREDENTIALS.email,
  role: 'admin',
}
