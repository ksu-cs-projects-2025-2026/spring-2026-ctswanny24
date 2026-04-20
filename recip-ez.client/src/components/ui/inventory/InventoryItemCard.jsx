import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import CardMedia from '@mui/material/CardMedia';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';

export default function InventoryItemCard({ingredient}) {
    return (
        <Card sx={{ maxWidth: 345 }}>
            <CardContent>
                <Typography gutterBottom variant="h5" component="div">
                    {ingredient.ingredientName}
                </Typography>
                <Typography variant="body2" sx={{ color: 'text.secondary' }}>
                    {ingredient.quantity} {ingredient.unit}
                </Typography>
            </CardContent>
            <CardActions>
                <Button size="small">Edit</Button>
                <Button size="small">Delete</Button>
            </CardActions>
        </Card>
    );
}