const DEFAULT_API_BASE_URL = 'http://localhost:3000/api'

function normalizeBaseUrl(url: string): string {
  return url.replace(/\/+$/, '')
}

export function getApiBaseUrl(): string {
  return normalizeBaseUrl(import.meta.env.VITE_API_BASE_URL || DEFAULT_API_BASE_URL)
}
