import React from "react";
import {useQuery} from "@tanstack/react-query";
import {Card, CardContent, Grid, Typography} from "@mui/material";
import Button from "@mui/material/Button";

export default function CertificateRequestCard(props) {
    const { id, type, state, hashAlgorithm, endDate, flags, parentSerialNumber } = props.data;
    const flagsNames = ["EncipherOnly", "CrlSign", "KeyCertSign", "KeyAgreement", "DataEncipherment", "KeyEncipherment", "NonRepudiation", "DigitalSignature"]

    function makeStringOfFlags(flagsStr) {
        let flagsNumbers = flagsStr.split(",");
        let str = ""
        for(let flagNum in flagsNumbers)
            str += flagsNames[flagNum] + '\n';
        return str
    }

    return <>
        <Grid item xs={12} sm={6} md={4}>
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
                                    <Button variant="contained" color="success" sx={{m:1}}>ACCEPT</Button>
                                    <Button variant="contained" color="error" sx={{m:1}}>DECLINE</Button>
                                </div>
                                : null
                        }
                    </CardContent>
                </Card>
            </div>
        </Grid>
    </>
}