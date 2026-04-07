import React, { useState } from "react";
import {
    Card,
    CardHeader,
    CardTitle,
    CardDescription,
    CardContent,
    CardFooter,
} from "../card"
import axios from "axios";
import Button from '@mui/material/Button';

function InventoryAdditionCard() {
    const [foodName, setName] = useState("");
    const [foodDescription, setDescription] = useState("");
    const [foodExpirationDate, setExpirationDate] = useState("");
    const [message, setMessage] = useState("");


    const handleAddition = async (e) => {
        e.preventDefault();

        const foodItem = {
            name: foodName, description: foodDescription, expirationDate: foodExpirationDate
        };

        try {
            const response = await axios.post("https://localhost:7111/login", foodItem)

            if (response.data.success) {
                setMessage("Success");
            }
            else {
                setMessage("Failure");
            }
        } catch (error) {
            if (error.response && error.response.status === 401) {
                setMessage(error.reponse.data.message);
            }
            else {
                setMessage("error has occurred");
            }
        }
    }

    return (
        <Card className="card">
            <CardHeader>
                <CardTitle>Inventory Addition</CardTitle>
            </CardHeader>

            <CardContent>
                <form onSubmit={handleAddition}>
                    <div className="flex-col gap-6">
                        <div>
                            <label>Food Name</label>
                            <input
                                id="foodName"
                                value={foodName}
                                placeholder="Example: Ground Beef"
                                onChange={(e) => setName(e.target.value)}
                                required
                            />
                        </div>
                        <div>
                            <label>Description</label>
                            <input
                                id="description"
                                value={foodDescription}
                                placeholder="Example: Kroger brand, 4 1lb sleeves"
                                onChange={(e) => setDescription(e.target.value)}
                            />
                        </div>
                        <div>
                            <input
                                type="date-time-local"
                                value={foodExpirationDate}
                                onChange={(e) => setExpirationDate(e.target.value)}
                            />
                        </div>

                        <div>
                            <Button variant="contained" type="submit" size="large"></Button>
                        </div>
                    </div>

                    {message && <p className="loginMessage">{message}</p>}

                    <div>
                        <Button variant="outlined" type="submit" color="error"></Button>
                    </div>
                </form>
            </CardContent>
        </Card>
    );

}

export default InventoryAdditionCard