import RecipeItemCard from "../components/ui/recipes/RecipeItemCard"
import axios from "axios";
import React, { useState, useEffect } from "react";

export default function Recipes() {

    const [recipes, setRecipes] = useState([]);

    useEffect(() => {
        let mounted = true;

        (async () => {
            try {
                const response = await axios.get("https://localhost:7111/api/recipe/placeholders");
                if (mounted) {
                    setRecipes(response.data);
                }
            }
            catch (error) {
                console.error("Error fetching recipe:", error);
            }
        })();

        return () => { mounted = false; };
    }, []);


    return (
        <div class="recipesContainer">
            {recipes.map(recipe => (
                <RecipeItemCard key={recipe.recipeId} recipe={recipe} />
            ))}
        </div>
    )
}