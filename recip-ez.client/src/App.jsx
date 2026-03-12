import { useState } from 'react'
import { Navigation } from './components/navigation'
import './App.css'
 
function App() {
  const [count, setCount] = useState(0)

  return (
    <>
      <div>
        <Navigation/>
      </div>

    </>
  )
}
export default App
