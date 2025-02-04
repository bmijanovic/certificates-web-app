import React, {useState} from "react";
import {Link, useNavigate} from "react-router-dom";
import axios from "axios";
import {
    Avatar,
    Box,
    Container,
    CssBaseline,
    Dialog,
    DialogActions,
    DialogContent,
    DialogContentText,
    DialogTitle,
    FormControl,
    FormControlLabel,
    FormLabel,
    Grid,
    InputLabel,
    Radio, RadioGroup,
    Stack,
    TextField,
    Typography
} from "@mui/material";
import {LockOutlined} from "@mui/icons-material";
import Button from "@mui/material/Button";
import {environment} from "../security/Environment.jsx";
import {GoogleReCaptcha} from "react-google-recaptcha-v3";

export default function RegisterForm() {
    const [email, setEmail] = useState("")
    const [password, setPassword] = useState("")
    const [name, setName] = useState("")
    const [surname, setSurname] = useState("")
    const [telephone, setTelephone] = useState("")
    const [error, setError] = useState("")
    const [verificationType, setVerificationType] = useState("email");
    const [dialogOpen, setDialogOpen] = React.useState(false);
    const [token, setToken] = useState("")
    const [refreshReCaptcha, setRefreshReCaptcha] = useState(false);
    const navigate = useNavigate()
    const googleButtonStyle = {
        color: "gray",
        backgroundImage: 'url(data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMTgiIGhlaWdodD0iMTgiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyI+PGcgZmlsbD0ibm9uZSIgZmlsbC1ydWxlPSJldmVub2RkIj48cGF0aCBkPSJNMTcuNiA5LjJsLS4xLTEuOEg5djMuNGg0LjhDMTMuNiAxMiAxMyAxMyAxMiAxMy42djIuMmgzYTguOCA4LjggMCAwIDAgMi42LTYuNnoiIGZpbGw9IiM0Mjg1RjQiIGZpbGwtcnVsZT0ibm9uemVybyIvPjxwYXRoIGQ9Ik05IDE4YzIuNCAwIDQuNS0uOCA2LTIuMmwtMy0yLjJhNS40IDUuNCAwIDAgMS04LTIuOUgxVjEzYTkgOSAwIDAgMCA4IDV6IiBmaWxsPSIjMzRBODUzIiBmaWxsLXJ1bGU9Im5vbnplcm8iLz48cGF0aCBkPSJNNCAxMC43YTUuNCA1LjQgMCAwIDEgMC0zLjRWNUgxYTkgOSAwIDAgMCAwIDhsMy0yLjN6IiBmaWxsPSIjRkJCQzA1IiBmaWxsLXJ1bGU9Im5vbnplcm8iLz48cGF0aCBkPSJNOSAzLjZjMS4zIDAgMi41LjQgMy40IDEuM0wxNSAyLjNBOSA5IDAgMCAwIDEgNWwzIDIuNGE1LjQgNS40IDAgMCAxIDUtMy43eiIgZmlsbD0iI0VBNDMzNSIgZmlsbC1ydWxlPSJub256ZXJvIi8+PHBhdGggZD0iTTAgMGgxOHYxOEgweiIvPjwvZz48L3N2Zz4=)',
        backgroundColor: 'white',
        backgroundRepeat: 'no-repeat',
        backgroundPosition: '12px 11px',

    }


    const handleVerify = (t) => {
        setToken(t);
    }

    const recaptcha = React.useMemo( () => <GoogleReCaptcha onVerify={handleVerify} refreshReCaptcha={refreshReCaptcha} />, [refreshReCaptcha] );


    const redirectToLogin = () => {
        navigate("/login");
    };
    function submitGoogle(event) {
        window.location.href="https://localhost:7018/api/User/signin-google"
    }

    function handleSubmit(event) {
        event.preventDefault()

        axios.post(environment + `/api/User/register`, {
            name: name,
            surname: surname,
            email: email,
            password: password,
            telephone: telephone,
            verificationType: verificationType,
            token: token
        }).then(res => {
            if (res.status === 200){
                setDialogOpen(true);
            }
        }).catch((error) => {
            console.log(error);
            if (error.response?.status !== undefined && error.response.status === 404){
                setError("Resource not found!");
            }
            else if (error.response?.status !== undefined && error.response.status === 400){
                setError("Invalid input!");
            }
            else{
                setError("An error occurred!");
            }
        });
    }

    return <>
        <Container component="main" maxWidth="xs">
            <CssBaseline />
            <Box
                sx={{
                    marginTop: 0,
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                }}
            >
                <Avatar sx={{ m: 1, bgcolor: 'primary.main' }}>
                    <LockOutlined/>
                </Avatar>
                <Typography component="h1" variant="h3">
                    Sign Up
                </Typography>
                <Box component="form" onSubmit={handleSubmit} sx={{ mt: 3 }}>
                    <Grid container spacing={2}>
                        <Grid item xs={12} sm={6}>
                            <TextField
                                required
                                fullWidth
                                autoFocus
                                id="name"
                                label="First Name"
                                name="name"
                                placeholder={"(e.g. Pera)"}
                                autoComplete="given-name"
                                onChange={(e) => {setName(e.target.value)}}
                            />
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <TextField
                                required
                                fullWidth
                                id="surname"
                                label="Last Name"
                                name="surname"
                                placeholder={"(e.g. Peric)"}
                                autoComplete="family-name"
                                onChange={(e) => {setSurname(e.target.value)}}
                            />
                        </Grid>
                        <Grid item xs={12}>
                            <TextField
                                required
                                fullWidth
                                id="email"
                                label="Email Address"
                                name="email"
                                placeholder={"(e.g. user@example.com)"}
                                autoComplete="email"
                                onChange={(e) => {setEmail(e.target.value)}}
                            />
                        </Grid>
                        <Grid item xs={12}>
                            <TextField
                                required
                                fullWidth
                                name="password"
                                label="Password"
                                type="password"
                                id="password"
                                autoComplete="new-password"
                                onChange={(e) => {setPassword(e.target.value)}}
                            />
                        </Grid>
                        <Grid item xs={12}>
                            <TextField
                                required
                                fullWidth
                                name="telephone"
                                label="Telephone number"
                                id="telephone"
                                placeholder={"(e.g. +3811234567)"}
                                inputProps={{ inputMode: 'numeric', pattern: "^\\+381\\d{1,2}\\d{3,11}$" }}
                                onChange={(e) => {setTelephone(e.target.value)}}
                            />
                        </Grid>
                    </Grid>
                    <div>
                        <InputLabel style={{color:"red"}} sx={{mt: 2}}>{error}</InputLabel>
                    </div>
                    <FormControl
                        sx={{mt: 2}}>
                        <FormLabel id="radio-buttons-group-label">Verify with</FormLabel>
                        <RadioGroup
                            row
                            aria-labelledby="radio-buttons-group-label"
                            name="row-radio-buttons-group"
                            defaultValue="email"
                            onChange={(e) => {setVerificationType(e.target.value)}}
                        >
                            <FormControlLabel value="email" control={<Radio />} label="Email" />
                            <FormControlLabel value="sms" control={<Radio />} label="SMS" />
                        </RadioGroup>
                    </FormControl>
                    <Button
                        type="submit"
                        fullWidth
                        variant="contained"
                        sx={{ mt: 3, mb: 3 }}
                    >
                        Sign Up
                    </Button>
                    <p style={{marginTop:5,marginBottom:5,textAlign:"center"}}>or</p>
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
                    <Stack style={{textAlign:"center",marginBottom:20}}>
                        <Stack>
                            {"Already have an account?"}
                        </Stack>
                        <Stack>
                            <Link to="/login" style={{color:"dodgerblue", textDecoration:"None"}}>
                                {"Sign in"}
                            </Link>
                        </Stack>
                    </Stack>
                </Box>
            </Box>
            {recaptcha}
        </Container>
        <Dialog
            open={dialogOpen}
            aria-labelledby="alert-dialog-title"
            aria-describedby="alert-dialog-description"
        >
            <DialogTitle id="alert-dialog-title">
                {"Congratulations, you signed up successfully!"}
            </DialogTitle>
            <DialogContent>
                <DialogContentText id="alert-dialog-description">
                    Next step is to open your email or sms inbox and follow activation process from there.
                </DialogContentText>
            </DialogContent>
            <DialogActions style={{display:"flex", justifyContent:"center"}}>
                <Button onClick={redirectToLogin} variant="contained">OK</Button>
            </DialogActions>
        </Dialog>
    </>
}