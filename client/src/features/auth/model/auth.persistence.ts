import { AUTH_STORAGE_KEY, AUTH_STORAGE_VERSION } from './auth.constants'
import type { PersistedAuthState } from './auth.types'

interface StorageLike {
  getItem(key: string): string | null
  setItem(key: string, value: string): void
  removeItem(key: string): void
}

function isPersistedAuthState(value: unknown): value is PersistedAuthState {
  if (!value || typeof value !== 'object') {
    return false
  }

  const state = value as PersistedAuthState
  return (
    state.version === AUTH_STORAGE_VERSION &&
    typeof state.updatedAt === 'string' &&
    typeof state.rememberMe === 'boolean' &&
    typeof state.accessToken === 'string' &&
    typeof state.expiresAt === 'string' &&
    typeof state.tokenType === 'string' &&
    !!state.user &&
    typeof state.user.id === 'string' &&
    typeof state.user.userName === 'string' &&
    typeof state.user.email === 'string' &&
    typeof state.user.role === 'string' &&
    typeof state.user.lastLogin === 'string'
  )
}

function getLocalStorage(): StorageLike | null {
  if (typeof window === 'undefined') {
    return null
  }

  try {
    return window.localStorage
  } catch {
    return null
  }
}

function getSessionStorage(): StorageLike | null {
  if (typeof window === 'undefined') {
    return null
  }

  try {
    return window.sessionStorage
  } catch {
    return null
  }
}

function readFromStorage(storage: StorageLike): PersistedAuthState | null {
  try {
    const raw = storage.getItem(AUTH_STORAGE_KEY)
    if (!raw) {
      return null
    }

    const parsed = JSON.parse(raw)
    if (!isPersistedAuthState(parsed)) {
      storage.removeItem(AUTH_STORAGE_KEY)
      return null
    }

    return parsed
  } catch {
    storage.removeItem(AUTH_STORAGE_KEY)
    return null
  }
}

export function loadPersistedAuthState(): PersistedAuthState | null {
  const local = getLocalStorage()
  if (local) {
    const localState = readFromStorage(local)
    if (localState) {
      return localState
    }
  }

  const session = getSessionStorage()
  if (!session) {
    return null
  }

  return readFromStorage(session)
}

export function savePersistedAuthState(state: PersistedAuthState): void {
  const local = getLocalStorage()
  const session = getSessionStorage()
  const payload = JSON.stringify(state)

  if (state.rememberMe) {
    try {
      local?.setItem(AUTH_STORAGE_KEY, payload)
    } catch {
      // Ignore storage errors so login flow keeps working.
    }

    try {
      session?.removeItem(AUTH_STORAGE_KEY)
    } catch {
      // Ignore storage errors so login flow keeps working.
    }

    return
  }

  try {
    session?.setItem(AUTH_STORAGE_KEY, payload)
  } catch {
    // Ignore storage errors so login flow keeps working.
  }

  try {
    local?.removeItem(AUTH_STORAGE_KEY)
  } catch {
    // Ignore storage errors so login flow keeps working.
  }
}

export function clearPersistedAuthState(): void {
  try {
    getLocalStorage()?.removeItem(AUTH_STORAGE_KEY)
  } catch {
    // Ignore storage errors so logout flow keeps working.
  }

  try {
    getSessionStorage()?.removeItem(AUTH_STORAGE_KEY)
  } catch {
    // Ignore storage errors so logout flow keeps working.
  }
}
