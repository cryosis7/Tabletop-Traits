import { useTranslation } from 'react-i18next'
import './App.css'
import { Dashboard } from './pages/Dashboard'
import bggLogo from './assets/powered_by_BGG_04_XL.png'

function App() {
  const { t } = useTranslation()

  return (
    <>
      <main>
        <Dashboard />
      </main>
      <footer className="bgg-footer">
        <a href="https://boardgamegeek.com" target="_blank" rel="noopener noreferrer">
          <img
            src={bggLogo}
            alt={t('app.poweredByBGG')}
            className="bgg-logo"
          />
        </a>
      </footer>
    </>
  )
}

export default App
