<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()

const title = computed(() => route.meta.title ?? 'Em construção')
const subtitle = computed(() => route.meta.subtitle ?? 'Conteúdo desta área será adicionado em breve.')
</script>

<template>
  <section class="rounded-2xl border border-surface-200/80 bg-white/70 p-6 dark:border-surface-700 dark:bg-surface-900/45">
    <h2 class="text-xl font-semibold tracking-tight">{{ title }}</h2>
    <p class="mt-2 text-sm text-muted-color">{{ subtitle }}</p>
  </section>
</template>
