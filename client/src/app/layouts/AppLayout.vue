<script setup lang="ts">
import { computed, ref } from 'vue'
import Drawer from 'primevue/drawer'
import { RouterView, useRoute, useRouter } from 'vue-router'

import { useAuthStore } from '@features/auth/model/useAuthStore'
import DashboardHeader from '@features/dashboard-header/ui/DashboardHeader.vue'
import AppSidebar from '@features/sidebar/ui/AppSidebar.vue'
import { useTheme } from '@features/theme-toggle/model/useTheme'

const authStore = useAuthStore()
const { resolvedTheme, toggleTheme } = useTheme()
const route = useRoute()
const router = useRouter()
const isSidebarCollapsed = ref(true)
const isMobileSidebarOpen = ref(false)

const pageTitle = computed(() => route.meta.title ?? 'Dashboard')
const pageSubtitle = computed(() => route.meta.subtitle ?? '')

async function handleLogout(): Promise<void> {
  await authStore.logout()
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

function handleToggleSidebarCollapse(): void {
  isSidebarCollapsed.value = !isSidebarCollapsed.value
}

function handleOpenSidebar(): void {
  isMobileSidebarOpen.value = true
}

function handleMobileNavigate(): void {
  isMobileSidebarOpen.value = false
}
</script>

<template>
  <main class="mx-auto max-w-[1600px] px-5 py-8">
    <DashboardHeader
      :title="pageTitle"
      :subtitle="pageSubtitle"
      :user-name="authStore.displayName || 'Administrador'"
      :user-role="authStore.user?.role || 'Admin'"
      :resolved-theme="resolvedTheme"
      @open-sidebar="handleOpenSidebar"
      @open-profile="handleOpenProfile"
      @open-settings="handleOpenSettings"
      @open-notifications="handleOpenNotifications"
      @open-help="handleOpenHelp"
      @toggle-theme="toggleTheme"
      @logout="handleLogout"
    />

    <section class="grid gap-4 lg:grid-cols-[auto_minmax(0,1fr)]">
      <div
        class="hidden self-start transition-all duration-200 lg:sticky lg:top-28 lg:block"
        :class="isSidebarCollapsed ? 'w-[4.5rem]' : 'w-[18rem]'"
      >
        <AppSidebar
          :collapsed="isSidebarCollapsed"
          @toggle-collapse="handleToggleSidebarCollapse"
        />
      </div>

      <section class="min-w-0">
        <RouterView />
      </section>
    </section>

    <Drawer
      v-model:visible="isMobileSidebarOpen"
      position="left"
      class="w-[18rem]"
      :modal="true"
      :dismissable="true"
      :show-close-icon="true"
      header="Navegação"
    >
      <AppSidebar mobile @navigate="handleMobileNavigate" />
    </Drawer>
  </main>
</template>
