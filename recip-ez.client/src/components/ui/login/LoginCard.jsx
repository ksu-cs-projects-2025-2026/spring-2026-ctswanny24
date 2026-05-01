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

function LoginCard() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [message, setMessage] = useState("");
    const [users, setUsers] = useState([]);

    const handleLogin = async (e) => {
        e.preventDefault();

        const loginData = { username: email, password: password };

        try {
            const response = await axios.post("https://localhost:7111/api/login", loginData);

            if (response.data.success) {
                setMessage("Login successful. Your kitchen is ready.");
                localStorage.setItem("userId", response.data.userId);
            }
            else {
                setMessage("Login failed.");
            }
        } catch (error) {
            if (error.response && error.response.status === 401) {
                setMessage(error.response.data.message);
            } else {
                setMessage("An error occurred while signing in.");
            }
        }
    };

    const getAllUsers = async (e) => {
        try {
            e.preventDefault();
            const response = await axios.get("https://localhost:7111/api/login/users");
            setUsers(response.data);
        } catch (error) {
            console.error("Error fetching users:", error);
        }
    };

    return (
        <Card className="loginCard">
            <CardHeader className="loginCardHeader">
                <p className="loginCardEyebrow">Account access</p>
                <CardTitle className="loginCardTitle">Log into Recip-EZ</CardTitle>
                <CardDescription>
                    Use your existing credentials to manage inventory and load personalized recipe matches.
                </CardDescription>
            </CardHeader>

            <CardContent className="loginCardContent">
                <form className="loginForm" onSubmit={handleLogin}>
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

                    {message && <p className="loginMessage">{message}</p>}

                    <Button type="submit" className="loginPrimaryButton">
                        Login
                    </Button>
                </form>

                <div className="loginDebugPanel">
                    <Button type="button" variant="outline" onClick={getAllUsers}>
                        Show seeded users
                    </Button>

                    {users.length > 0 && (
                        <div className="loginUserList">
                            {users.map((user) => (
                                <div key={user.userId} className="loginUserRow">
                                    <strong>{user.firstName} {user.lastName}</strong>
                                    <span>{user.username}</span>
                                </div>
                            ))}
                        </div>
                    )}
                </div>
            </CardContent>
        </Card>
    );
}

export default LoginCard;
