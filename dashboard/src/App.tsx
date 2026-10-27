import { useMsal } from '@azure/msal-react';

export default function App() {
  const { accounts, instance, inProgress } = useMsal();
  const isAuthenticated = accounts.length > 0;

  if (inProgress === 'none' && !isAuthenticated) {
    instance.loginRedirect();
    return null;
  }

  if (inProgress !== 'none') {
    return <div>Loading...</div>;
  }

  return (
    <div style={{ padding: 32, fontFamily: 'sans-serif' }}>
      <h1>Tardis Dashboard (Placeholder)</h1>
      <p>Authenticated as: {accounts[0]?.username ?? '—'}</p>
      <p>
        BFF layer will be wired here. Logout:
        <button onClick={() => instance.logoutRedirect()}>Sign out</button>
      </p>
    </div>
  );
}