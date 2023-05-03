import React, {useState} from "react";
import axios from "axios";
import {
    Box,
    Container,
    CssBaseline,
    Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, FormControl, FormControlLabel, FormLabel,
    InputLabel, Radio, RadioGroup,
    TextField,
    Typography
} from "@mui/material";
import Button from "@mui/material/Button";
import {useNavigate} from "react-router-dom";

export default function ForgotPasswordForm() {
    const [input, setInput] = useState("")
    const [error, setError] = useState("")
    const [verificationType, setVerificationType] = useState("email")

    const navigate = useNavigate()

    const [dialogOpen, setDialogOpen] = React.useState(false);

    const redirectToLogin = () => {
        navigate("/login");
    };

    function handleSubmit(event) {
        event.preventDefault()

        if (verificationType === "email"){
            axios.post(`https://localhost:7018/api/User/sendResetPasswordMail/` + input)
                .then(res => {
                    if (res.status === 200){
                        setDialogOpen(true);
                    }
                }).catch((error) => {
                console.log(error);
            });
        }
        else{
            axios.post(`https://localhost:7018/api/User/sendResetPasswordSMS/` + input)
                .then(res => {
                    if (res.status === 200){
                        setDialogOpen(true);
                    }
                }).catch((error) => {
                console.log(error);
            });
        }
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
                    <FormControl
                        sx={{mt: 2, mb:2}}>
                        <FormLabel id="radio-buttons-group-label">Reset with</FormLabel>
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

                    <TextField
                        required
                        fullWidth
                        id="email"
                        label={verificationType === "email" ? "Email address" : "Telephone"}
                        name="email"
                        placeholder={verificationType === "email" ? "(e.g. user@example.com)" : "(e.g. +3811234567)"}
                        autoComplete={verificationType === "email" ? "email" : "tel"}
                        onChange={(e) => {setInput(e.target.value)}}
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
                        Begin reset process
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
                    Next step is to open your email or sms inbox and follow password reset process from there.
                </DialogContentText>
            </DialogContent>
            <DialogActions style={{display:"flex", justifyContent:"center"}}>
                <Button onClick={redirectToLogin} variant="contained">OK</Button>
            </DialogActions>
        </Dialog>
    </>
}