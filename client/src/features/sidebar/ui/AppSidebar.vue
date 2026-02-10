<script setup lang="ts">
import Button from 'primevue/button'
import { computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'

import { getSidebarSections } from '@features/sidebar/model/sidebar.menu'
import type { SidebarItem } from '@features/sidebar/model/sidebar.types'

interface AppSidebarProps {
  collapsed?: boolean
  mobile?: boolean
}

const props = withDefaults(defineProps<AppSidebarProps>(), {
  collapsed: false,
  mobile: false,
})

const emit = defineEmits<{
  (event: 'toggle-collapse'): void
  (event: 'navigate'): void
}>()

const route = useRoute()
const router = useRouter()

const sections = computed(() => getSidebarSections())

function isActiveByName(item: SidebarItem): boolean {
  return route.name === item.routeName
}

function onNavigate(): void {
  emit('navigate')
}

function handleRouteItemClick(navigate: () => void): void {
  navigate()
  onNavigate()
}

async function navigateToRoute(routeName: SidebarItem['routeName']): Promise<void> {
  await router.push({ name: routeName })
  onNavigate()
}
</script>

<template>
  <aside
    :class="[
      'rounded-2xl border border-white/35 bg-white/60 p-3 shadow-lg shadow-black/5 backdrop-blur-xl transition-all duration-200 dark:border-white/10 dark:bg-surface-900/45',
      mobile ? 'h-full overflow-y-auto' : 'max-h-[calc(100dvh-8.5rem)] overflow-y-auto',
    ]"
  >
    <div class="mb-3 flex items-center justify-between" :class="collapsed ? 'px-1' : 'px-2'">
      <span
        v-if="!collapsed"
        class="text-xs font-semibold tracking-[0.12em] text-muted-color uppercase"
      >
        Navegação
      </span>
      <Button
        v-if="!mobile"
        :icon="collapsed ? 'pi pi-angle-right' : 'pi pi-angle-left'"
        text
        rounded
        :aria-label="collapsed ? 'Expandir menu lateral' : 'Recolher menu lateral'"
        @click="emit('toggle-collapse')"
      />
    </div>

    <div v-if="collapsed && !mobile" class="space-y-3">
      <div
        v-for="section in sections"
        :key="section.key"
        class="space-y-1 border-t border-surface-200 pt-2 first:border-t-0 first:pt-0 dark:border-surface-700"
      >
        <button
          v-for="item in section.items"
          :key="item.key"
          v-tooltip.right="`${section.label}: ${item.label}`"
          type="button"
          class="mx-auto flex h-9 w-9 items-center justify-center rounded-lg transition-colors"
          :class="
            isActiveByName(item)
              ? 'bg-primary text-primary-contrast'
              : 'text-muted-color hover:bg-surface-100 dark:hover:bg-surface-800'
          "
          @click="navigateToRoute(item.routeName)"
        >
          <i :class="[item.icon, 'text-sm']" />
        </button>
      </div>
    </div>

    <div v-else class="space-y-3">
      <section
        v-for="section in sections"
        :key="section.key"
        class="rounded-xl border border-surface-200/80 bg-white/40 p-2 shadow-sm shadow-black/5 backdrop-blur-md dark:border-surface-700 dark:bg-surface-900/35"
      >
        <header
          class="mb-1.5 flex items-center gap-2 px-2 py-1 text-[11px] font-semibold tracking-[0.09em] text-muted-color uppercase"
        >
          <i :class="[section.icon, 'text-xs']" />
          <span>{{ section.label }}</span>
        </header>

        <nav class="space-y-0.5">
          <router-link
            v-for="item in section.items"
            :key="item.key"
            v-slot="{ href, navigate }"
            :to="{ name: item.routeName }"
            custom
          >
            <a
              :href="href"
              class="group flex items-center gap-2 rounded-lg px-2.5 py-2 text-sm transition-colors"
              :class="
                isActiveByName(item)
                  ? 'bg-primary/15 text-primary'
                  : 'text-color hover:bg-surface-100/70 dark:hover:bg-surface-800/70'
              "
              @click.prevent="handleRouteItemClick(navigate)"
            >
              <i :class="[item.icon, 'text-sm']" />
              <span>{{ item.label }}</span>
            </a>
          </router-link>
        </nav>
      </section>
    </div>
  </aside>
</template>
