export type TicketStatus = 'Open' | 'In Progress' | 'Resolved'
export type TicketSeverity = 'Low' | 'Medium' | 'High'

export interface Ticket {
  id: string
  asset: string
  status: TicketStatus
  severity: TicketSeverity
  owner: string
}

export interface DashboardKpi {
  label: string
  value: string
  trend: string
}

export type TicketStatusTagSeverity = 'danger' | 'warn' | 'success'
export type TicketImpactTagSeverity = 'danger' | 'warn' | 'secondary'
