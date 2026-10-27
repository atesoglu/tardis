import { useEffect, useState } from 'react';
import { bff } from '../services/api';

type Data = { user: string; prCount: number };

export default function Dashboard() {
  const [data, setData] = useState<Data | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    bff.dashboard()
      .then((d: any) => setData(d))
      .catch((e: Error) => setError(e.message));
  }, []);

  if (error) return <div className="error">{error}</div>;
  if (!data) return <div>Loading dashboard...</div>;

  return (
    <div style={{ padding: 24 }}>
      <h1>Tardis Dashboard</h1>
      <h2>Authenticated user: {data.user}</h2>
      <div>Open PRs (last 24h): {data.prCount}</div>
    </div>
  );
}