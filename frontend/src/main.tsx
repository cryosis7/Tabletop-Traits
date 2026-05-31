import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './i18n'
import './index.css'
import App from './App.tsx'

// Fire-and-forget ping to warm up the backend on cold starts
const apiUrl = import.meta.env.VITE_API_URL || '/api'
if (!navigator.sendBeacon?.(`${apiUrl}/ping`)) {
  fetch(`${apiUrl}/ping`).catch(() => {})
}

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <App />
  </StrictMode>,
)
