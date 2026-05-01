const navLinks = [
    { href: "/", label: "Home" },
    { href: "/inventory", label: "Inventory" },
    { href: "/recipes", label: "Recipes" }
];

export default function Navbar() {
    const currentPath = window.location.pathname;

    return (
        <nav className="siteNav">
            <a href="/" className="siteBrand" aria-label="Recip-EZ home">
                <span className="siteTitle">Recip-EZ</span>
                <span className="siteTagline">Cook from what is already in your kitchen.</span>
            </a>

            <ul className="navLinks">
                {navLinks.map((link) => (
                    <li key={link.href}>
                        <a
                            href={link.href}
                            className={`navLink ${currentPath === link.href ? "isActive" : ""}`}
                        >
                            {link.label}
                        </a>
                    </li>
                ))}
            </ul>

            <ul className="navActions">
                <li>
                    <a
                        href="/login"
                        className={`navLink navActionButton ${currentPath === "/login" ? "isActive" : ""}`}
                    >
                        Login
                    </a>
                </li>
            </ul>
        </nav>
    );
}
