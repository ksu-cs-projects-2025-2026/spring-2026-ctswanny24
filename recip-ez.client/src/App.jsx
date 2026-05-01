import Navbar from './components/ui/Navbar'
import Inventory from './pages/Inventory'
import Recipes from './pages/Recipes'
import Home from './pages/Home'
import Login from './pages/Login'
import './App.css'
 
function App() {
    let Component
    switch (window.location.pathname) {
        case "/":
            Component = Home
            break
        case "/inventory":
            Component = Inventory
            break
        case "/recipes":
            Component = Recipes
            break
        case "/login":
            Component = Login
            break
        default:
            Component = Home
            break
    }
  return (
      <div className="appShell">
          <div className="siteNavWrap">
            <Navbar/>
          </div>

          <main className="appContent">
            <Component/>
          </main>
      </div>
  )
}
export default App
