import { config } from '@vue/test-utils'
import Tooltip from 'primevue/tooltip'
import { afterEach, vi } from 'vitest'

config.global.directives = {
  ...config.global.directives,
  tooltip: Tooltip,
}

afterEach(() => {
  vi.restoreAllMocks()
})
