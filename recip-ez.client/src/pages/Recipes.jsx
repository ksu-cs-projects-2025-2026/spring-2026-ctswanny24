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
            const userId = localStorage.getItem("userId");

            if (!userId) {
                setMessage("Log in to see recipes curated from your inventory. Showing sample recipes for now.");

                try {
                    const response = await axios.get("https://localhost:7111/api/recipe/placeholders");

                    if (mounted) {
                        setRecipes(response.data);
                        setIsCuratedView(false);
                    }
                }
                catch (error) {
                    if (mounted) {
                        setMessage("Unable to fetch recipes right now.");
                    }

                    console.error("Error fetching placeholder recipes:", error);
                }

                return;
            }

            try {
                const response = await axios.get("https://localhost:7111/api/recipe/curated", {
                    params: {
                        userId: parseInt(userId, 10),
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
                console.error("Error fetching curated recipes:", error);

                try {
                    const fallbackResponse = await axios.get("https://localhost:7111/api/recipe/placeholders");

                    if (mounted) {
                        setRecipes(fallbackResponse.data);
                        setIsCuratedView(false);
                        setMessage("Curated recipes could not be loaded, so sample recipes are being shown instead.");
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
            <div className="recipesHeader">
                <p className="recipesEyebrow">Recip-EZ Matches</p>
                <h1>{isCuratedView ? "Recipes picked from your inventory" : "Recipe ideas"}</h1>
                <p className="recipesMessage">{message}</p>
            </div>

            <Grid container spacing={3}>
                {recipes.map((recipe) => (
                    <Grid key={recipe.recipeId} size={{ xs: 12, md: 6, xl: 4 }}>
                        <RecipeItemCard recipe={recipe} isCurated={isCuratedView} />
                    </Grid>
                ))}
            </Grid>
        </section>
    );
}
