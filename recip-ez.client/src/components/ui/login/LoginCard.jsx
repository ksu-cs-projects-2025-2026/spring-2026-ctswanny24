
import React, { useState } from "react";
import "./LoginCard.css"
import { Button } from "../button"
import {
    Card,
    CardHeader,
    CardTitle,
    CardDescription,
    CardContent,
    CardFooter,
} from "../card"
import axios from "axios";
function LoginCard() {

    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [message, setMessage] = useState("");

    const handleLogin = async (e) => {
        e.preventDefault();

        const loginData = { username: email, password: password };

        try {
            const response = await axios.post("https://localhost:7111/login", loginData);

            if (response.data.success) {
                setMessage("Login success");
            }
            else {
                setMessage("Login failed");
            }

        } catch (error) {
            if (error.response && error.response.status === 401) {
                setMessage(error.response.data.message);
            } else {
                setMessage("An error occurred");
            }
        }
    }


    return (
        <Card className="card">
            <h2>Log into Recip-EZ</h2>
                <CardHeader>
                    <CardTitle>Login to your account</CardTitle>
                    <CardDescription>Enter your information into the fields below: </CardDescription>
                </CardHeader>

            <CardContent>
                <form onSubmit={handleLogin}>
                        <div className="flex-col gap-6">
                            <div>
                                <label>Email: </label>
                                <input
                                    id="email"
                                    type="email"
                                    value={email}
                                    placeholder="abc@example.com"
                                    onChange={(e) => setEmail(e.target.value)}
                                    required
                                />
                            </div>
                            <div>
                                <label>Password: </label>
                                <input
                                    id="password"
                                    type="password"
                                    value={password}
                                    onChange={(e) => setPassword(e.target.value) }
                                    required
                                />
                            </div>
                    </div>

                    {message && <p className="loginMessage">{message}</p>}

                    <div>
                            <Button type="submit" className="w-full">
                                Login
                            </Button>
                        </div>
                </form>
                </CardContent>
            </Card>
    );

}

export default LoginCard