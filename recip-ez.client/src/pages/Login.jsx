import LoginCard from "../components/ui/login/LoginCard";
import "./Login.css";

export default function Login() {
    return (
        <section className="loginLayout">
            <div className="loginIntro pagePanel">
                <p className="loginEyebrow">Your kitchen, remembered</p>
                <h1>Sign in and turn ingredient tracking into better dinner decisions.</h1>
                <p>
                    Once you log in, Recip-EZ can tie your inventory to recipe matching and keep your kitchen state personalized.
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
                <LoginCard />
            </div>
        </section>
    );
}
