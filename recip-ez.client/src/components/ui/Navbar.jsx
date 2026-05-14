import { useEffect, useState } from "react";
import axios from "axios";
import Button from "@mui/material/Button";
import Menu from "@mui/material/Menu";
import MenuItem from "@mui/material/MenuItem";

//Array of nav links that the navbar will use.
//Better to use than explicit coding of each link.
const navLinks = [
    { href: "/", label: "Home" },
    { href: "/inventory", label: "Inventory" },
    { href: "/recipes", label: "Recipes" }
];

export default function Navbar() {
    //Getting the current path to determine which nav link should be active
    const currentPath = window.location.pathname;

    //Keeping track of the anchor element for account menu
    const [anchorEl, setAnchorEl] = useState(null);

    //State for the authenticated user.
    const [authUser, setAuthUser] = useState(null);

    //State of whether the authentication check has happened.
    const [authChecked, setAuthChecked] = useState(false);

    const menuOpen = Boolean(anchorEl);

    //On the open/refresh of the page, check for authenticated user and update state accordingly
    //This allows the navbar (account menu) to use the correct options. (Login/Register vs Logout)
    useEffect(() => {
        let mounted = true;

        const fetchAuthenticatedUser = async () => {
            try {
                //Grabbing the authenticated user (if any)
                const response = await axios.get("/api/login/me");

                //If the component is still mounted and we got a successful response, update the authUser state
                if (mounted && response.data.success) {
                    setAuthUser(response.data);
                }
                //If there was an error (like a 401 Unauthorized), we can assume there's no authenticated user and set authUser to null
            } catch (error) {
                if (mounted) {
                    setAuthUser(null);
                }
            //Final check to ensure we mark authChecked as true regardless of success or failure, but only if the component is still mounted
            } finally {
                if (mounted) {
                    setAuthChecked(true);
                }
            }
        };

        fetchAuthenticatedUser();

        return () => {
            mounted = false;
        };
    }, []);

    //Handler for opening the account menu
    //Event.currentTarget is the Button element that was clicked
    const handleMenuOpen = (event) => {
        setAnchorEl(event.currentTarget);
    };

    //Handler for closing the account menu
    //Sets the anchorEl back to null closing the menu
    const handleMenuClose = () => {
        setAnchorEl(null);
    };

    //Function for logging out the user.
    //Makes a POST request to the logout endpoint created earlier
    //On success, it clears the authUser state, closes the menu, and redirects to the homepage 
    //as well as sets the menu options back to Login / Register
    const handleLogout = async () => {
        try {
            await axios.post("/api/login/logout");
            setAuthUser(null);
            handleMenuClose();
            window.location.href = "/";
        } catch (error) {
            console.error("Logout failed:", error);
            handleMenuClose();
        }
    };

    //Function to set the navigation link and close menu when the location changes.
    const goTo = (href) => {
        handleMenuClose();
        window.location.href = href;
    };

    //Finds the label for the account menu button.
    //If authChecked is false, we haven't checked for authenticated user
    //Otherwise, display the authenticated user's name if available
    const authLabel = authChecked
        ? authUser?.name ?? "Account"
        : "Account";

    return (
        //Main navbar element
        //First a element is for displaying the Site name and tagline
        //Next ul element is for the navigation links that aren't login related, like Home, Inventory, Recipes
        //Finally, the div with className=navActions contains the account menu button and the dropdown menu
        //Upon the main button click, an anchor element is tied to that button to open the menu. 
        //The Menu is tied to that anchor element and contains MenuItems corresponding to the current state of the login (logged in, logged out).
        <nav className="siteNav">

            <a href="/" className="siteBrand" aria-label="Recip-EZ home">
                <span className="siteTitle">Recip-EZ</span>
                <span className="siteTagline">Quelling the chaos in your kitchen.</span>
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

            <div className="navActions">
                <Button
                    onClick={handleMenuOpen}
                    className="navMenuTrigger"
                    disableElevation
                    aria-controls={menuOpen ? "account-menu" : undefined}
                    aria-haspopup="true"
                    aria-expanded={menuOpen ? "true" : undefined}
                >
                    {authLabel}
                </Button>

                <Menu
                    id="account-menu"
                    anchorEl={anchorEl}
                    open={menuOpen}
                    onClose={handleMenuClose}
                    MenuListProps={{
                        "aria-labelledby": "account-menu"
                    }}
                    PaperProps={{
                        sx: {
                            marginTop: 1,
                            minWidth: 180,
                            borderRadius: "20px",
                            padding: "0.35rem",
                            background: "rgba(248, 252, 255, 0.98)",
                            border: "1px solid rgba(48, 91, 148, 0.12)",
                            boxShadow: "0 18px 36px rgba(37, 78, 130, 0.14)"
                        }
                    }}
                >
                    {authUser ? (
                        <MenuItem onClick={handleLogout}>Log Out</MenuItem>
                    ) : (
                        [
                            <MenuItem key="login" onClick={() => goTo("/login")}>
                                Log In
                            </MenuItem>,
                            <MenuItem key="register" onClick={() => goTo("/register")}>
                                Register
                            </MenuItem>
                        ]
                    )}
                </Menu>
            </div>
        </nav>
    );
}
