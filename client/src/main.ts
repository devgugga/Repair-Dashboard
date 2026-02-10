import '@app/styles/main.css'

import { createApp } from 'vue'
import PrimeVue from 'primevue/config'
import Aura from '@primeuix/themes/aura'

import App from '@app/App.vue'
import router from '@app/providers/router'
import { initTheme } from '@shared/config/theme/theme.service'

initTheme()

const app = createApp(App)

app.use(router)
app.use(PrimeVue, {
  theme: {
    preset: Aura,
    options: {
      darkModeSelector: '[data-theme="dark"]',
    },
  },
})

app.mount('#app')
