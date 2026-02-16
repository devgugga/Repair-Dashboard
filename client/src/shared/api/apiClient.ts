import { getApiBaseUrl } from '@shared/config/api/api.config'

interface ApiClientAuthConfig {
  getAuthorizationHeader: () => string | null
  refreshSession: () => Promise<boolean>
  onUnauthorized: () => void
}

interface ApiRequestOptions extends Omit<RequestInit, 'body'> {
  authenticated?: boolean
  retryOnUnauthorized?: boolean
  body?: BodyInit | Record<string, unknown> | null
}

export class ApiClientError extends Error {
  endpoint: string
  status?: number
  data?: unknown

  constructor(message: string, endpoint: string, status?: number, data?: unknown) {
    super(message)
    this.name = 'ApiClientError'
    this.endpoint = endpoint
    this.status = status
    this.data = data
  }
}

let authConfig: ApiClientAuthConfig | null = null
let refreshPromise: Promise<boolean> | null = null

export function configureApiClientAuth(config: ApiClientAuthConfig): void {
  authConfig = config
}

function resolveUrl(path: string): string {
  if (/^https?:\/\//.test(path)) {
    return path
  }

  const normalizedPath = path.startsWith('/') ? path : `/${path}`
  return `${getApiBaseUrl()}${normalizedPath}`
}

function toHeaders(initHeaders?: HeadersInit): Headers {
  return new Headers(initHeaders ?? {})
}

function serializeBody(body: ApiRequestOptions['body'], headers: Headers): BodyInit | undefined {
  if (body == null) {
    return undefined
  }

  if (
    typeof body === 'string' ||
    body instanceof Blob ||
    body instanceof FormData ||
    body instanceof URLSearchParams ||
    body instanceof ArrayBuffer
  ) {
    return body
  }

  if (!headers.has('Content-Type')) {
    headers.set('Content-Type', 'application/json')
  }

  return JSON.stringify(body)
}

async function parseResponseData(response: Response): Promise<unknown> {
  const rawText = await response.text()

  if (!rawText) {
    return null
  }

  try {
    return JSON.parse(rawText)
  } catch {
    return rawText
  }
}

function shouldSkipRefresh(endpoint: string): boolean {
  return endpoint.endsWith('/auth/refresh')
}

async function refreshAccessToken(): Promise<boolean> {
  if (!authConfig) {
    return false
  }

  if (!refreshPromise) {
    refreshPromise = authConfig.refreshSession().finally(() => {
      refreshPromise = null
    })
  }

  return refreshPromise
}

async function doFetch(
  endpoint: string,
  options: ApiRequestOptions,
  hasRetried: boolean,
): Promise<Response> {
  const headers = toHeaders(options.headers)
  const authenticated = options.authenticated ?? false

  if (authenticated && authConfig) {
    const authorization = authConfig.getAuthorizationHeader()
    if (authorization) {
      headers.set('Authorization', authorization)
    }
  }

  const response = await fetch(endpoint, {
    method: options.method ?? 'GET',
    credentials: options.credentials ?? 'include',
    ...options,
    headers,
    body: serializeBody(options.body, headers),
  })

  const shouldRetry =
    response.status === 401 &&
    authenticated &&
    !hasRetried &&
    (options.retryOnUnauthorized ?? true) &&
    !shouldSkipRefresh(endpoint)

  if (!shouldRetry) {
    return response
  }

  const refreshed = await refreshAccessToken()
  if (!refreshed) {
    authConfig?.onUnauthorized()
    return response
  }

  return doFetch(endpoint, options, true)
}

export async function requestJson<T>(path: string, options: ApiRequestOptions = {}): Promise<T> {
  const endpoint = resolveUrl(path)

  let response: Response
  try {
    response = await doFetch(endpoint, options, false)
  } catch {
    throw new ApiClientError('Network request failed.', endpoint)
  }

  const data = await parseResponseData(response)
  if (!response.ok) {
    throw new ApiClientError('Request failed.', endpoint, response.status, data)
  }

  return data as T
}
