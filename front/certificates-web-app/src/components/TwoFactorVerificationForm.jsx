import React, {useState} from "react";
import {useNavigate} from "react-router-dom";
import {
    Box,
    Container,
    CssBaseline, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle,
    FormControl,
    FormControlLabel,
    FormLabel, InputLabel, Radio,
    RadioGroup, TextField,
    Typography
} from "@mui/material";
import Button from "@mui/material/Button";
import axios from "axios";
import {environment} from "../security/Environment.jsx";

export default function TwoFactorVerificationForm() {
    const [code, setCode] = useState("")
    const [verificationType, setVerificationType] = useState("email")
    const navigate = useNavigate();

    const [dialogOpen, setDialogOpen] = React.useState(false);

    const verifyCode = () => {
        axios.post(environment + `/api/User/verifyTwoFactor/`+code)
            .then(res => {
                if (res.status === 200){
                    navigate(0);
                }
            }).catch((error) => {
            console.log(error);
        });

    };

    function handleSubmit(event) {
        event.preventDefault()

        axios.post(environment + `/api/User/sendTwoFactor?verificationType=` + ((verificationType === "email") ? 0 : 1))
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
                    Two factor authentication
                </Typography>

                <Box component="form" onSubmit={handleSubmit} sx={{ mt: 3 }}>
                    <FormControl
                        sx={{mt: 1, mb:1}}>
                        <FormLabel id="radio-buttons-group-label">Get code with</FormLabel>
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
                    >
                        Send code
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
                {"Verification code sent to your " + verificationType + "!"}
            </DialogTitle>
            <DialogContent>
                <DialogContentText id="alert-dialog-description"
                sx={{mb:1}}>
                    Please enter your two factor verification code:
                </DialogContentText>
                <TextField
                    required
                    fullWidth
                    id="Code"
                    name="Code"
                    onChange={(e) => {setCode(e.target.value)}}
                />
            </DialogContent>
            <DialogActions style={{display:"flex", justifyContent:"center"}}>
                <Button onClick={verifyCode} variant="contained">Confirm</Button>
            </DialogActions>
        </Dialog>
    </>
}