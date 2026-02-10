import type { MenuItem } from 'primevue/menuitem'

import type { SidebarSection } from './sidebar.types'

export function getSidebarSections(): SidebarSection[] {
  return [
    {
      key: 'operacao',
      label: 'Operação',
      icon: 'pi pi-briefcase',
      items: [
        { key: 'dashboard', label: 'Dashboard', icon: 'pi pi-home', routeName: 'dashboard' },
        { key: 'customers', label: 'Clientes', icon: 'pi pi-users', routeName: 'customers' },
        { key: 'devices', label: 'Aparelhos', icon: 'pi pi-mobile', routeName: 'devices' },
        {
          key: 'service-orders',
          label: 'Ordens de Serviço',
          icon: 'pi pi-wrench',
          routeName: 'service-orders',
        },
        {
          key: 'repair-history',
          label: 'Histórico de Consertos',
          icon: 'pi pi-history',
          routeName: 'repair-history',
        },
      ],
    },
    {
      key: 'estoque',
      label: 'Estoque',
      icon: 'pi pi-box',
      items: [
        { key: 'parts', label: 'Peças', icon: 'pi pi-tag', routeName: 'parts' },
        {
          key: 'stock-movements',
          label: 'Movimentações',
          icon: 'pi pi-sort-alt',
          routeName: 'stock-movements',
        },
        {
          key: 'suppliers',
          label: 'Fornecedores',
          icon: 'pi pi-truck',
          routeName: 'suppliers',
        },
      ],
    },
    {
      key: 'financeiro',
      label: 'Financeiro',
      icon: 'pi pi-wallet',
      items: [
        { key: 'cashflow', label: 'Caixa', icon: 'pi pi-money-bill', routeName: 'cashflow' },
        {
          key: 'receivables',
          label: 'Contas a Receber',
          icon: 'pi pi-credit-card',
          routeName: 'receivables',
        },
        {
          key: 'finance-reports',
          label: 'Relatórios Financeiros',
          icon: 'pi pi-chart-line',
          routeName: 'finance-reports',
        },
      ],
    },
    {
      key: 'config',
      label: 'Configurações',
      icon: 'pi pi-cog',
      items: [
        {
          key: 'users-permissions',
          label: 'Usuários e Permissões',
          icon: 'pi pi-user-edit',
          routeName: 'users-permissions',
        },
        {
          key: 'system-settings',
          label: 'Parâmetros do Sistema',
          icon: 'pi pi-sliders-h',
          routeName: 'system-settings',
        },
      ],
    },
  ]
}

export function buildPanelMenuModel(sections: SidebarSection[]): MenuItem[] {
  return sections.map((section) => ({
    key: section.key,
    label: section.label,
    icon: section.icon,
    items: section.items.map((item) => ({
      key: item.key,
      label: item.label,
      icon: item.icon,
      route: { name: item.routeName },
      badge: item.badge,
    })),
  }))
}
