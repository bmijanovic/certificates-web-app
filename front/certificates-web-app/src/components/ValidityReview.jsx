import React, {useEffect, useState} from "react";
import axios from "axios";
import {Link, useNavigate} from "react-router-dom";
import {InputLabel, TextField, Typography} from "@mui/material";
import Button from "@mui/material/Button";

export default function ValidityReview(props) {
    const { endTime, issuer, owner, serialNumber, startTime, type, valid } = props.data;
    const [imageSrc, setImageSrc] = React.useState("./src/assets/Valid.png");
    useEffect(()=>{
        setImageSrc(valid?"./src/assets/Valid.png":"./src/assets/Invalid.png");
    },[])

    return <>
        <div style={{alignItems:"center", display: "flex", flexDirection:"column", margin: "50px"}}>
            <img src={imageSrc} style={{width:180, height:180, margin:"0 auto",display:"flex", borderRadius:"20px"}}/>
            <div style={{margin:"20px"}}>
                {
                    valid ? <Typography color="green" variant="h3" component="h2">Valid</Typography> : <Typography color="error" variant="h3" component="h2">Invalid</Typography>
                }
                <Typography color={"primary"} variant="h4" component="h4">{["Root", "Intermediate", "End"][type]}</Typography>
            </div>
            <div>
                <Typography color={"primary"} variant="h5" component="h5">Valid date range</Typography>
                <Typography color={"secondary"} variant="h6" component="h6">{startTime.split("T")[0]} - {endTime.split("T")[0]}</Typography>
            </div>
            <div style={{marginTop:"50px"}}>
                <Typography color={"primary"} variant="h5" component="h5">Serial number</Typography>
                <Typography color={"secondary"} variant="h6" component="h6">{serialNumber}</Typography>
            </div>
            <div style={{alignItems:"center", display: "flex", flexDirection:"row"}}>
                <div style={{margin:"50px"}}>
                    <Typography color={"primary"} variant="h5" component="h5">Issuer</Typography>
                    <Typography color={"secondary"} variant="h6" component="h6">{issuer}</Typography>
                </div>
                <div style={{margin:"50px"}}>
                    <Typography color={"primary"} variant="h5" component="h5">Owner</Typography>
                    <Typography color={"secondary"} variant="h6" component="h6">{owner}</Typography>
                </div>
            </div>
        </div>
    </>
}