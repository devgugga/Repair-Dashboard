import { computed, onMounted, onUnmounted, ref } from 'vue'

import type { ResolvedTheme, ThemeMode } from '@shared/config/theme/theme.types'
import {
  applyTheme,
  getStoredTheme,
  resolveTheme,
  setStoredTheme,
} from '@shared/config/theme/theme.service'

const themeMode = ref<ThemeMode>(getStoredTheme())
const resolvedTheme = ref<ResolvedTheme>(resolveTheme(themeMode.value))

let mediaQuery: MediaQueryList | null = null
let mediaQueryListener: ((event: MediaQueryListEvent) => void) | null = null
let listenersInitialized = false

function syncTheme(): void {
  resolvedTheme.value = applyTheme(themeMode.value)
}

function attachSystemListener(): void {
  if (
    listenersInitialized ||
    typeof window === 'undefined' ||
    typeof window.matchMedia !== 'function'
  ) {
    return
  }

  mediaQuery = window.matchMedia('(prefers-color-scheme: dark)')
  mediaQueryListener = () => {
    if (themeMode.value === 'system') {
      syncTheme()
    }
  }

  mediaQuery.addEventListener('change', mediaQueryListener)
  listenersInitialized = true
}

function detachSystemListener(): void {
  if (!listenersInitialized || !mediaQuery || !mediaQueryListener) {
    return
  }

  mediaQuery.removeEventListener('change', mediaQueryListener)
  mediaQuery = null
  mediaQueryListener = null
  listenersInitialized = false
}

export function useTheme() {
  onMounted(() => {
    syncTheme()
    attachSystemListener()
  })

  onUnmounted(() => {
    detachSystemListener()
  })

  function setThemeMode(mode: ThemeMode): void {
    themeMode.value = mode
    setStoredTheme(mode)
    syncTheme()
  }

  function toggleTheme(): void {
    const nextMode: ThemeMode = resolvedTheme.value === 'dark' ? 'light' : 'dark'
    setThemeMode(nextMode)
  }

  return {
    themeMode: computed(() => themeMode.value),
    resolvedTheme: computed(() => resolvedTheme.value),
    isDark: computed(() => resolvedTheme.value === 'dark'),
    setThemeMode,
    toggleTheme,
  }
}
