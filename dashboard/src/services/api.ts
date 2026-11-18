export async function fetchBff<T>(path: string, init?: RequestInit): Promise<T> {
  const res = await fetch(path, init);
  if (!res.ok) throw new Error(`BFF ${path} failed: ${res.status}`);
  return res.json() as Promise<T>;
}

export const bff = {
  dashboard: () => fetchBff('/bff/dashboard'),
};