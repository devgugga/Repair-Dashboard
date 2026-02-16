<script setup lang="ts">
import Button from 'primevue/button'
import Card from 'primevue/card'
import Checkbox from 'primevue/checkbox'
import InputText from 'primevue/inputtext'
import Message from 'primevue/message'
import Password from 'primevue/password'
import { computed, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'

import { useAuthStore } from '@features/auth/model/useAuthStore'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

const userName = ref('')
const password = ref('')
const rememberMe = ref(false)
const errorMessage = ref('')

const isLoading = computed(() => authStore.status === 'loading')

function resolveRedirectTarget(): string {
  const redirectParam = route.query.redirect

  if (typeof redirectParam === 'string' && redirectParam.startsWith('/')) {
    return redirectParam
  }

  return '/'
}

async function handleSubmit(): Promise<void> {
  errorMessage.value = ''

  if (!userName.value.trim() || !password.value) {
    errorMessage.value = 'Informe usuário e senha para continuar.'
    return
  }

  try {
    await authStore.login({
      userName: userName.value.trim(),
      password: password.value,
      rememberMe: rememberMe.value,
    })

    await router.replace(resolveRedirectTarget())
  } catch (error) {
    errorMessage.value =
      error instanceof Error ? error.message : 'Não foi possível entrar no sistema. Tente novamente.'
  }
}
</script>

<template>
  <main
    class="relative min-h-screen overflow-hidden bg-linear-to-br from-surface-100 via-surface-50 to-surface-100 px-5 py-8 dark:from-surface-950 dark:via-surface-900 dark:to-surface-950"
  >
    <div
      class="absolute -top-32 -left-16 -z-10 h-64 w-64 rounded-full bg-primary-300/25 blur-3xl dark:bg-primary-500/20"
    />
    <div
      class="absolute -top-20 -right-20 -z-10 h-72 w-72 rounded-full bg-primary-500/20 blur-3xl dark:bg-primary-700/30"
    />
    <div
      class="absolute -bottom-36 left-1/2 -z-10 h-80 w-80 -translate-x-1/2 rounded-full bg-surface-200/80 blur-3xl dark:bg-surface-800/50"
    />

    <section class="mx-auto grid min-h-[calc(100vh-4rem)] w-full max-w-6xl items-center gap-8 lg:grid-cols-2 lg:grid-rows-1">
      <div class="text-center lg:text-left">
        <p class="mb-2 text-xs font-medium tracking-[0.16em] text-muted-color uppercase">
          Repair Dashboard
        </p>
        <h1 class="text-balance text-4xl leading-tight font-semibold tracking-tight sm:text-5xl">
          Acesse sua conta
        </h1>
        <p class="mx-auto mt-4 max-w-[42ch] text-sm leading-relaxed text-muted-color lg:mx-0">
          Controle chamados, ativos e indicadores com uma experiência clara e eficiente.
        </p>

        <div
          class="mx-auto mt-6 w-fit rounded-full border border-white/40 bg-white/30 px-4 py-2 text-xs text-color/80 backdrop-blur-md dark:border-white/15 dark:bg-white/5"
        >
          Ambiente seguro • Sessão persistente opcional
        </div>
      </div>

      <Card
        class="border border-white/45 bg-white/55 shadow-2xl shadow-surface-900/10 backdrop-blur-xl dark:border-white/10 dark:bg-surface-900/45 dark:shadow-black/25"
      >
        <template #title>Entrar</template>
        <template #subtitle>Use suas credenciais para continuar.</template>
        <template #content>
          <form class="mt-1 grid gap-4" @submit.prevent="handleSubmit">
            <label class="grid gap-2 text-sm text-muted-color">
              <span class="font-semibold text-color">Usuário</span>
              <InputText
                v-model="userName"
                class="w-full rounded-xl!"
                type="text"
                autocomplete="username"
                placeholder="Digite seu usuário"
              />
            </label>

            <label class="grid gap-2 text-sm text-muted-color">
              <span class="font-semibold text-color">Senha</span>
              <Password
                v-model="password"
                :feedback="false"
                class="w-full"
                toggle-mask
                fluid
                input-class="w-full !rounded-xl"
                autocomplete="current-password"
                placeholder="Digite sua senha"
              />
            </label>

            <div class="flex items-center justify-between">
              <label class="inline-flex items-center gap-2 text-sm text-muted-color">
                <Checkbox v-model="rememberMe" input-id="rememberMe" binary />
                <span>Lembrar-me</span>
              </label>
            </div>

            <Message v-if="errorMessage" severity="error" size="small" variant="simple">
              {{ errorMessage }}
            </Message>

            <Button
              type="submit"
              label="Entrar"
              class="w-full rounded-xl!"
              :loading="isLoading"
              :disabled="isLoading"
            />
          </form>
        </template>
      </Card>

      <p class="text-center text-xs text-muted-color lg:col-span-2">
        Entre com suas credenciais do sistema oficial.
      </p>
    </section>
  </main>
</template>
