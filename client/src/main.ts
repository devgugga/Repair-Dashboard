import '@app/styles/main.css'
import 'primeicons/primeicons.css'

import { createApp } from 'vue'
import PrimeVue from 'primevue/config'
import Tooltip from 'primevue/tooltip'
import Aura from '@primeuix/themes/aura'
import { definePreset } from '@primeuix/themes'

import App from '@app/App.vue'
import pinia from '@app/providers/pinia'
import router from '@app/providers/router'
import { useAuthStore } from '@features/auth/model/useAuthStore'
import { configureApiClientAuth } from '@shared/api/apiClient'
import { initTheme } from '@shared/config/theme/theme.service'

const AppPreset = definePreset(Aura, {
  semantic: {
    primary: {
      50: '{indigo.50}',
      100: '{indigo.100}',
      200: '{indigo.200}',
      300: '{indigo.300}',
      400: '{indigo.400}',
      500: '{indigo.500}',
      600: '{indigo.600}',
      700: '{indigo.700}',
      800: '{indigo.800}',
      900: '{indigo.900}',
      950: '{indigo.950}',
    },
  },
})

initTheme()

const app = createApp(App)

app.use(pinia)
const authStore = useAuthStore(pinia)
configureApiClientAuth({
  getAuthorizationHeader: () => authStore.authorizationHeader,
  refreshSession: () => authStore.refreshSession(),
  onUnauthorized: () => authStore.clearSession(),
})
app.use(router)
app.use(PrimeVue, {
  theme: {
    preset: AppPreset,
    options: {
      darkModeSelector: '[data-theme="dark"]',
      cssLayer: {
        name: 'primevue',
        order: 'theme, base, primevue',
      },
    },
  },
})
app.directive('tooltip', Tooltip)

app.mount('#app')
