import { mount } from '@vue/test-utils'
import { afterEach, describe, expect, it } from 'vitest'
import PrimeVue from 'primevue/config'
import { nextTick } from 'vue'

import DashboardHeader from './DashboardHeader.vue'

const baseProps = {
  title: 'Operations Overview',
  subtitle: 'Gestão completa de clientes e aparelhos.',
  userName: 'Administrador',
  userRole: 'Admin',
  resolvedTheme: 'dark' as const,
}

function mountHeader() {
  return mount(DashboardHeader, {
    attachTo: document.body,
    props: baseProps,
    global: {
      plugins: [
        [
          PrimeVue,
          {
            ripple: true,
          },
        ],
      ],
    },
  })
}

async function clickDocumentMenuItem(label: string): Promise<void> {
  await nextTick()
  const allLinks = Array.from(document.body.querySelectorAll('a'))
  const target = allLinks.find((node) => node.textContent?.includes(label))

  if (!target) {
    throw new Error(`Menu item "${label}" not found`)
  }

  target.dispatchEvent(new MouseEvent('click', { bubbles: true }))
  await nextTick()
}

describe.sequential('DashboardHeader', () => {
  afterEach(() => {
    document.body.innerHTML = ''
  })

  it('renders menu categories when profile menu opens', async () => {
    const wrapper = mountHeader()
    const profileButtons = wrapper.findAll('button[aria-label="Abrir menu do usuário"]')

    await profileButtons[0]?.trigger('click')
    await nextTick()

    expect(document.body.textContent).toContain('Sistema')
    expect(document.body.textContent).toContain('Aparência')
    expect(document.body.textContent).toContain('Sessão')
  })

  it('emits profile-related events from profile menu items', async () => {
    const wrapper = mountHeader()
    const profileButtons = wrapper.findAll('button[aria-label="Abrir menu do usuário"]')

    await profileButtons[0]?.trigger('click')
    await clickDocumentMenuItem('Meu perfil')
    expect(wrapper.emitted('open-profile')).toHaveLength(1)

    await profileButtons[0]?.trigger('click')
    await clickDocumentMenuItem('Configurações')
    expect(wrapper.emitted('open-settings')).toHaveLength(1)

    await profileButtons[0]?.trigger('click')
    await clickDocumentMenuItem('Ativar tema claro')
    expect(wrapper.emitted('toggle-theme')).toHaveLength(1)

    await profileButtons[0]?.trigger('click')
    await clickDocumentMenuItem('Sair')
    expect(wrapper.emitted('logout')).toHaveLength(1)
  })

  it('emits overflow actions from mobile/overflow menu', async () => {
    const wrapper = mountHeader()
    const overflowButton = wrapper.find('button[aria-label="Abrir ações"]')

    await overflowButton.trigger('click')
    await clickDocumentMenuItem('Notificações')
    expect(wrapper.emitted('open-notifications')).toHaveLength(1)

    await overflowButton.trigger('click')
    await clickDocumentMenuItem('Ajuda')
    expect(wrapper.emitted('open-help')).toHaveLength(1)
  })
})
