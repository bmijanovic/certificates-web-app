import React, {useState} from "react";
import {TextField, Typography} from "@mui/material";
import Button from "@mui/material/Button";
import axios from "axios";
import ValidityReview from "../components/ValidityReview.jsx";
import {environment} from "../security/Environment.jsx";

export default function CheckValidity(){
    const [serialNumber, setSerialNumber] = useState("");
    const [file, setFile] = useState();
    const [showDetails, setShowDetails] = useState(false);
    const [details, setDetails] = useState();

    function checkValidity(event){
        event.preventDefault();
        if(serialNumber !== "") {
            checkValidityBySerialNumbber(serialNumber)
        }
        else {
            if(file.name.split('.')[1] === "crt" && file.type === "application/x-x509-ca-cert")
            {
                checkValidityByUploadedFile(file)
            }
        }
    }

    function checkValidityBySerialNumbber(serialNumber) {
        axios.get(environment + "/api/Certificate/checkValidity/" + serialNumber)
            .then((res) => {
                setShowDetails(true);
                console.log(res.data)
                setDetails(res.data)
            }).catch((err) => {

        });
    }

    function checkValidityByUploadedFile(file) {
        const formData = new FormData();
        formData.append('certificate', file);
        axios.post(environment + "/api/Certificate/checkValidity", formData, {
            headers: {
                "Content-Type": "multipart/form-data"
            }
        }).then((res) => {
            setShowDetails(true);
            console.log(res.data)
            setDetails(res.data)
        }).catch((err) => {

        });
    }

    return <>
        <div style={{width:"100%", textAlign:"center", margin:"30px auto", display:"flex", flexDirection:"row"}}>
            <div style={{marginRight: "50px"}}>
                <Typography variant="h4" component="h3">Check validity</Typography>
                <div style={{margin:"100px"}}>
                    <TextField sx={{minWidth: 470}} style={{alignSelf: "center"}} type="text" name="serialNumber" label="Serial number" variant="outlined" onChange={(e) => {setSerialNumber(e.target.value)}} />
                    <div style={{display: "flex", alignItems: "center", margin: "auto", justifyContent: "center"}}>
                        <div style={{width: "220px", verticalAlign:"auto"}}>
                            <hr style={{backgroundColor:"#146C94", height:1, margin:"auto"}}/>
                        </div>
                        <p style={{color:"#146C94", margin: "15px 10px"}}>or</p>
                        <div style={{width: "220px", verticalAlign:"auto"}}>
                            <hr style={{backgroundColor:"#146C94", height:1, margin:"auto"}}/>
                        </div>
                    </div>

                    <input
                        style={{ display: "none" }}
                        id="contained-button-file"
                        type="file"
                        onChange={(e) => setFile(e.target.files[0])}
                    />
                    <label htmlFor="contained-button-file">
                        <Button variant="contained" color="secondary" component="span">
                            Upload file
                        </Button>
                    </label>
                </div>
                <div>
                    <Button variant="contained" color="primary" component="span" onClick={checkValidity}>
                        Check validity
                    </Button>
                </div>
            </div>
            <div style={{marginLeft: "50px"}}>
                {
                    showDetails ?
                        <ValidityReview data={details}></ValidityReview>
                        : null
                }`
            </div>
        </div>
    </>
}