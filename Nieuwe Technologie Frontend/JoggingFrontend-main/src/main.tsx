import { createRoot } from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';
import App from './routes/App';
import './index.css';
import { AuthProvider } from './routes/auth/context/AuthProvider'; // Import als named export

const root = createRoot(document.getElementById('root') as HTMLElement);

root.render(
  <BrowserRouter>
    <AuthProvider>
      <App />
    </AuthProvider>
  </BrowserRouter>
);