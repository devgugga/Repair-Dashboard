import { mount } from '@vue/test-utils'
import { describe, expect, it } from 'vitest'

import App from '@app/App.vue'
import router from '@app/providers/router'

describe('App smoke', () => {
  it('renders dashboard page on root route', async () => {
    await router.push('/')
    await router.isReady()

    const wrapper = mount(App, {
      global: {
        plugins: [router],
      },
    })

    expect(wrapper.text()).toContain('Operations Overview')
  })
})
