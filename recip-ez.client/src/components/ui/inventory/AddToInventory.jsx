import React, { useState, useEffect } from "react";
import axios from "axios";
import Autocomplete from '@mui/material/Autocomplete';
import TextField from '@mui/material/TextField';

function AddInventoryItem() {
    const [ingredients, setIngredients] = useState([]);
    const [ingredientId, setIngredientId] = useState("");
    const [quantity, setQuantity] = useState("");
    const [unit, setUnit] = useState("");
    const [message, setMessage] = useState("");

    // 🔹 Fetch ingredients from backend
    useEffect(() => {
        const userId = localStorage.getItem("userId");
        if (!userId) {
            return;
        }

        const fetchIngredients = async () => {
            try {
                const response = await axios.get("https://localhost:7111/api/Inventory/ingredients");
                setIngredients(response.data);
            } catch (err) {
                console.error(err);
            }
        };

        fetchIngredients();
    }, []);

    // 🔹 Unit enum (must match backend exactly)
    const units = [
        "Piece",
        "Cup",
        "Tablespoon",
        "Teaspoon",
        "Gram",
        "Kilogram",
        "Ounce",
        "Pound",
        "Milliliter",
        "Liter"
    ];

    const handleSubmit = async (e) => {
        e.preventDefault();

        // 🔥 Frontend validation
        if (!ingredientId || !quantity || !unit) {
            setMessage("All fields are required");
            return;
        }

        if (quantity <= 0) {
            setMessage("Quantity must be greater than 0");
            return;
        }

        const userId = localStorage.getItem("userId");

        const payload = {
            userId: parseInt(userId),
            ingredientId: parseInt(ingredientId),
            quantity: parseFloat(quantity),
            unit: unit
        };

        try {
            const response = await axios.post("https://localhost:7111/api/Inventory/add", payload);
            setMessage(response.data.message);

            // reset form
            setIngredientId("");
            setQuantity("");
            setUnit("");
        } catch (err) {
            console.error(err);
            setMessage("Error adding item");
        }
    };

    return (
        <form onSubmit={handleSubmit}>
            <h2>Add Inventory Item</h2>

            {/* Ingredient Dropdown */}
            <div>
                <Autocomplete
                    disablePortal
                    options={ingredients}
                    getOptionLabel={(option) => option.name}
                    value={ingredients.find(i => i.ingredientId === parseInt(ingredientId)) || null}
                    renderInput={(params) => (
                        <TextField {...params} label="Ingredient"
                            sx={{
                                input: { color: "white" },
                                label: { color: "white" }
                            }}
                        />
                    )}
                    onChange={(event, newValue) => {
                        setIngredientId(newValue ? newValue.ingredientId : "");
                    }}
                />
            </div>

            {/* Quantity */}
            <div>
                <label>Quantity:</label>
                <input
                    type="number"
                    step="0.1"
                    value={quantity}
                    onChange={(e) => setQuantity(e.target.value)}
                />
            </div>

            {/* Unit Dropdown */}
            <div>
                <label>Unit:</label>
                <select
                    value={unit}
                    onChange={(e) => setUnit(e.target.value)}
                >
                    <option value="">Select unit</option>
                    {units.map(u => (
                        <option key={u} value={u}>
                            {u}
                        </option>
                    ))}
                </select>
            </div>

            <button type="submit">Add Item</button>

            {message && <p>{message}</p>}
        </form>
    );
}

export default AddInventoryItem;