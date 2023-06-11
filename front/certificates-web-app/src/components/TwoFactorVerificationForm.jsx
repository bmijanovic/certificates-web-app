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
    const [code, setCode] = useState("");
    const [verificationType, setVerificationType] = useState("email");
    const navigate = useNavigate();
    const [error, setError] = useState("");

    const [dialogOpen, setDialogOpen] = React.useState(false);
    const verifyCode = () => {
        axios.post(environment + `/api/User/verifyTwoFactor/`+code)
            .then(res => {
                if (res.status === 200){
                    navigate(0);
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
                    marginTop: 8,
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                }}
            >
                <Typography component="h1" variant="h3" sx={{textAlign:"Center"}}>
                    Two factor verification
                </Typography>

                <Box component="form" onSubmit={handleSubmit} sx={{ mt: 3 }}>
                    <FormControl
                        sx={{mt: 1, mb:1,display:'flex',justifyContent:"center"}}>
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
                {"Verification code is sent to your " + verificationType + " inbox!"}
            </DialogTitle>
            <DialogContent sx={{display:"flex",justifyContent:"center",flexDirection:"column"}}>
                <DialogContentText id="alert-dialog-description"
                sx={{mb:1}}>
                    Please enter your two factor verification code:
                </DialogContentText>

                <TextField
                    id="Code"
                    name="Code"
                    type="number"
                    sx={{justifyContent:"center",alignItems:"center",alignSelf:"center"}}
                    onInput = {(e) =>{
                        e.target.value = Math.max(0, parseInt(e.target.value) ).toString().slice(0,8)
                    }}
                    InputProps={{style: {fontSize: 30, width: 180}}}
                    onChange={(e) => {setCode(e.target.value)}}
                />
                <div>
                    <InputLabel style={{color:"red"}} sx={{mt:2}}>{error}</InputLabel>
                </div>

            </DialogContent>
            <DialogActions style={{display:"flex", justifyContent:"center"}}>
                <Button onClick={verifyCode} variant="contained">Confirm</Button>
            </DialogActions>
        </Dialog>
    </>
}