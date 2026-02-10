<script setup lang="ts">
import Button from 'primevue/button'
import Card from 'primevue/card'
import Column from 'primevue/column'
import DataTable from 'primevue/datatable'
import Tag from 'primevue/tag'
import { computed } from 'vue'

import { useTheme } from '@features/theme-toggle/model/useTheme'

type TicketStatus = 'Open' | 'In Progress' | 'Resolved'
type TicketSeverity = 'Low' | 'Medium' | 'High'

interface Ticket {
  id: string
  asset: string
  status: TicketStatus
  severity: TicketSeverity
  owner: string
}

const kpis = [
  { label: 'Open Tickets', value: '18', trend: '+4 today' },
  { label: 'SLA Compliance', value: '97.6%', trend: '+0.8%' },
  { label: 'Pending Parts', value: '6', trend: '-2 today' },
  { label: 'Completed This Week', value: '42', trend: '+11%' },
]

const tickets: Ticket[] = [
  {
    id: 'TCK-2081',
    asset: 'POS Terminal - Store 12',
    status: 'Open',
    severity: 'High',
    owner: 'Ana M.',
  },
  {
    id: 'TCK-2079',
    asset: 'Barcode Scanner - Store 08',
    status: 'In Progress',
    severity: 'Medium',
    owner: 'Lucas R.',
  },
  {
    id: 'TCK-2076',
    asset: 'Router - Warehouse',
    status: 'Resolved',
    severity: 'Low',
    owner: 'Maria C.',
  },
]

const statusSeverityMap: Record<TicketStatus, 'danger' | 'warn' | 'success'> = {
  Open: 'danger',
  'In Progress': 'warn',
  Resolved: 'success',
}

const impactSeverityMap: Record<TicketSeverity, 'danger' | 'warn' | 'secondary'> = {
  High: 'danger',
  Medium: 'warn',
  Low: 'secondary',
}

function getStatusSeverity(value: unknown): 'danger' | 'warn' | 'success' {
  const normalized = String(value) as TicketStatus
  return statusSeverityMap[normalized] ?? 'warn'
}

function getImpactSeverity(value: unknown): 'danger' | 'warn' | 'secondary' {
  const normalized = String(value) as TicketSeverity
  return impactSeverityMap[normalized] ?? 'secondary'
}

const { themeMode, resolvedTheme, toggleTheme, setThemeMode } = useTheme()

const themeButtonLabel = computed(() =>
  resolvedTheme.value === 'dark' ? 'Switch to Light' : 'Switch to Dark',
)
</script>

<template>
  <main class="dashboard-page">
    <section class="dashboard-header">
      <div>
        <p class="eyebrow">Repair Dashboard</p>
        <h1>Operations Overview</h1>
        <p class="theme-state">
          Theme: {{ resolvedTheme }} <span v-if="themeMode === 'system'">(following system)</span>
        </p>
      </div>
      <div class="header-actions">
        <Button :label="themeButtonLabel" severity="secondary" outlined @click="toggleTheme" />
        <Button
          label="Use System Theme"
          severity="contrast"
          text
          :disabled="themeMode === 'system'"
          @click="setThemeMode('system')"
        />
        <Button label="New Ticket" />
        <Button label="Export Report" severity="secondary" outlined />
      </div>
    </section>

    <section class="kpi-grid">
      <Card v-for="item in kpis" :key="item.label">
        <template #content>
          <p class="kpi-label">{{ item.label }}</p>
          <p class="kpi-value">{{ item.value }}</p>
          <p class="kpi-trend">{{ item.trend }}</p>
        </template>
      </Card>
    </section>

    <section>
      <Card>
        <template #title>Recent Tickets</template>
        <template #content>
          <DataTable :value="tickets" striped-rows size="small">
            <Column field="id" header="Ticket" />
            <Column field="asset" header="Asset" />
            <Column header="Status">
              <template #body="{ data }">
                <Tag :value="data.status" :severity="getStatusSeverity(data.status)" />
              </template>
            </Column>
            <Column header="Severity">
              <template #body="{ data }">
                <Tag :value="data.severity" :severity="getImpactSeverity(data.severity)" />
              </template>
            </Column>
            <Column field="owner" header="Owner" />
          </DataTable>
        </template>
      </Card>
    </section>
  </main>
</template>
