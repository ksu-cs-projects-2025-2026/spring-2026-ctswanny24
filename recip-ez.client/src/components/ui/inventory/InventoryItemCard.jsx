import Card from "@mui/material/Card";
import CardActions from "@mui/material/CardActions";
import CardContent from "@mui/material/CardContent";
import Button from "@mui/material/Button";
import Chip from "@mui/material/Chip";
import Stack from "@mui/material/Stack";
import Typography from "@mui/material/Typography";
import axios from "axios";
import { useState } from "react";

export default function InventoryItemCard({ inventoryItem, onDelete }) {
    const [message, setMessage] = useState("");

    const deleteButton = async () => {
        try {
            const id = parseInt(inventoryItem.userInventoryId, 10);
            const response = await axios.delete(`/api/Inventory/${id}`);

            if (response.data.success) {
                onDelete(id);
            }
        }
        catch (error) {
            setMessage(error.message);
        }
    };

    return (
        <Card
            sx={{
                height: "100%",
                borderRadius: 4,
                background: "linear-gradient(180deg, #fbfdff 0%, #eaf4ff 100%)",
                boxShadow: "0 16px 34px rgba(37, 78, 130, 0.1)"
            }}
        >
            <CardContent sx={{ display: "grid", gap: 1.2 }}>
                <Stack direction="row" spacing={1} alignItems="center" useFlexGap flexWrap="wrap">
                    <Chip label="In pantry" size="small" color="primary" variant="outlined" />
                    <Chip label={inventoryItem.unit} size="small" variant="outlined" />
                </Stack>

                <div>
                    <Typography gutterBottom variant="h5" component="h3" sx={{ marginBottom: 0.3 }}>
                        {inventoryItem.ingredientName}
                    </Typography>
                    <Typography variant="body1" sx={{ color: "text.secondary" }}>
                        {inventoryItem.quantity} {inventoryItem.unit}
                    </Typography>
                </div>

                {message && (
                    <Typography variant="caption" sx={{ color: "#b33f34" }}>
                        {message}
                    </Typography>
                )}
            </CardContent>

            <CardActions sx={{ padding: "0 16px 16px" }}>
                <Button size="small" disabled>Edit</Button>
                <Button size="small" color="error" type="button" onClick={deleteButton}>
                    Delete
                </Button>
            </CardActions>
        </Card>
    );
}
