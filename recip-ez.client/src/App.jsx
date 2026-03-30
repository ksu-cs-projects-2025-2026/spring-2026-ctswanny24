import Navbar from './components/ui/Navbar'
import Inventory from './pages/Inventory'
import Recipes from './pages/Recipes'
import Home from './pages/Home'
import Login from './pages/Login'
//import Styles from './components/ui/Styles.css'
 
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
    }
  return (
      <>
          <div>
            <Navbar/>
          </div>

          <div className="componentContainer">
            <Component/>
          </div>
      </>
  )
}
export default App
