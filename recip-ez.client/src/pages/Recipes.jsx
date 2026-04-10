import RecipeItemCard from "../components/ui/recipes/RecipeItemCard"
import axios from "axios";
import React, { useState, useEffect } from "react";
import Grid from '@mui/material/Grid';

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
        <Grid container rowSpacing={5} columnSpacing={2}>
            {recipes.map(recipe => (
                <Grid key={recipe.recipeId}>
                    <RecipeItemCard recipe={recipe} />
                </Grid>
            ))}
        </Grid>

    )
}