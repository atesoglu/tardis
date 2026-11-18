import { useEffect, useRef, useState } from 'react';
import { useMsal } from '@azure/msal-react';
import { InteractionRequiredAuthError } from '@azure/msal-browser';

export default function App() {
  const { accounts, instance, inProgress } = useMsal();
  const [data, setData] = useState<any>(null);
  const [error, setError] = useState<string | null>(null);
  const loaded = useRef<boolean>(false);

  const loadDashboard = async () => {
    if (!accounts.length) return;
    const account = accounts[0];
    try {
      const token = await instance.acquireTokenSilent({ account, scopes: ['api://cd3de366-c14a-4249-b55d-15cebcc4ab99/.default'] });
      const res = await fetch('/bff/dashboard', { headers: { Authorization: `Bearer ${token.accessToken}` } });
      if (!res.ok) throw new Error(`BFF ${res.status}`);
      setData(await res.json());
    } catch (err: unknown) {
      if (isInteractionRequiredError(err)) {
        await instance.acquireTokenRedirect({ scopes: ['api://cd3de366-c14a-4249-b55d-15cebcc4ab99/.default'] });
      } else if (err instanceof Error) {
        setError(err.message);
      }
    }
  };

  useEffect(() => {
    if (accounts.length > 0 && !loaded.current) {
      loaded.current = true;
      loadDashboard();
    }
  }, [accounts]);

  if (inProgress !== 'none') return <div>Loading authentication...</div>;
  if (accounts.length === 0) {
    return (
      <div style={{ padding: 32, fontFamily: 'sans-serif' }}>
        <h1>Tardis Dashboard</h1>
        <p>Please sign in to continue.</p>
        <button onClick={() => instance.loginRedirect()}>Sign in with Microsoft</button>
      </div>
    );
  }

  return (
    <div style={{ padding: 32, fontFamily: 'sans-serif' }}>
      <h1>Tardis Dashboard</h1>
      <p>Authenticated as: {accounts[0]?.username ?? '—'}</p>
      {error && <p style={{ color: 'red' }}>{error}</p>}
      {data && <div>BFF payload: {JSON.stringify(data)}</div>}
      <button onClick={() => instance.logoutRedirect()}>Sign out</button>
    </div>
  );
}

function isInteractionRequiredError(err: any): boolean {
  return err instanceof InteractionRequiredAuthError || err?.errorCode === 'interaction_required';
}