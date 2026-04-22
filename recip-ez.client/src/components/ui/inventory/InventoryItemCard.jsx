import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import CardMedia from '@mui/material/CardMedia';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import axios from 'axios';
import { useState } from 'react';

export default function InventoryItemCard({ inventoryItem, onDelete }) {

    const [message, setMessage] = useState("");

    const DeleteButton = async () => {
        try {
            const id = parseInt(inventoryItem.userInventoryId);
            const response = await axios.delete(`https://localhost:7111/api/Inventory/${id}`);
            if (response.data.success) {
                onDelete(id);
            }
        }
        catch (e) {
            setMessage(e.message);
        }
    }

    return (
        <Card sx={{ maxWidth: 345 }}>
            <CardContent>
                <Typography gutterBottom variant="h5" component="div">
                    {inventoryItem.ingredientName}
                </Typography>
                <Typography variant="body2" sx={{ color: 'text.secondary' }}>
                    {inventoryItem.quantity} {inventoryItem.unit}
                </Typography>
            </CardContent>
            <CardActions>
                <Button size="small">Edit</Button>
                <Button size="small" type="submit" onClick={DeleteButton}>Delete</Button>
            </CardActions>
            {message && <p>{message}</p>}
        </Card>
    );
}