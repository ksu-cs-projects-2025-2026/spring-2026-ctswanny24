import React, { useState } from "react";
import "./LoginCard.css";
import { Button } from "../button";
import {
    Card,
    CardHeader,
    CardTitle,
    CardDescription,
    CardContent,
} from "../card";
import axios from "axios";

function RegistrationCard() {
    const [firstName, setFirstName] = useState("");
    const [lastName, setLastName] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [message, setMessage] = useState("");

    const handleRegister = async (e) => {
        e.preventDefault();

        if (password !== confirmPassword) {
            setMessage("Passwords do not match.");
            return;
        }

        const registerData = {
            firstName,
            lastName,
            username: email,
            password
        };

        try {
            const response = await axios.post("/api/Login/register", registerData);

            if (response.data.success) {
                setMessage("Registration successful. Your kitchen is ready.");
                window.location.href = "/";
            }
            else {
                setMessage("Registration failed.");
            }
        } catch (error) {
            if (error.response && error.response.data?.message) {
                setMessage(error.response.data.message);
            } else {
                setMessage("An error occurred while creating your account.");
            }
        }
    };

    return (
        <Card className="loginCard">
            <CardHeader className="loginCardHeader">
                <p className="loginCardEyebrow">New account</p>
                <CardTitle className="loginCardTitle">Register for Recip-EZ</CardTitle>
                <CardDescription>
                    Create credentials so Recip-EZ can remember your inventory and recipe matches.
                </CardDescription>
            </CardHeader>

            <CardContent className="loginCardContent">
                <form className="loginForm" action="/api/Login/register" method="post" onSubmit={handleRegister}>
                    <div className="loginNameFields">
                        <div className="loginField">
                            <label htmlFor="firstName">First name</label>
                            <input
                                id="firstName"
                                type="text"
                                value={firstName}
                                placeholder="Alex"
                                onChange={(e) => setFirstName(e.target.value)}
                                required
                            />
                        </div>

                        <div className="loginField">
                            <label htmlFor="lastName">Last name</label>
                            <input
                                id="lastName"
                                type="text"
                                value={lastName}
                                placeholder="Cook"
                                onChange={(e) => setLastName(e.target.value)}
                                required
                            />
                        </div>
                    </div>

                    <div className="loginField">
                        <label htmlFor="email">Email / Username</label>
                        <input
                            id="email"
                            type="email"
                            value={email}
                            placeholder="abc@example.com"
                            onChange={(e) => setEmail(e.target.value)}
                            required
                        />
                    </div>

                    <div className="loginField">
                        <label htmlFor="password">Password</label>
                        <input
                            id="password"
                            type="password"
                            value={password}
                            placeholder="Enter your password"
                            onChange={(e) => setPassword(e.target.value)}
                            required
                        />
                    </div>

                    <div className="loginField">
                        <label htmlFor="confirmPassword">Confirm password</label>
                        <input
                            id="confirmPassword"
                            type="password"
                            value={confirmPassword}
                            placeholder="Re-enter your password"
                            onChange={(e) => setConfirmPassword(e.target.value)}
                            required
                        />
                    </div>

                    {message && <p className="loginMessage">{message}</p>}

                    <Button type="submit" className="loginPrimaryButton">
                        Register
                    </Button>
                </form>
            </CardContent>
        </Card>
    );
}

export default RegistrationCard;
