import './App.css'
import { Dashboard } from './pages/Dashboard'

function App() {
  return (
    <>
      <Dashboard />
      <footer className="bgg-footer">
        <a href="https://boardgamegeek.com" target="_blank" rel="noopener noreferrer">
          <img
            src="https://cf.geekdo-images.com/images/geekdo/bgg_cornerlogo.png"
            alt="Powered by BoardGameGeek"
            className="bgg-logo"
          />
        </a>
      </footer>
    </>
  )
}

export default App
