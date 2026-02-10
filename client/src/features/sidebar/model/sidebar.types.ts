export type SidebarSectionKey = 'operacao' | 'estoque' | 'financeiro' | 'config'

export type SidebarRouteName =
  | 'dashboard'
  | 'customers'
  | 'devices'
  | 'service-orders'
  | 'repair-history'
  | 'parts'
  | 'stock-movements'
  | 'suppliers'
  | 'cashflow'
  | 'receivables'
  | 'finance-reports'
  | 'users-permissions'
  | 'system-settings'

export interface SidebarItem {
  key: string
  label: string
  icon: string
  routeName: SidebarRouteName
  badge?: string
}

export interface SidebarSection {
  key: SidebarSectionKey
  label: string
  icon: string
  items: SidebarItem[]
}
