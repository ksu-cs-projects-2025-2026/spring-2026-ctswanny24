import axios from "axios";
import React, { useEffect, useState } from "react";
import Grid from "@mui/material/Grid";
import RecipeItemCard from "../components/ui/recipes/RecipeItemCard";
import "./Recipes.css";

export default function Recipes() {
    const [recipes, setRecipes] = useState([]);
    const [message, setMessage] = useState("");
    const [isCuratedView, setIsCuratedView] = useState(false);

    useEffect(() => {
        let mounted = true;

        const fetchRecipes = async () => {
            try {
                const response = await axios.get("https://localhost:7111/api/recipe/curated", {
                    params: {
                        limit: 24,
                        minimumMatchPercentage: 0
                    }
                });

                if (mounted) {
                    setRecipes(response.data.recipes ?? []);
                    setMessage(response.data.message ?? "");
                    setIsCuratedView(true);
                }
            }
            catch (error) {
                if (error.response?.status === 401) {
                    setMessage("Log in to unlock personalized matches. Sample recipes are shown for now.");
                } else {
                    console.error("Error fetching curated recipes:", error);
                }

                try {
                    const fallbackResponse = await axios.get("https://localhost:7111/api/recipe/placeholders");

                    if (mounted) {
                        setRecipes(fallbackResponse.data);
                        setIsCuratedView(false);
                        if (error.response?.status !== 401) {
                            setMessage("Curated recipes could not be loaded, so sample recipes are being shown instead.");
                        }
                    }
                }
                catch (fallbackError) {
                    if (mounted) {
                        setMessage("Unable to fetch recipes right now.");
                    }

                    console.error("Error fetching fallback recipes:", fallbackError);
                }
            }
        };

        fetchRecipes();

        return () => {
            mounted = false;
        };
    }, []);

    return (
        <section className="recipesPage">
            <div className="recipesHeader pagePanel">
                <div>
                    <p className="recipesEyebrow">Recipe curation</p>
                    <h1>{isCuratedView ? "Meals ranked against your current inventory" : "Recipe ideas to explore"}</h1>
                    <p className="recipesMessage">{message}</p>
                </div>

                <div className="recipesHighlights">
                    <div>
                        <strong>{recipes.length}</strong>
                        <span>{isCuratedView ? "recipes ranked for you" : "recipes in preview mode"}</span>
                    </div>
                    <div>
                        <strong>{isCuratedView ? "Inventory-aware" : "Browse mode"}</strong>
                        <span>{isCuratedView ? "matches react to your pantry" : "log in for personalized scoring"}</span>
                    </div>
                </div>
            </div>

            {recipes.length === 0 ? (
                <div className="recipesEmptyState pagePanel">
                    <h2>No recipes to show yet.</h2>
                    <p>Add inventory items or check that the recipe API is available, then come back for curated matches.</p>
                </div>
            ) : (
                <Grid container spacing={3}>
                    {recipes.map((recipe) => (
                        <Grid key={recipe.recipeId} size={{ xs: 12, md: 6, xl: 4 }}>
                            <RecipeItemCard recipe={recipe} isCurated={isCuratedView} />
                        </Grid>
                    ))}
                </Grid>
            )}
        </section>
    );
}
