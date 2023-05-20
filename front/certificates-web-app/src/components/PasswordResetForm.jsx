import React, {useEffect, useState} from "react";
import {Navigate, useNavigate} from "react-router-dom";
import axios from "axios";
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
import {environment} from "../security/Environment.jsx";
import {GoogleReCaptcha} from "react-google-recaptcha-v3";

export default function PasswordResetForm() {
    const [password, setPassword] = useState("")
    const [passwordConfirmation, setPasswordConfirmation] = useState("")
    const [code, setCode] = useState(null)
    const [codeExists, setCodeExists] = useState(null)
    const [error, setError] = useState("")

    const [dialogOpen, setDialogOpen] = React.useState(false);
    const [token, setToken] = useState("")
    const [refreshReCaptcha, setRefreshReCaptcha] = useState(false);
    const navigate = useNavigate()



    const handleVerify = (t) => {
        setToken(t);
    }

    const recaptcha = React.useMemo( () => <GoogleReCaptcha onVerify={handleVerify} refreshReCaptcha={refreshReCaptcha} />, [refreshReCaptcha] );


    function checkIfCodeExists(code) {
        axios.get(environment + `/api/User/doesPasswordResetCodeExists/` + code)
            .then(res => {
                if (res.status === 200){
                    setCodeExists(true)
                }
            }).catch((error) => {
            setCodeExists(false);
        });
    }

    useEffect(() => {
        const queryParams = new URLSearchParams(location.search);
        const codeParam = queryParams.get('code');
        if (!codeParam){
            navigate("/login");
        }
        setCode(codeParam);
        checkIfCodeExists(codeParam);
    }, []);

    const redirectToLogin = () => {
        navigate("/login");
    };

    function handleSubmit(event) {
        event.preventDefault()

        axios.post(environment + `/api/User/resetPassword/` + code, {
            password: password,
            passwordConfirmation: passwordConfirmation,
            token: token,
        })
            .then(res => {
                if (res.status === 200){
                    setDialogOpen(true);
                }
            }).catch((error) => {
            console.log(error);
        });
    }

    if (codeExists === null) {
        return <div>Loading...</div>;
    }
    else if (codeExists === true){
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
                    <Typography component="h1" variant="h3">
                        Password reset
                    </Typography>
                    <Box component="form" onSubmit={handleSubmit} sx={{ mt: 3 }}>
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
                            onChange={(e) => {setPassword(e.target.value)}}
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
                        onChange={(e) => {setPasswordConfirmation(e.target.value)}}
                    />
                        <div>
                            <InputLabel style={{color:"red"}}>{error}</InputLabel>
                        </div>
                        <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            sx={{ mt: 3, mb: 3 }}
                        >
                            Reset password
                        </Button>
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
                    {"Your new password is set successfully!"}
                </DialogTitle>
                <DialogContent>
                    <DialogContentText id="alert-dialog-description">
                        You can sign in now with your new password.
                    </DialogContentText>
                </DialogContent>
                <DialogActions style={{display:"flex", justifyContent:"center"}}>
                    <Button onClick={redirectToLogin} variant="contained">OK</Button>
                </DialogActions>
            </Dialog>
        </>
    }
    else{
        return <Navigate to="/login"/>
    }
}