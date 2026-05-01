import Grid from "@mui/material/Grid";
import { useEffect, useState } from "react";
import axios from "axios";
import InventoryItemCard from "../components/ui/inventory/InventoryItemCard";
import AddInventoryItem from "../components/ui/inventory/AddToInventory";
import "./Inventory.css";

export default function Inventory() {
    const [inventory, setInventory] = useState([]);
    const [message, setMessage] = useState("");

    useEffect(() => {
        const fetchInventory = async () => {
            try {
                const userId = localStorage.getItem("userId");

                if (userId == null) {
                    setMessage("Log in to start building your pantry inventory.");
                    return;
                }

                const response = await axios.get("https://localhost:7111/api/Inventory/userInventory", {
                    params: { userId: userId }
                });

                if (response.data.success) {
                    setInventory(response.data.inventory);
                    setMessage("Inventory ready.");
                }
                else {
                    setMessage("Failed to fetch inventory.");
                }
            }
            catch (error) {
                setMessage(error.message);
            }
        };

        fetchInventory();
    }, []);

    const handleDelete = (id) => {
        setInventory((prev) => prev.filter((item) => item.userInventoryId !== id));
    };

    const handleAdd = (item) => {
        setInventory((prev) => [...prev, item]);
    };

    return (
        <section className="inventoryPage">
            <div className="inventoryHeader pagePanel">
                <div>
                    <p className="inventoryEyebrow">Kitchen inventory</p>
                    <h1>Keep your pantry current so recipe matching stays smart.</h1>
                </div>

                <div className="inventoryHeaderStats">
                    <div>
                        <strong>{inventory.length}</strong>
                        <span>items tracked</span>
                    </div>
                    <div>
                        <strong>{inventory.length > 0 ? "Live" : "Start"}</strong>
                        <span>{inventory.length > 0 ? "ready for matching" : "add your first ingredient"}</span>
                    </div>
                </div>
            </div>

            {message && <p className="inventoryMessage">{message}</p>}

            <div className="inventoryLayout">
                <aside className="inventorySidebar pagePanel">
                    <AddInventoryItem onAdd={handleAdd} />
                </aside>

                <section className="inventoryCollection pagePanel">
                    <div className="inventoryCollectionHeader">
                        <h2>Your ingredients</h2>
                        <p>Everything here can be reused by the recipe curator.</p>
                    </div>

                    {inventory.length === 0 ? (
                        <div className="inventoryEmptyState">
                            <h3>Your shelves are still empty.</h3>
                            <p>Add a few basics like eggs, pasta, tomatoes, garlic, or olive oil to start generating more interesting matches.</p>
                        </div>
                    ) : (
                        <Grid container spacing={2.2}>
                            {inventory.map((item) => (
                                <Grid key={item.userInventoryId} size={{ xs: 12, sm: 6, xl: 4 }}>
                                    <InventoryItemCard
                                        inventoryItem={item}
                                        ingredient={item}
                                        onDelete={handleDelete}
                                    />
                                </Grid>
                            ))}
                        </Grid>
                    )}
                </section>
            </div>
        </section>
    );
}
