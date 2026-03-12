import { Button } from "./ui/button"
import {
    Card,
    CardHeader,
    CardTitle,
    CardDescription,
    CardContent,
    CardFooter,
} from "./ui/card"

function Login() {
    return (
        <Card>
            <CardHeader>
                <CardTitle>Login to your account</CardTitle>
                <CardDescription>Enter your email below to login to your account</CardDescription>
            </CardHeader>

            <CardContent>
                <form>
                    <div className="flex-col gap-6">
                        <div>
                            <label>Email</label>
                            <input
                                id="email"
                                type="email"
                                placeholder="abc@example.com"
                                required
                            />
                        </div>
                        <div>
                            <label>Password</label>
                            <input
                                id="password"
                                type="password"
                                required
                            />
                        </div>
                    </div>
                </form>
            </CardContent>
            <CardFooter className="flex-col gap-2">
                <Button type="submit" className="w-full">
                    Login
                </Button>
                <Button variant="outline" className="w-full">
                    Login with Google
                </Button>
            </CardFooter>
        </Card>
    );

}

export default Login