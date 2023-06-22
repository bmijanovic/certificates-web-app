import React, {useEffect, useState} from "react";
import {Navigate, useNavigate} from "react-router-dom";
import axios from "axios";
import {environment} from "../security/Environment.jsx";
import {
    Box,
    Container,
    CssBaseline,
    Dialog, DialogActions,
    DialogContent, DialogContentText,
    DialogTitle,
    InputLabel,
    TextField,
    Typography
} from "@mui/material";
import Button from "@mui/material/Button";
import {GoogleReCaptcha} from "react-google-recaptcha-v3";

export default function PasswordExpiredForm() {
    const [password, setPassword] = useState("")
    const [passwordConfirmation, setPasswordConfirmation] = useState("")
    const [error, setError] = useState("")

    const [dialogOpen, setDialogOpen] = React.useState(false);
    const navigate = useNavigate()

    function handleSubmit(event) {
        event.preventDefault()

        axios.post(environment + `/api/User/resetPassword/`, {
            password: password,
            passwordConfirmation: passwordConfirmation,
        })
            .then(res => {
                if (res.status === 200) {
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

    const redirectToHome = () => {
        navigate(0);
    }

    return <>
        <Container component="main" maxWidth="xs">
            <CssBaseline/>
            <Box
                sx={{
                    marginTop: 8,
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                }}
            >
                <Typography component="h1" variant="h3">
                    Password expired
                </Typography>
                <Typography component="h1" variant="h5">
                    Please create a new one
                </Typography>
                <Box component="form" onSubmit={handleSubmit} sx={{mt: 3}}>
                    <TextField
                        margin="normal"
                        required
                        fullWidth
                        name="password"
                        label="New password"
                        type="password"
                        id="password"
                        autoComplete="current-password"
                        variant="outlined"
                        onChange={(e) => {
                            setPassword(e.target.value)
                        }}
                    />
                    <TextField
                        margin="normal"
                        required
                        fullWidth
                        name="passwordConfirmation"
                        label="Confirm new password"
                        type="password"
                        id="passwordConfirmation"
                        autoComplete="current-password"
                        variant="outlined"
                        onChange={(e) => {
                            setPasswordConfirmation(e.target.value)
                        }}
                    />
                    <div>
                        <InputLabel style={{color: "red"}} sx={{mt:2}}>{error}</InputLabel>
                    </div>
                    <Button
                        type="submit"
                        fullWidth
                        variant="contained"
                        sx={{mt: 3, mb: 3}}
                    >
                        Confirm
                    </Button>
                </Box>
            </Box>
        </Container>
        <Dialog
            open={dialogOpen}
            aria-labelledby="alert-dialog-title"
            aria-describedby="alert-dialog-description"
        >
            <DialogTitle id="alert-dialog-title">
                {"Your new password is set successfully!"}
            </DialogTitle>
            <DialogContent>
                <DialogContentText id="alert-dialog-description">
                    You can now procceed to home page.
                </DialogContentText>
            </DialogContent>
            <DialogActions style={{display: "flex", justifyContent: "center"}}>
                <Button onClick={redirectToHome} variant="contained">OK</Button>
            </DialogActions>
        </Dialog>
    </>
}