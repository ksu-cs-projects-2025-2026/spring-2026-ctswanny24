import "./Home.css";

const featureCards = [
    {
        title: "Build a live pantry",
        body: "Track ingredients you already have so your kitchen becomes the starting point for every meal."
    },
    {
        title: "Match recipes instantly",
        body: "See which dishes are ready now, which are close, and what single item would unlock dinner."
    },
    {
        title: "Waste less food",
        body: "Turn forgotten ingredients into actual meals before they disappear into the back of the fridge."
    }
];

const workflowSteps = [
    "Log into your account and start an inventory for your kitchen.",
    "Add ingredients with quantities and units as you shop or restock.",
    "Browse curated recipes ranked by what you can already make."
];

export default function Home() {
    return (
        <section className="homePage">
            <div className="homeHero pagePanel">
                <div className="heroCopy">
                    <p className="heroEyebrow">Smart pantry planning</p>
                    <h1>Cook from your shelves before reaching for another grocery list.</h1>
                    <p className="heroLead">
                        Recip-EZ helps you turn the ingredients you already own into realistic recipe ideas,
                        so dinner feels easier and your inventory stays useful.
                    </p>

                    <div className="heroActions">
                        <a href="/inventory" className="heroButton heroButtonPrimary">Start your inventory</a>
                        <a href="/recipes" className="heroButton heroButtonSecondary">See recipe matches</a>
                    </div>
                </div>

                <div className="heroShowcase">
                    <div className="showcaseCard showcasePrimary">
                        <span className="showcaseLabel">Tonight's best fit</span>
                        <strong>Skillet pasta with tomato, garlic, and olive oil</strong>
                        <p>86% pantry match with only parmesan missing.</p>
                    </div>

                    <div className="showcaseGrid">
                        <div className="showcaseCard">
                            <span className="showcaseMetric">24</span>
                            <p>ingredients tracked in the average active pantry</p>
                        </div>
                        <div className="showcaseCard">
                            <span className="showcaseMetric">3</span>
                            <p>quick steps from pantry check to dinner idea</p>
                        </div>
                    </div>
                </div>
            </div>

            <div className="homeFeatureGrid">
                {featureCards.map((card) => (
                    <article key={card.title} className="featureCard pagePanel">
                        <h2>{card.title}</h2>
                        <p>{card.body}</p>
                    </article>
                ))}
            </div>

            <div className="homeWorkflow pagePanel">
                <div>
                    <p className="heroEyebrow">How it works</p>
                    <h2>A simple loop built for everyday cooking</h2>
                </div>

                <div className="workflowList">
                    {workflowSteps.map((step, index) => (
                        <div key={step} className="workflowStep">
                            <span>{index + 1}</span>
                            <p>{step}</p>
                        </div>
                    ))}
                </div>
            </div>
        </section>
    );
}
