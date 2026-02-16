import type {
  AuthAdapter,
  AuthCurrentUser,
  AuthSession,
  LoginInput,
} from '@features/auth/model/auth.types'
import { ApiClientError, requestJson } from '@shared/api/apiClient'

interface ApiErrorResponse {
  status?: unknown
  title?: unknown
  detail?: unknown
  traceId?: unknown
  extensions?: unknown
}

interface AuthUserPayload {
  id?: unknown
  Id?: unknown
  userName?: unknown
  UserName?: unknown
  email?: unknown
  Email?: unknown
  role?: unknown
  Role?: unknown
  lastLogin?: unknown
  LastLogin?: unknown
}

interface AuthSessionPayload {
  accessToken?: unknown
  AccessToken?: unknown
  expiresAt?: unknown
  ExpiresAt?: unknown
  tokenType?: unknown
  TokenType?: unknown
  user?: AuthUserPayload
  User?: AuthUserPayload
}

interface AuthMePayload {
  user?: AuthUserPayload
  User?: AuthUserPayload
  permissions?: unknown
  Permissions?: unknown
}

interface LogoutPayload {
  success?: unknown
}

interface AuthApiErrorParams {
  endpoint: string
  status?: number
  title?: string
  detail?: string
  traceId?: string
  errorCode?: string
  errors?: Record<string, string[]>
}

export class AuthApiError extends Error {
  endpoint: string
  status?: number
  detail?: string
  traceId?: string
  errorCode?: string
  errors?: Record<string, string[]>

  constructor(message: string, params: AuthApiErrorParams) {
    super(message)
    this.name = 'AuthApiError'
    this.endpoint = params.endpoint
    this.status = params.status
    this.detail = params.detail
    this.traceId = params.traceId
    this.errorCode = params.errorCode
    this.errors = params.errors
  }
}

function parseObject(value: unknown): Record<string, unknown> | null {
  if (!value || typeof value !== 'object' || Array.isArray(value)) {
    return null
  }

  return value as Record<string, unknown>
}

function parseString(value: unknown): string | undefined {
  return typeof value === 'string' && value.length > 0 ? value : undefined
}

function parseStatus(value: unknown): number | undefined {
  return typeof value === 'number' ? value : undefined
}

function parseErrors(value: unknown): Record<string, string[]> | undefined {
  const asObject = parseObject(value)
  if (!asObject) {
    return undefined
  }

  const parsed: Record<string, string[]> = {}
  for (const [key, entry] of Object.entries(asObject)) {
    if (!Array.isArray(entry)) {
      continue
    }

    const messages = entry.filter((item): item is string => typeof item === 'string')
    if (messages.length > 0) {
      parsed[key] = messages
    }
  }

  return Object.keys(parsed).length > 0 ? parsed : undefined
}

function getErrorCode(extensions: unknown): string | undefined {
  const asObject = parseObject(extensions)
  if (!asObject) {
    return undefined
  }

  return parseString(asObject.errorCode)
}

function getValidationErrors(extensions: unknown): Record<string, string[]> | undefined {
  const asObject = parseObject(extensions)
  if (!asObject) {
    return undefined
  }

  return parseErrors(asObject.errors)
}

function normalizeApiError(error: ApiClientError): AuthApiErrorParams {
  const payload = error.data as ApiErrorResponse | undefined

  return {
    endpoint: error.endpoint,
    status: error.status ?? parseStatus(payload?.status),
    title: parseString(payload?.title),
    detail: parseString(payload?.detail),
    traceId: parseString(payload?.traceId),
    errorCode: getErrorCode(payload?.extensions),
    errors: getValidationErrors(payload?.extensions),
  }
}

function logAuthError(context: string, params: AuthApiErrorParams): void {
  console.error('[auth]', context, {
    endpoint: params.endpoint,
    status: params.status,
    title: params.title,
    detail: params.detail,
    traceId: params.traceId,
    errorCode: params.errorCode,
    errors: params.errors,
  })
}

function toAuthApiError(context: string, error: unknown): never {
  if (error instanceof AuthApiError) {
    throw error
  }

  if (error instanceof ApiClientError) {
    const normalized = normalizeApiError(error)
    logAuthError(context, normalized)
    throw new AuthApiError(normalized.title ?? 'Auth request failed.', normalized)
  }

  const fallback: AuthApiErrorParams = {
    endpoint: 'unknown',
    title: 'NetworkError',
    detail: 'Failed to reach authentication API',
  }
  logAuthError(context, fallback)
  throw new AuthApiError('Falha de conexão ao autenticar.', fallback)
}

