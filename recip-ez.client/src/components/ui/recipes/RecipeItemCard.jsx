import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import CardMedia from '@mui/material/CardMedia';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';

/* Include Collapse button in V 1.0. */

export default function RecipeItemCard({recipe}) {
    return (
        <Card sx={{ maxWidth: 345 }}>
            <CardContent>
            
                <Typography gutterBottom variant="h5" component="div">
                    {recipe.recipeName}
                </Typography>
                    <h3>Ingredients: </h3>
                    <ul>
                        {recipe.ingredients.map(ingredient => (
                            <li>{ingredient}</li>
                        )) }

                    </ul>

                    <h5>Instructions: </h5>
                    <ol>
                        {recipe.instructions.map(instructions => (
                            <li>{instructions}</li>
                        ))}
                    </ol>

            </CardContent>
            <CardActions>
                <Button size="small">Share</Button>
                <Button size="small">Learn More</Button>
            </CardActions>
        </Card>
    );
}