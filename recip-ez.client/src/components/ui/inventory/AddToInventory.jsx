import React, { useEffect, useState } from "react";
import axios from "axios";
import Autocomplete from "@mui/material/Autocomplete";
import TextField from "@mui/material/TextField";

function normalizeIngredientOption(option) {
    if (!option || typeof option !== "object") {
        return null;
    }

    const ingredientId = option.ingredientId ?? option.IngredientId;
    const name = option.name ?? option.Name;

    if (!ingredientId || !name) {
        return null;
    }

    return {
        ingredientId,
        name
    };
}

function AddInventoryItem({ onAdd }) {
    const [ingredients, setIngredients] = useState([]);
    const [ingredientId, setIngredientId] = useState("");
    const [quantity, setQuantity] = useState("");
    const [unit, setUnit] = useState("");
    const [message, setMessage] = useState("");

    useEffect(() => {
        const fetchIngredients = async () => {
            try {
                const response = await axios.get("/api/Inventory/ingredients");
                const ingredientOptions = Array.isArray(response.data)
                    ? response.data.map(normalizeIngredientOption).filter(Boolean)
                    : [];

                setIngredients(ingredientOptions);
            } catch (err) {
                console.error(err);
                setIngredients([]);
                setMessage("Unable to load ingredients for search.");
            }
        };

        fetchIngredients();
    }, []);

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

        if (!ingredientId || !quantity || !unit) {
            setMessage("Choose an ingredient, quantity, and unit before saving.");
            return;
        }

        if (quantity <= 0) {
            setMessage("Quantity must be greater than 0.");
            return;
        }

        const payload = {
            ingredientId: parseInt(ingredientId, 10),
            quantity: parseFloat(quantity),
            unit: unit
        };

        try {
            const response = await axios.post("/api/Inventory/add", payload);
            setMessage(response.data.message);

            if (response.data.success && response.data.inventory) {
                onAdd(response.data.inventory[0]);
            }

            setIngredientId("");
            setQuantity("");
            setUnit("");
        } catch (err) {
            console.error(err);

            if (err.response?.status === 401) {
                setMessage("Log in first to edit your inventory.");
                return;
            }

            setMessage("Something went wrong while adding the ingredient.");
        }
    };

    return (
        <form className="inventoryForm" onSubmit={handleSubmit}>
            <div className="inventoryFormHeader">
                <p>Add ingredient</p>
                <h2>Update your pantry</h2>
                <span>Capture what you already have so recipe curation can work with real kitchen data.</span>
            </div>

            <div className="inventoryFormField">
                <label>Ingredient</label>
                <Autocomplete
                    disablePortal
                    options={ingredients}
                    getOptionLabel={(option) => option?.name ?? ""}
                    isOptionEqualToValue={(option, value) => option.ingredientId === value.ingredientId}
                    value={ingredients.find((item) => item.ingredientId === parseInt(ingredientId, 10)) || null}
                    renderInput={(params) => (
                        <TextField
                            {...params}
                            placeholder="Search for an ingredient"
                            sx={{
                                "& .MuiOutlinedInput-root": {
                                    borderRadius: "18px",
                                    backgroundColor: "rgba(248, 252, 255, 0.96)"
                                }
                            }}
                        />
                    )}
                    onChange={(event, newValue) => {
                        setIngredientId(newValue ? newValue.ingredientId : "");
                    }}
                />
            </div>

            <div className="inventoryFormSplit">
                <div className="inventoryFormField">
                    <label>Quantity</label>
                    <input
                        type="number"
                        step="0.1"
                        value={quantity}
                        placeholder="2"
                        onChange={(e) => setQuantity(e.target.value)}
                    />
                </div>

                <div className="inventoryFormField">
                    <label>Unit</label>
                    <select value={unit} onChange={(e) => setUnit(e.target.value)}>
                        <option value="">Select unit</option>
                        {units.map((entry) => (
                            <option key={entry} value={entry}>
                                {entry}
                            </option>
                        ))}
                    </select>
                </div>
            </div>

            <button className="inventorySubmitButton" type="submit">Add to inventory</button>

            {message && <p className="inventoryFormMessage">{message}</p>}
        </form>
    );
}

export default AddInventoryItem;
