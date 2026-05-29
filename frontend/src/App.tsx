import './App.css'
import { Dashboard } from './pages/Dashboard'
import bggLogo from './assets/powered_by_BGG_04_XL.png'

function App() {
  return (
    <>
      <Dashboard />
      <footer className="bgg-footer">
        <a href="https://boardgamegeek.com" target="_blank" rel="noopener noreferrer">
          <img
            src={bggLogo}
            alt="Powered by BoardGameGeek"
            className="bgg-logo"
          />
        </a>
      </footer>
    </>
  )
}

export default App
