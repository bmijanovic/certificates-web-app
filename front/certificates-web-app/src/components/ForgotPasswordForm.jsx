import React, {useState} from "react";
import axios from "axios";
import {
    Avatar,
    Box,
    Container,
    CssBaseline,
    Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle,
    Grid,
    InputLabel,
    Stack,
    TextField,
    Typography
} from "@mui/material";
import {LockOutlined} from "@mui/icons-material";
import Button from "@mui/material/Button";
import {Link, useNavigate} from "react-router-dom";

export default function ForgotPasswordForm() {
    const [email, setEmail] = useState("")
    const [error, setError] = useState("")

    const navigate = useNavigate()

    const [dialogOpen, setDialogOpen] = React.useState(false);

    const redirectToLogin = () => {
        navigate("/login");
    };

    function handleSubmit(event) {
        event.preventDefault()

        axios.post(`https://localhost:7018/api/User/sendResetPasswordMail/` + email)
            .then(res => {
            if (res.status === 200){
                setDialogOpen(true);
            }
        }).catch((error) => {
            console.log(error);
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
                <Typography component="h1" variant="h3">
                    Forgot password
                </Typography>
                <Box component="form" onSubmit={handleSubmit} sx={{ mt: 3 }}>
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
                    <div>
                        <InputLabel style={{color:"red"}}>{error}</InputLabel>
                    </div>
                    <Button
                        type="submit"
                        fullWidth
                        variant="contained"
                        sx={{ mt: 3, mb: 3 }}
                    >
                        Send reset password mail
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
                {"Your password reset request submitted successfully!"}
            </DialogTitle>
            <DialogContent>
                <DialogContentText id="alert-dialog-description">
                    Next step is to open your email inbox and follow password reset process from there.
                </DialogContentText>
            </DialogContent>
            <DialogActions style={{display:"flex", justifyContent:"center"}}>
                <Button onClick={redirectToLogin} variant="contained">OK</Button>
            </DialogActions>
        </Dialog>
    </>
}