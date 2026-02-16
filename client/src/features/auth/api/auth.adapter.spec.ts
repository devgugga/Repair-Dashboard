import { describe, expect, it, vi } from 'vitest'

import { getApiBaseUrl } from '@shared/config/api/api.config'

import {
  getCurrentUserWithApiAuth,
  loginWithApiAuth,
  logoutWithApiAuth,
  refreshWithApiAuth,
} from './auth.adapter'

describe('auth.adapter', () => {
  it('maps login response payload and uses credentials include', async () => {
    const fetchSpy = vi.spyOn(globalThis, 'fetch').mockResolvedValue(
      new Response(
        JSON.stringify({
          accessToken: 'jwt-token',
          expiresAt: '2030-01-01T00:00:00.000Z',
          tokenType: 'Bearer',
          user: {
            id: 'u-1',
            userName: 'admin',
            email: 'admin@repair.com.br',
            role: 'admin',
            lastLogin: '2026-02-16T00:00:00.000Z',
          },
        }),
        { status: 200 },
      ),
    )

    const session = await loginWithApiAuth({
      userName: 'admin',
      password: 'secret',
      rememberMe: true,
    })

    expect(session.accessToken).toBe('jwt-token')
    expect(session.user.userName).toBe('admin')
    expect(fetchSpy).toHaveBeenCalledWith(
      `${getApiBaseUrl()}/auth/login`,
      expect.objectContaining({
        method: 'POST',
        credentials: 'include',
      }),
    )
  })

  it('accepts pascal case response payload from API', async () => {
    vi.spyOn(globalThis, 'fetch').mockResolvedValue(
      new Response(
        JSON.stringify({
          AccessToken: 'jwt-token',
          ExpiresAt: '2030-01-01T00:00:00.000Z',
          TokenType: 'Bearer',
          User: {
            Id: 'u-2',
            UserName: 'tech',
            Email: 'tech@repair.com.br',
            Role: 'technician',
            LastLogin: '2026-02-16T00:00:00.000Z',
          },
        }),
        { status: 200 },
      ),
    )

    const session = await loginWithApiAuth({
      userName: 'tech',
      password: 'secret',
      rememberMe: false,
    })

    expect(session.user.role).toBe('technician')
  })

  it('throws AuthApiError with status and traceId when login fails', async () => {
    vi.spyOn(globalThis, 'fetch').mockResolvedValue(
      new Response(
        JSON.stringify({
          status: 401,
          title: 'Authentication failed.',
          detail: 'Invalid username or password',
          traceId: 'trace-123',
          extensions: {
            errorCode: 'AUTH_FAILED',
          },
        }),
        { status: 401 },
      ),
    )

    await expect(
      loginWithApiAuth({
        userName: 'wrong',
        password: 'wrong',
        rememberMe: false,
      }),
    ).rejects.toMatchObject({
      name: 'AuthApiError',
      status: 401,
      traceId: 'trace-123',
      errorCode: 'AUTH_FAILED',
    })
  })

  it('calls refresh endpoint with credentials include', async () => {
    const fetchSpy = vi.spyOn(globalThis, 'fetch').mockResolvedValue(
      new Response(
        JSON.stringify({
          accessToken: 'jwt-refresh-token',
          expiresAt: '2030-01-01T00:00:00.000Z',
          tokenType: 'Bearer',
          user: {
            id: 'u-1',
            userName: 'admin',
            email: 'admin@repair.com.br',
            role: 'admin',
            lastLogin: '2026-02-16T00:00:00.000Z',
          },
        }),
        { status: 200 },
      ),
    )

    const session = await refreshWithApiAuth()

    expect(session.accessToken).toBe('jwt-refresh-token')
    expect(fetchSpy).toHaveBeenCalledWith(
      `${getApiBaseUrl()}/auth/refresh`,
      expect.objectContaining({
        method: 'POST',
        credentials: 'include',
      }),
    )
  })

  it('loads current user profile from /auth/me', async () => {
    const fetchSpy = vi.spyOn(globalThis, 'fetch').mockResolvedValue(
      new Response(
        JSON.stringify({
          user: {
            id: 'u-1',
            userName: 'admin',
            email: 'admin@repair.com.br',
            role: 'admin',
            lastLogin: '2026-02-16T00:00:00.000Z',
          },
          permissions: ['users.read', 'users.manage_roles'],
        }),
        { status: 200 },
      ),
    )

    const me = await getCurrentUserWithApiAuth()

    expect(me.user.userName).toBe('admin')
    expect(me.permissions).toContain('users.read')
    expect(fetchSpy).toHaveBeenCalledWith(
      `${getApiBaseUrl()}/auth/me`,
      expect.objectContaining({
        method: 'GET',
        credentials: 'include',
      }),
    )
  })

  it('calls logout endpoint and returns success flag', async () => {
    const fetchSpy = vi.spyOn(globalThis, 'fetch').mockResolvedValue(
      new Response(
        JSON.stringify({
          success: true,
        }),
        { status: 200 },
      ),
    )

    const success = await logoutWithApiAuth()

    expect(success).toBe(true)
    expect(fetchSpy).toHaveBeenCalledWith(
      `${getApiBaseUrl()}/auth/logout`,
      expect.objectContaining({
        method: 'POST',
        credentials: 'include',
      }),
    )
  })
})
