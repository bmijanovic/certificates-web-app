import React, {useState} from "react";
import axios from "axios";
import {Link, useNavigate} from "react-router-dom";
import {
    Avatar,
    Box, Container,
    CssBaseline,
    InputLabel, Stack,
    TextField,
    Typography
} from "@mui/material";
import Button from "@mui/material/Button";
import {LockOutlined} from "@mui/icons-material";

export default function LoginForm() {
    const [email, setEmail] = useState("")
    const [password, setPassword] = useState("")
    const [error, setError] = useState("")

    const googleButtonStyle = {
        color: "gray",
         backgroundImage: 'url(data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMTgiIGhlaWdodD0iMTgiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyI+PGcgZmlsbD0ibm9uZSIgZmlsbC1ydWxlPSJldmVub2RkIj48cGF0aCBkPSJNMTcuNiA5LjJsLS4xLTEuOEg5djMuNGg0LjhDMTMuNiAxMiAxMyAxMyAxMiAxMy42djIuMmgzYTguOCA4LjggMCAwIDAgMi42LTYuNnoiIGZpbGw9IiM0Mjg1RjQiIGZpbGwtcnVsZT0ibm9uemVybyIvPjxwYXRoIGQ9Ik05IDE4YzIuNCAwIDQuNS0uOCA2LTIuMmwtMy0yLjJhNS40IDUuNCAwIDAgMS04LTIuOUgxVjEzYTkgOSAwIDAgMCA4IDV6IiBmaWxsPSIjMzRBODUzIiBmaWxsLXJ1bGU9Im5vbnplcm8iLz48cGF0aCBkPSJNNCAxMC43YTUuNCA1LjQgMCAwIDEgMC0zLjRWNUgxYTkgOSAwIDAgMCAwIDhsMy0yLjN6IiBmaWxsPSIjRkJCQzA1IiBmaWxsLXJ1bGU9Im5vbnplcm8iLz48cGF0aCBkPSJNOSAzLjZjMS4zIDAgMi41LjQgMy40IDEuM0wxNSAyLjNBOSA5IDAgMCAwIDEgNWwzIDIuNGE1LjQgNS40IDAgMCAxIDUtMy43eiIgZmlsbD0iI0VBNDMzNSIgZmlsbC1ydWxlPSJub256ZXJvIi8+PHBhdGggZD0iTTAgMGgxOHYxOEgweiIvPjwvZz48L3N2Zz4=)',
        backgroundColor: 'white',
        backgroundRepeat: 'no-repeat',
        backgroundPosition: '12px 11px',

    }

    const navigate = useNavigate()
    function submitHandler(event) {
        event.preventDefault()

        axios.post(`https://localhost:7018/api/User/login`, {
            email: email,
            password: password
        }).then(res => {
            if (res.status === 200){
                navigate(0);
            }
        }).catch((error) => {
            if (error.response?.status !== undefined && error.response.status === 404){
                setError("Invalid email or password!");
            }
            else if (error.response?.status !== undefined && error.response.status === 400){
                setError("Account is not activated!");
            }
            else{
                setError("An error occurred!");
            }
        });
    }
    function submitGoogle(event) {
        window.location.href="https://localhost:7018/api/User/signin-google"
    }

    return <>
        <Container container="main" maxWidth="xs">
            <CssBaseline />
            <Box
                sx={{
                    marginTop: 8,
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                }}
            >
                <Avatar sx={{ m: 1, bgcolor: 'primary.main' }}>
                    <LockOutlined/>
                </Avatar>
                <Typography component="h1" variant="h3">
                    Sign in
                </Typography>
                <Box component="form" onSubmit={submitHandler} sx={{ mt: 1 }}>
                    <TextField
                        margin="normal"
                        required
                        fullWidth
                        autoFocus
                        type="text"
                        id="email"
                        label="Email Address"
                        name="email"
                        autoComplete="email"
                        variant="outlined"
                        onChange={(e) => {setEmail(e.target.value)}}
                    />
                    <TextField
                        margin="normal"
                        required
                        fullWidth
                        name="password"
                        label="Password"
                        type="password"
                        id="password"
                        autoComplete="current-password"
                        variant="outlined"
                        onChange={(e) => {setPassword(e.target.value)}}
                    />
                    <div style={{textAlign:"center", marginBottom:"5px"}}>
                        <Link to="/forgotPassword" style={{color:"grey", textDecoration:"None"}}>{"Forgot password?"}</Link>
                    </div>
                    <div>
                        <InputLabel style={{color:"red"}}>{error}</InputLabel>
                    </div>
                    <Button
                        type="submit"
                        fullWidth
                        variant="contained"
                        sx={{mt:3, mb: 3 }}
                    >
                        Sign In
                    </Button>
                    <p style={{marginTop:20,marginBottom:20,textAlign:"center"}}>or</p>
                    <Button
                        type="button"
                        onClick={submitGoogle}
                        style={googleButtonStyle}
                        fullWidth
                        variant="contained"
                        sx={{mt:3, mb: 3 }}
                    >
                        Continue with google
                    </Button>
                    <Stack style={{textAlign:"center"}}>
                        <Stack>
                            {"Don't have an account? "}
                        </Stack>
                        <Stack>
                            <Link to="/register" style={{color:"dodgerblue", textDecoration:"None"}}>
                                {"Sign up"}
                            </Link>
                        </Stack>
                    </Stack>
                </Box>
            </Box>
        </Container>
    </>
}