import Button from "@mui/material/Button";
import Card from "@mui/material/Card";
import CardActions from "@mui/material/CardActions";
import CardContent from "@mui/material/CardContent";
import Chip from "@mui/material/Chip";
import Stack from "@mui/material/Stack";
import Typography from "@mui/material/Typography";

function getRecipeStatus(recipe, isCurated) {
    if (!isCurated) {
        return { label: "Sample Recipe", color: "default" };
    }

    if (recipe.canMakeNow) {
        return { label: "Ready to Make", color: "success" };
    }

    if (recipe.isCloseMatch) {
        return { label: "Close Match", color: "warning" };
    }

    return { label: "Needs More Items", color: "default" };
}

export default function RecipeItemCard({ recipe, isCurated = false }) {
    const status = getRecipeStatus(recipe, isCurated);
    const missingIngredients = recipe.missingIngredients ?? [];
    const matchedIngredients = recipe.matchedIngredients ?? [];
    const hasLink = Boolean(recipe.url);

    return (
        <Card
            sx={{
                height: "100%",
                borderRadius: 4,
                background: "linear-gradient(180deg, #fffdf8 0%, #fff4e8 100%)",
                boxShadow: "0 18px 44px rgba(84, 58, 20, 0.12)"
            }}
        >
            <CardContent sx={{ display: "grid", gap: 2.25 }}>
                <Stack direction="row" spacing={1} useFlexGap flexWrap="wrap">
                    <Chip label={status.label} color={status.color} size="small" />
                    {isCurated && (
                        <Chip
                            label={`${recipe.matchPercentage}% match`}
                            variant="outlined"
                            size="small"
                        />
                    )}
                </Stack>

                <div>
                    <Typography gutterBottom variant="h5" component="h2" sx={{ marginBottom: 0.5 }}>
                        {recipe.recipeName}
                    </Typography>

                    {isCurated && (
                        <Typography variant="body2" sx={{ color: "text.secondary" }}>
                            {recipe.matchedIngredientCount} of {recipe.totalIngredientCount} core ingredients match your inventory.
                        </Typography>
                    )}
                </div>

                {isCurated && matchedIngredients.length > 0 && (
                    <div>
                        <Typography variant="subtitle2" sx={{ fontWeight: 700, marginBottom: 0.75 }}>
                            You already have
                        </Typography>
                        <Stack direction="row" spacing={1} useFlexGap flexWrap="wrap">
                            {matchedIngredients.map((ingredient) => (
                                <Chip key={`${recipe.recipeId}-match-${ingredient}`} label={ingredient} size="small" color="success" variant="outlined" />
                            ))}
                        </Stack>
                    </div>
                )}

                {isCurated && missingIngredients.length > 0 && (
                    <div>
                        <Typography variant="subtitle2" sx={{ fontWeight: 700, marginBottom: 0.75 }}>
                            Missing ingredients
                        </Typography>
                        <Stack direction="row" spacing={1} useFlexGap flexWrap="wrap">
                            {missingIngredients.map((ingredient) => (
                                <Chip key={`${recipe.recipeId}-missing-${ingredient}`} label={ingredient} size="small" color="warning" />
                            ))}
                        </Stack>
                    </div>
                )}

                <div>
                    <Typography variant="subtitle2" sx={{ fontWeight: 700, marginBottom: 0.75 }}>
                        Full ingredient list
                    </Typography>
                    <ul style={{ margin: 0, paddingLeft: "1.25rem" }}>
                        {recipe.ingredients.map((ingredient, index) => (
                            <li key={`${recipe.recipeId}-ingredient-${index}`}>{ingredient}</li>
                        ))}
                    </ul>
                </div>

                <div>
                    <Typography variant="subtitle2" sx={{ fontWeight: 700, marginBottom: 0.75 }}>
                        Instructions
                    </Typography>
                    <ol style={{ margin: 0, paddingLeft: "1.25rem" }}>
                        {recipe.instructions.map((instruction, index) => (
                            <li key={`${recipe.recipeId}-instruction-${index}`}>{instruction}</li>
                        ))}
                    </ol>
                </div>
            </CardContent>

            <CardActions sx={{ padding: "0 16px 16px" }}>
                {hasLink && (
                    <Button size="small" href={recipe.url} target="_blank" rel="noreferrer">
                        Open Source
                    </Button>
                )}
                {recipe.source && <Typography variant="caption">Source: {recipe.source}</Typography>}
            </CardActions>
        </Card>
    );
}
