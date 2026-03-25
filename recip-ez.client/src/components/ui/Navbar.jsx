export default function Navbar() {
    return(
    <nav className="nav">
        <a href="/" className="site-title">Recip-EZ</a>

        <ul>
            <li>
                <a href="/inventory">Inventory</a>
            </li>
            <li>
                <a href="/recipes">Recipes</a>
            </li>
        </ul>
        <ul>
            <li>
                <a href="/login">Login</a>
            </li>
        </ul>
    </nav >
    )
} 