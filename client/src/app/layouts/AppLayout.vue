<script setup lang="ts">
import { computed } from 'vue'
import { RouterView, useRoute, useRouter } from 'vue-router'

import { useAuthStore } from '@features/auth/model/useAuthStore'
import DashboardHeader from '@features/dashboard-header/ui/DashboardHeader.vue'
import { useTheme } from '@features/theme-toggle/model/useTheme'

const authStore = useAuthStore()
const { resolvedTheme, toggleTheme } = useTheme()
const route = useRoute()
const router = useRouter()

const pageTitle = computed(() => route.meta.title ?? 'Dashboard')
const pageSubtitle = computed(() => route.meta.subtitle ?? '')

async function handleLogout(): Promise<void> {
  authStore.logout()
  await router.replace('/login')
}

function handleOpenProfile(): void {
  console.warn('TODO: abrir página de perfil.')
}

function handleOpenSettings(): void {
  console.warn('TODO: abrir página de configurações.')
}

function handleOpenNotifications(): void {
  console.warn('TODO: abrir central de notificações.')
}

function handleOpenHelp(): void {
  console.warn('TODO: abrir central de ajuda.')
}
</script>

<template>
  <main class="mx-auto max-w-7xl px-5 py-8">
    <DashboardHeader
      :title="pageTitle"
      :subtitle="pageSubtitle"
      :user-name="authStore.displayName || 'Administrador'"
      user-role="Admin"
      :resolved-theme="resolvedTheme"
      @open-profile="handleOpenProfile"
      @open-settings="handleOpenSettings"
      @open-notifications="handleOpenNotifications"
      @open-help="handleOpenHelp"
      @toggle-theme="toggleTheme"
      @logout="handleLogout"
    />

    <RouterView />
  </main>
</template>
