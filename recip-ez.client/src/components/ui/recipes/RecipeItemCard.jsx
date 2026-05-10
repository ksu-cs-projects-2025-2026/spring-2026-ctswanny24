import Button from "@mui/material/Button";
import Card from "@mui/material/Card";
import CardActions from "@mui/material/CardActions";
import CardContent from "@mui/material/CardContent";
import Chip from "@mui/material/Chip";
import Divider from "@mui/material/Divider";
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

function formatScore(score) {
    if (typeof score !== "number" || Number.isNaN(score)) {
        return "0%";
    }

    return `${Math.round(score * 100)}%`;
}

function IngredientChipList({ recipeId, label, ingredients, color }) {
    if (!ingredients.length) {
        return null;
    }

    return (
        <div>
            <Typography variant="caption" sx={{ color: "text.secondary", fontSize: "0.68rem", fontWeight: 700, textTransform: "uppercase" }}>
                {label}
            </Typography>
            <Stack direction="row" spacing={1} useFlexGap flexWrap="wrap" sx={{ marginTop: 0.75 }}>
                {ingredients.map((ingredient) => (
                    <Chip
                        key={`${recipeId}-${label}-${ingredient}`}
                        label={ingredient}
                        size="small"
                        color={color}
                        variant={color === "default" ? "outlined" : "filled"}
                    />
                ))}
            </Stack>
        </div>
    );
}

export default function RecipeItemCard({ recipe, isCurated = false }) {
    const status = getRecipeStatus(recipe, isCurated);
    const matchedIngredients = recipe.matchedIngredients ?? [];
    const missingCoreIngredients = recipe.missingCoreIngredients ?? [];
    const missingSupportingIngredients = recipe.missingSupportingIngredients ?? [];
    const missingOptionalIngredients = recipe.missingOptionalIngredients ?? [];
    const hasMissingIngredients = missingCoreIngredients.length > 0
        || missingSupportingIngredients.length > 0
        || missingOptionalIngredients.length > 0;
    const hasLink = Boolean(recipe.url);

    return (
        <Card
            sx={{
                height: "100%",
                borderRadius: 2.5,
                background: "linear-gradient(180deg, #fffdf8 0%, #fff4e8 100%)",
                boxShadow: "0 18px 44px rgba(84, 58, 20, 0.12)"
            }}
        >
            <CardContent sx={{ display: "grid", gap: 1.75, padding: 2.25 }}>
                <Stack direction="row" spacing={1} useFlexGap flexWrap="wrap">
                    <Chip label={status.label} color={status.color} size="small" />
                    {isCurated && (
                        <Chip
                            label={`${formatScore(recipe.score)} heuristic match`}
                            variant="outlined"
                            size="small"
                        />
                    )}
                </Stack>

                <div>
                    <Typography
                        gutterBottom
                        variant="h6"
                        component="h2"
                        sx={{ fontSize: "1.12rem", lineHeight: 1.25, marginBottom: 0.5 }}
                    >
                        {recipe.recipeName}
                    </Typography>

                    {isCurated && (
                        <Typography variant="body2" sx={{ color: "text.secondary", fontSize: "0.88rem" }}>
                            {recipe.matchedIngredientCount} of {recipe.totalIngredientCount} ingredients match your inventory.
                        </Typography>
                    )}
                </div>

                {isCurated && (
                    <Stack direction="row" spacing={1} useFlexGap flexWrap="wrap">
                        <Chip label={`Core ${formatScore(recipe.coreScore)}`} color="success" variant="outlined" size="small" />
                        <Chip label={`Supporting ${formatScore(recipe.supportingScore)}`} color="primary" variant="outlined" size="small" />
                        <Chip label={`Optional ${formatScore(recipe.optionalScore)}`} color="default" variant="outlined" size="small" />
                    </Stack>
                )}

                {isCurated && matchedIngredients.length > 0 && (
                    <div>
                        <Typography variant="subtitle2" sx={{ fontSize: "0.9rem", fontWeight: 700, marginBottom: 0.75 }}>
                            You already have
                        </Typography>
                        <Stack direction="row" spacing={1} useFlexGap flexWrap="wrap">
                            {matchedIngredients.map((ingredient) => (
                                <Chip key={`${recipe.recipeId}-match-${ingredient}`} label={ingredient} size="small" color="success" variant="outlined" />
                            ))}
                        </Stack>
                    </div>
                )}

                {isCurated && hasMissingIngredients && (
                    <div>
                        <Typography variant="subtitle2" sx={{ fontSize: "0.9rem", fontWeight: 700, marginBottom: 0.75 }}>
                            Missing ingredients
                        </Typography>
                        <Stack spacing={1.5}>
                            <IngredientChipList
                                recipeId={recipe.recipeId}
                                label="Core"
                                ingredients={missingCoreIngredients}
                                color="error"
                            />
                            <IngredientChipList
                                recipeId={recipe.recipeId}
                                label="Supporting"
                                ingredients={missingSupportingIngredients}
                                color="warning"
                            />
                            <IngredientChipList
                                recipeId={recipe.recipeId}
                                label="Optional"
                                ingredients={missingOptionalIngredients}
                                color="default"
                            />
                        </Stack>
                    </div>
                )}

                {isCurated && <Divider />}

                <div>
                    <Typography variant="subtitle2" sx={{ fontSize: "0.9rem", fontWeight: 700, marginBottom: 0.75 }}>
                        Full ingredient list
                    </Typography>
                    <ul style={{ fontSize: "0.9rem", lineHeight: 1.45, margin: 0, paddingLeft: "1.15rem" }}>
                        {recipe.ingredients.map((ingredient, index) => (
                            <li key={`${recipe.recipeId}-ingredient-${index}`}>{ingredient}</li>
                        ))}
                    </ul>
                </div>

                <div>
                    <Typography variant="subtitle2" sx={{ fontSize: "0.9rem", fontWeight: 700, marginBottom: 0.75 }}>
                        Instructions
                    </Typography>
                    <ol style={{ fontSize: "0.9rem", lineHeight: 1.45, margin: 0, paddingLeft: "1.15rem" }}>
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