function mapAuthUser(payload: AuthUserPayload | undefined): AuthSession['user'] | null {
  const id = parseString(payload?.id ?? payload?.Id)
  const userName = parseString(payload?.userName ?? payload?.UserName)
  const email = parseString(payload?.email ?? payload?.Email)
  const role = parseString(payload?.role ?? payload?.Role)
  const lastLogin = parseString(payload?.lastLogin ?? payload?.LastLogin)

  if (!id || !userName || !email || !role || !lastLogin) {
    return null
  }

  return {
    id,
    userName,
    email,
    role,
    lastLogin,
  }
}

function mapAuthSession(payload: AuthSessionPayload): AuthSession | null {
  const user = mapAuthUser(payload.user ?? payload.User)
  const accessToken = parseString(payload.accessToken ?? payload.AccessToken)
  const expiresAt = parseString(payload.expiresAt ?? payload.ExpiresAt)
  const tokenType = parseString(payload.tokenType ?? payload.TokenType)

  if (!user || !accessToken || !expiresAt || !tokenType) {
    return null
  }

  return {
    user,
    accessToken,
    expiresAt,
    tokenType,
  }
}

function mapCurrentUser(payload: AuthMePayload): AuthCurrentUser | null {
  const user = mapAuthUser(payload.user ?? payload.User)
  const permissionsRaw = payload.permissions ?? payload.Permissions

  if (!user || !Array.isArray(permissionsRaw)) {
    return null
  }

  const permissions = permissionsRaw.filter((item): item is string => typeof item === 'string')

  return {
    user,
    permissions,
  }
}

function throwInvalidPayloadError(endpoint: string): never {
  const params: AuthApiErrorParams = {
    endpoint,
    title: 'InvalidAuthPayload',
    detail: 'Authentication payload is missing required fields',
  }
  logAuthError('invalid_success_payload', params)
  throw new AuthApiError('Resposta de autenticação inválida.', params)
}

export async function loginWithApiAuth(input: LoginInput): Promise<AuthSession> {
  const endpoint = '/auth/login'
  try {
    const payload = await requestJson<AuthSessionPayload>(endpoint, {
      method: 'POST',
      body: {
        userName: input.userName.trim(),
        password: input.password,
        rememberMe: input.rememberMe,
      },
    })

    const session = mapAuthSession(payload)
    if (!session) {
      return throwInvalidPayloadError(endpoint)
    }

    return session
  } catch (error) {
    return toAuthApiError('login_failed', error)
  }
}

export async function refreshWithApiAuth(): Promise<AuthSession> {
  const endpoint = '/auth/refresh'

  try {
    const payload = await requestJson<AuthSessionPayload>(endpoint, {
      method: 'POST',
      retryOnUnauthorized: false,
    })

    const session = mapAuthSession(payload)
    if (!session) {
      return throwInvalidPayloadError(endpoint)
    }

    return session
  } catch (error) {
    return toAuthApiError('refresh_failed', error)
  }
}

export async function getCurrentUserWithApiAuth(): Promise<AuthCurrentUser> {
  const endpoint = '/auth/me'

  try {
    const payload = await requestJson<AuthMePayload>(endpoint, {
      method: 'GET',
      authenticated: true,
    })

    const currentUser = mapCurrentUser(payload)
    if (!currentUser) {
      return throwInvalidPayloadError(endpoint)
    }

    return currentUser
  } catch (error) {
    return toAuthApiError('me_failed', error)
  }
}

export async function logoutWithApiAuth(): Promise<boolean> {
  const endpoint = '/auth/logout'

  try {
    const payload = await requestJson<LogoutPayload>(endpoint, {
      method: 'POST',
      authenticated: true,
    })
    return payload.success === true
  } catch (error) {
    return toAuthApiError('logout_failed', error)
  }
}

export const authAdapter: AuthAdapter = {
  login: loginWithApiAuth,
  refresh: refreshWithApiAuth,
  me: getCurrentUserWithApiAuth,
  logout: logoutWithApiAuth,
}
