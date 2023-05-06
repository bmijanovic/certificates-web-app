import React, {useState} from "react";
import {useQuery} from "@tanstack/react-query";
import {Box, Card, CardContent, Grid, Modal, TextField, Typography} from "@mui/material";
import Button from "@mui/material/Button";
import axios from "axios";
import {useNavigate} from "react-router-dom";

export default function CertificateRequestCard(props) {
    const { id, type, state, hashAlgorithm, endDate, flags, parentSerialNumber } = props.data;
    const flagsNames = ["EncipherOnly", "CrlSign", "KeyCertSign", "KeyAgreement", "DataEncipherment", "KeyEncipherment", "NonRepudiation", "DigitalSignature"]
    const [open, setOpen] = useState(false);
    const [reason, setReason] = useState("");

    const navigate = useNavigate()

    function makeStringOfFlags(flagsStr) {
        let flagsNumbers = flagsStr.split(",");
        let str = ""
        for(let flagNum in flagsNumbers)
            str += flagsNames[flagNum] + '\n';
        return str
    }

    function declineRequest(event) {
        event.preventDefault();
        axios.post("https://localhost:7018/api/Certificate/decline/" + id, {
            message: reason
        }).then(res => {
            if (res.status === 200){
                navigate("/requests");
            }
        }).catch((error) => {
            console.log(error);
        });
    }

    function acceptRequest(event) {
        event.preventDefault();
        axios.post("https://localhost:7018/api/Certificate/accept/" + id)
            .then(res => {
            if (res.status === 200){
                navigate("/requests");
            }
        }).catch((error) => {
            console.log(error);
        });
    }

    function formatDate(date){
        date=date.split('T')[0].split('-')

        return "".concat(date[2],".",date[1],".",date[0],".")
    }


    return <>
        <Grid item xs={12} sm={6} md={4} style={{paddingTop: "20px"}}>
            <div style={{ display: "flex", alignItems: "center", flexDirection:"column"}}>
                <div style={{width:150, height:150, backgroundColor:"#146C94", position:"relative", top:75, borderRadius:"20px"}}></div>
                <Card>
                    <CardContent style={{height:400, width: 250, marginTop: 80}}>
                        <Typography variant="h5" component="h2" style={{textAlign:"center", marginBottom:20}}>
                            <strong>{type}</strong>
                        </Typography>
                        {
                            parentSerialNumber !== "" ?
                            <Typography color="textSecondary" gutterBottom>
                                Based on certificate:<br/>
                                <strong>{parentSerialNumber}</strong>
                            </Typography> : null
                        }
                        <Typography color="textSecondary" gutterBottom>
                            State:<br/>
                            <strong>{state}</strong>
                        </Typography>
                        <Typography color="textSecondary" gutterBottom>
                            Hash Algorithm:<br/>
                            <strong>{hashAlgorithm}</strong>
                        </Typography>
                        <Typography color="textSecondary" gutterBottom>
                            End Date:<br/>
                            <strong>{endDate.replace("T", " ").split(".")[0]}</strong>
                        </Typography>
                        <Typography color="textSecondary" gutterBottom>
                            Flags:<br/>
                            <strong>{makeStringOfFlags(flags)}</strong>
                        </Typography>
                        {
                            (state === "IN_PROGRESS" && props.acceptable === true) ?
                                <div style={{display: "flex", alignItems: "center", flexDirection:"row", justifyContent:"center"}}>
                                    <Button variant="contained" color="success" sx={{m:1}} onClick={acceptRequest}>ACCEPT</Button>
                                    <Button variant="contained" color="error" sx={{m:1}} onClick={() => setOpen(true)}>DECLINE</Button>
                                    <Modal
                                        open={open}
                                        onClose={() => setOpen(false)}
                                        aria-labelledby="modal-modal-title"
                                        aria-describedby="modal-modal-description"
                                    >
                                        <Box sx={{ position: 'absolute', top: '50%', left: '50%', transform: 'translate(-50%, -50%)', bgcolor: 'background.paper', boxShadow: 24, p: 4, width: 400}}>
                                            <Typography id="modal-modal-title" variant="h6" component="h2">
                                                Enter a reason for declining:
                                            </Typography>
                                            <TextField sx={{my: 1, minWidth: 400}} type="text" name="reason" label="Reason" variant="outlined" onChange={(e) => {setReason(e.target.value)}} />
                                            <Button variant="contained" color="error" sx={{my:1}} onClick={declineRequest}>DECLINE</Button>
                                        </Box>
                                    </Modal>
                                </div>
                                : null
                        }
                    </CardContent>
                </Card>
            </div>
        </Grid>
    </>
}