import React, { useState, useEffect } from "react";
import axios from "axios";

function AddInventoryItem() {
    const [ingredients, setIngredients] = useState([]);
    const [ingredientId, setIngredientId] = useState("");
    const [quantity, setQuantity] = useState("");
    const [unit, setUnit] = useState("");
    const [message, setMessage] = useState("");

    // 🔹 Fetch ingredients from backend
    useEffect(() => {
        const fetchIngredients = async () => {
            try {
                const response = await axios.get("https://localhost:7111/api/ingredients");
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

        const payload = {
            ingredientId: parseInt(ingredientId),
            quantity: parseFloat(quantity),
            unit: unit
        };

        try {
            await axios.post("https://localhost:7111/api/inventory", payload);
            setMessage("Item added successfully");

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
                <label>Ingredient:</label>
                <select
                    value={ingredientId}
                    onChange={(e) => setIngredientId(e.target.value)}
                >
                    <option value="">Select ingredient</option>
                    {ingredients.map(i => (
                        <option key={i.ingredientId} value={i.ingredientId}>
                            {i.name}
                        </option>
                    ))}
                </select>
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