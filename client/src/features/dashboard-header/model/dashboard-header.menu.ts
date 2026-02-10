import type { MenuItem } from 'primevue/menuitem'

interface ProfileMenuOptions {
  themeActionLabel: string
  onOpenProfile: () => void
  onOpenSettings: () => void
  onToggleTheme: () => void
  onLogout: () => void
}

interface OverflowMenuOptions {
  onOpenNotifications: () => void
  onOpenHelp: () => void
}

export function buildProfileMenuItems(options: ProfileMenuOptions): MenuItem[] {
  return [
    {
      label: 'Sistema',
      icon: 'pi pi-th-large',
      disabled: true,
    },
    {
      label: 'Meu perfil',
      icon: 'pi pi-id-card',
      command: options.onOpenProfile,
    },
    {
      label: 'Configurações',
      icon: 'pi pi-cog',
      command: options.onOpenSettings,
    },
    {
      separator: true,
    },
    {
      label: 'Aparência',
      icon: 'pi pi-palette',
      disabled: true,
    },
    {
      label: options.themeActionLabel,
      icon: 'pi pi-palette',
      command: options.onToggleTheme,
    },
    {
      separator: true,
    },
    {
      label: 'Sessão',
      icon: 'pi pi-shield',
      disabled: true,
    },
    {
      label: 'Sair',
      icon: 'pi pi-sign-out',
      command: options.onLogout,
    },
  ]
}

export function buildOverflowMenuItems(options: OverflowMenuOptions): MenuItem[] {
  return [
    {
      label: 'Notificações',
      icon: 'pi pi-bell',
      command: options.onOpenNotifications,
    },
    {
      label: 'Ajuda',
      icon: 'pi pi-question-circle',
      command: options.onOpenHelp,
    },
  ]
}
