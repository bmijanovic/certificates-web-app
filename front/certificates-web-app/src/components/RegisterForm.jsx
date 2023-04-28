import React, {useState} from "react";
import {Link, useNavigate} from "react-router-dom";
import axios from "axios";
import {
    Avatar,
    Box,
    Container,
    CssBaseline, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle,
    Grid,
    InputLabel,
    Stack,
    TextField,
    Typography
} from "@mui/material";
import {LockOutlined} from "@mui/icons-material";
import Button from "@mui/material/Button";

export default function RegisterForm() {
    const [email, setEmail] = useState("")
    const [password, setPassword] = useState("")
    const [name, setName] = useState("")
    const [surname, setSurname] = useState("")
    const [telephone, setTelephone] = useState("")
    const [error, setError] = useState("")

    const navigate = useNavigate()

    const [dialogOpen, setDialogOpen] = React.useState(false);

    const redirectToLogin = () => {
        navigate("/login");
    };

    function handleSubmit(event) {
        event.preventDefault()

        axios.post(`https://localhost:7018/api/User/register`, {
            name: name,
            surname: surname,
            email: email,
            password: password,
            telephone: telephone
        }).then(res => {
            if (res.status === 200){
                setDialogOpen(true);
            }
        }).catch((error) => {
            if (error.response?.status !== undefined && error.response.status === 404){
                //setError("Invalid email or password!");
            }
            else if (error.response?.status !== undefined && error.response.status === 400){
                // let errorMsg = "";
                // for(let key in error.response.data.errors) {
                //     let errors = error.response.data.errors[key];
                //     for (let errorKey in errors){
                //         errorMsg += errors[errorKey] + "\n";
                //     }
                // }
                // setError(errorMsg);
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
                        <InputLabel style={{color:"red"}}>{error}</InputLabel>
                    </div>
                    <Button
                        type="submit"
                        fullWidth
                        variant="contained"
                        sx={{ mt: 3, mb: 3 }}
                    >
                        Sign Up
                    </Button>
                    <Stack style={{textAlign:"center"}}>
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
                    Next step is to open your email inbox and follow activation process from there.
                </DialogContentText>
            </DialogContent>
            <DialogActions style={{display:"flex", justifyContent:"center"}}>
                <Button onClick={redirectToLogin} variant="contained">Go to sign in</Button>
            </DialogActions>
        </Dialog>
    </>
}