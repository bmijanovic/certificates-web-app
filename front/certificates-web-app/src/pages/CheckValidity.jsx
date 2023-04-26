import React, {useState} from "react";
import {TextField, Typography} from "@mui/material";
import Button from "@mui/material/Button";
import axios from "axios";

export default function CheckValidity(){
    const [serialNumber, setSerialNumber] = useState("");
    const [file, setFile] = useState();
    const [showDetails, setShowDetails] = useState(false);

    function checkValidity(event){
        event.preventDefault();
        if(serialNumber !== "") {
            checkValidityBySerialNumbber(serialNumber)
        }
        else {
            console.log(file)
            if(file.name.split('.')[1] === "crt")
            {
                checkValidityByUploadedFile(file)
            }
        }
    }

    function checkValidityBySerialNumbber(serialNumber) {
        axios.get("https://localhost:7018/api/Certificate/checkValidity" + serialNumber)
            .then((res) => {
                setShowDetails(true);
            }).catch((err) => {

        });
    }

    function checkValidityByUploadedFile(file) {
        const formData = new FormData();
        formData.append('certificate', file);
        axios.post("https://localhost:7018/api/Certificate/checkValidity", formData, {
            headers: {
                "Content-Type": "multipart/form-data"
            }
        }).then((res) => {
            setShowDetails(true);
            console.log(res.data)
        }).catch((err) => {

        });
    }

    return <>
        <div style={{width:"80%", textAlign:"center", margin:"30px auto"}}>
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
    </>
}