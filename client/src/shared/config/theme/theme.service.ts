import { DEFAULT_THEME_MODE, THEME_ATTRIBUTE, THEME_STORAGE_KEY } from './theme.constants'
import type { ResolvedTheme, ThemeMode } from './theme.types'

function isThemeMode(value: unknown): value is ThemeMode {
  return value === 'light' || value === 'dark' || value === 'system'
}

function getSystemTheme(): ResolvedTheme {
  if (typeof window === 'undefined' || typeof window.matchMedia !== 'function') {
    return 'light'
  }

  return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light'
}

export function resolveTheme(mode: ThemeMode): ResolvedTheme {
  if (mode === 'system') {
    return getSystemTheme()
  }

  return mode
}

export function getStoredTheme(): ThemeMode {
  if (typeof window === 'undefined') {
    return DEFAULT_THEME_MODE
  }

  try {
    const raw = window.localStorage.getItem(THEME_STORAGE_KEY)
    return isThemeMode(raw) ? raw : DEFAULT_THEME_MODE
  } catch {
    return DEFAULT_THEME_MODE
  }
}

export function setStoredTheme(mode: ThemeMode): void {
  if (typeof window === 'undefined') {
    return
  }

  try {
    window.localStorage.setItem(THEME_STORAGE_KEY, mode)
  } catch {
    // Ignore storage errors to keep theme behavior functional.
  }
}

function applyResolvedTheme(theme: ResolvedTheme): void {
  if (typeof document === 'undefined') {
    return
  }

  const root = document.documentElement
  root.setAttribute(THEME_ATTRIBUTE, theme)
  root.style.colorScheme = theme
}

export function applyTheme(mode: ThemeMode): ResolvedTheme {
  const resolved = resolveTheme(mode)
  applyResolvedTheme(resolved)
  return resolved
}

export function initTheme(): ThemeMode {
  const storedMode = getStoredTheme()
  applyTheme(storedMode)
  return storedMode
}
