// recip-ez.client\src\pages\Inventory.jsx
import InventoryAdditionCard from "../components/ui/inventory/InventoryAdditionCard"
import  Grid from '@mui/material/Grid'
import InventoryItemCard from "../components/ui/inventory/InventoryItemCard"
import AddInventoryItem from "../components/ui/inventory/AddToInventory"
import { useEffect } from "react";
import { useState } from "react";
import axios from "axios";


export default function Inventory() {
    
    const [inventory, setInventory] = useState([]);
    const [message, setMessage] = useState("");

    useEffect(() => {
        const fetchInventory = async () => {
            try {
                const userId = localStorage.getItem("userId");

                if (userId == null) {
                    setMessage("You must log-in before accessing Inventory!");
                    return;
                }

                const response = await axios.get("https://localhost:7111/api/Inventory/userInventory", {
                    params: { userId: userId }
                });
                if (response.data.success) {
                    setInventory(response.data.inventory);
                    setMessage("Inventory fetched successfully");
                }
                else {
                    setMessage("Failed to fetch inventory");
                }
            }
            catch (error) {
                setMessage(error.message);
            }
        };

        fetchInventory();
    }, []);

    const handleDelete = (id) => {
        setInventory(prev =>
            prev.filter(item => item.userInventoryId !== id)
        );
    };

    const handleAdd = (item) => {
        setInventory(prev => [...prev, item]);
    };


    return (
        <>
            {message && <p>{message}</p> }
            <Grid container spacing={2}>
                <Grid size={4}>
                    <AddInventoryItem onAdd={handleAdd} />
                </Grid>
                <Grid container spacing={2}>
                    {inventory.map(item => (
                        <Grid item key={item.userInventoryId}>
                            <InventoryItemCard
                                inventoryItem={item}
                                ingredient={item}
                                onDelete={handleDelete} />
                        </Grid>))}
                </Grid>
            </Grid>
        </>
    )
}