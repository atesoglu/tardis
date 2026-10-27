const API_BASE = import.meta.env.VITE_API_BASE || '';

export async function fetchBff<T>(path: string, init?: RequestInit): Promise<T> {
  const res = await fetch(`${API_BASE}${path}`, { credentials: 'include', ...init });
  if (!res.ok) throw new Error(`BFF ${path} failed: ${res.status}`);
  return res.json() as Promise<T>;
}

export const bff = {
  dashboard: () => fetchBff('/bff/dashboard'),
  health: () => fetch(`/health`).then(r => r.json()),
};