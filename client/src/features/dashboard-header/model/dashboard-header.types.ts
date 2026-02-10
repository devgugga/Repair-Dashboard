import type { MenuItem } from 'primevue/menuitem'

export type ResolvedThemeMode = 'light' | 'dark'

export interface DashboardHeaderProps {
  title: string
  subtitle?: string
  userName: string
  userRole: string
  resolvedTheme: ResolvedThemeMode
}

export type DashboardHeaderEmits = {
  (event: 'open-profile'): void
  (event: 'open-settings'): void
  (event: 'open-notifications'): void
  (event: 'open-help'): void
  (event: 'toggle-theme'): void
  (event: 'logout'): void
}

export type DashboardHeaderCategory = 'Sistema' | 'Aparência' | 'Sessão'

export function isDashboardHeaderCategory(item: MenuItem): boolean {
  return item.label === 'Sistema' || item.label === 'Aparência' || item.label === 'Sessão'
}
