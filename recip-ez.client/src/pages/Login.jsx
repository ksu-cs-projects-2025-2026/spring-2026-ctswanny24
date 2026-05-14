import LoginCard from "../components/ui/login/LoginCard";
import RegistrationCard from "../components/ui/login/RegistrationCard";
import "./Login.css";

export default function Login() {
    const isRegistering = window.location.pathname === "/register";

    return (
        <section className="loginLayout">
            <div className="loginIntro pagePanel">
                <p className="loginEyebrow">{isRegistering ? "Start fresh" : "Your kitchen, remembered"}</p>
                <h1>
                    {isRegistering
                        ? "Create an account and start matching recipes to your kitchen."
                        : "Sign in and turn ingredient tracking into better dinner decisions."}
                </h1>
                <p>
                    {isRegistering
                        ? "Once your account is ready, Recip-EZ can keep your pantry and recipe matches connected."
                        : "Once you log in, Recip-EZ can tie your inventory to recipe matching and keep your kitchen state personalized."}
                </p>

                <div className="loginHighlights">
                    <div>
                        <strong>Track</strong>
                        <span>what is already in your pantry</span>
                    </div>
                    <div>
                        <strong>Match</strong>
                        <span>recipes against real ingredients</span>
                    </div>
                    <div>
                        <strong>Cook</strong>
                        <span>with fewer extra store trips</span>
                    </div>
                </div>
            </div>

            <div className="loginFormColumn">
                {isRegistering ? <RegistrationCard /> : <LoginCard />}
            </div>
        </section>
    );
}
