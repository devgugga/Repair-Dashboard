<script setup lang="ts">
import Avatar from 'primevue/avatar'
import Badge from 'primevue/badge'
import Button from 'primevue/button'
import Menu from 'primevue/menu'
import type { MenuItem } from 'primevue/menuitem'
import { computed, ref } from 'vue'

import {
  buildOverflowMenuItems,
  buildProfileMenuItems,
} from '@features/dashboard-header/model/dashboard-header.menu'
import type {
  DashboardHeaderEmits,
  DashboardHeaderProps,
} from '@features/dashboard-header/model/dashboard-header.types'
import { isDashboardHeaderCategory } from '@features/dashboard-header/model/dashboard-header.types'

const props = defineProps<DashboardHeaderProps>()

const emit = defineEmits<DashboardHeaderEmits>()

const profileMenuRef = ref<InstanceType<typeof Menu> | null>(null)
const overflowMenuRef = ref<InstanceType<typeof Menu> | null>(null)

const userInitials = computed(() => {
  const [first = '', second = ''] = props.userName.trim().split(/\s+/, 2)
  return `${first.charAt(0)}${second.charAt(0)}`.toUpperCase() || 'U'
})

const themeActionLabel = computed(() =>
  props.resolvedTheme === 'dark' ? 'Ativar tema claro' : 'Ativar tema escuro',
)

const profileMenuItems = computed<MenuItem[]>(() =>
  buildProfileMenuItems({
    themeActionLabel: themeActionLabel.value,
    onOpenProfile: () => emit('open-profile'),
    onOpenSettings: () => emit('open-settings'),
    onToggleTheme: () => emit('toggle-theme'),
    onLogout: () => emit('logout'),
  }),
)

const overflowMenuItems = computed<MenuItem[]>(() =>
  buildOverflowMenuItems({
    onOpenNotifications: () => emit('open-notifications'),
    onOpenHelp: () => emit('open-help'),
  }),
)

function toggleProfileMenu(event: Event): void {
  profileMenuRef.value?.toggle(event)
}

function toggleOverflowMenu(event: Event): void {
  overflowMenuRef.value?.toggle(event)
}

</script>

<template>
  <header class="sticky top-4 z-20 mb-6">
    <div
      class="rounded-2xl border border-white/40 bg-white/60 px-4 py-3 shadow-lg shadow-black/5 backdrop-blur-xl dark:border-white/10 dark:bg-surface-900/45"
    >
      <div class="flex items-center justify-between gap-3">
        <div class="min-w-0">
          <p class="truncate text-[11px] font-semibold tracking-[0.14em] text-primary uppercase">
            Repair Control
          </p>
          <h1 class="truncate text-xl font-semibold tracking-tight sm:text-2xl">{{ title }}</h1>
          <p v-if="subtitle" class="truncate text-xs text-muted-color sm:text-sm">{{ subtitle }}</p>
        </div>

        <div class="hidden items-center gap-2 md:flex">
          <div class="relative">
            <Button
              icon="pi pi-bell"
              text
              rounded
              aria-label="Abrir notificações"
              @click="emit('open-notifications')"
            />
            <Badge value="3" class="pointer-events-none absolute -top-1 -right-1 scale-75" severity="danger" />
          </div>

          <Button
            icon="pi pi-question-circle"
            text
            rounded
            aria-label="Abrir ajuda"
            @click="emit('open-help')"
          />

          <span class="h-6 w-px bg-surface-300 dark:bg-surface-700" aria-hidden="true" />

          <button
            type="button"
            class="inline-flex items-center gap-2 rounded-xl border border-surface-200/80 bg-white/70 px-2 py-1 transition-colors hover:bg-white dark:border-surface-700/80 dark:bg-surface-900/70 dark:hover:bg-surface-800"
            aria-label="Abrir menu do usuário"
            @click="toggleProfileMenu"
          >
            <Avatar
              :label="userInitials"
              size="small"
              shape="circle"
              class="bg-primary text-primary-contrast"
              :pt="{ label: { class: 'leading-none' } }"
            />
            <span class="hidden text-left lg:inline">
              <span class="block max-w-28 truncate text-sm font-semibold">{{ userName }}</span>
              <span class="block text-xs text-muted-color">{{ userRole }}</span>
            </span>
            <i class="pi pi-angle-down text-xs text-muted-color" />
          </button>
        </div>

        <div class="flex items-center gap-1 md:hidden">
          <Button
            icon="pi pi-ellipsis-v"
            text
            rounded
            aria-label="Abrir ações"
            @click="toggleOverflowMenu"
          />
          <button
            type="button"
            class="inline-flex items-center rounded-xl border border-surface-200/80 bg-white/70 px-2 py-1 transition-colors hover:bg-white dark:border-surface-700/80 dark:bg-surface-900/70 dark:hover:bg-surface-800"
            aria-label="Abrir menu do usuário"
            @click="toggleProfileMenu"
          >
            <Avatar
              :label="userInitials"
              size="small"
              shape="circle"
              class="bg-primary text-primary-contrast"
              :pt="{ label: { class: 'leading-none' } }"
            />
          </button>
        </div>
      </div>
    </div>

    <Menu ref="profileMenuRef" popup :model="profileMenuItems" class="w-72">
      <template #item="{ item, props: itemProps }">
        <a
          v-bind="itemProps.action"
          :class="[
            'flex items-center gap-2 rounded-lg transition-colors',
            isDashboardHeaderCategory(item)
              ? 'pointer-events-none px-2 py-1.5 text-[11px] font-semibold tracking-[0.08em] text-muted-color uppercase'
              : 'ml-4 px-2 py-2 text-sm',
            item.disabled && !isDashboardHeaderCategory(item) ? 'opacity-60' : '',
          ]"
        >
          <span
            v-if="item.icon"
            :class="[item.icon, isDashboardHeaderCategory(item) ? 'text-xs text-muted-color' : 'text-sm']"
          />
          <span>{{ item.label }}</span>
        </a>
      </template>
    </Menu>
    <Menu ref="overflowMenuRef" popup :model="overflowMenuItems" class="w-48" />
  </header>
</template>
