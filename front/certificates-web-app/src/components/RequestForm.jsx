import React, {useState, useRef, useContext, useCallback, useEffect} from "react";
import {useMutation, useQueryClient} from "@tanstack/react-query";
import {useNavigate} from "react-router-dom";
import Button from '@mui/material/Button';
import {
    Checkbox,
    FormControl,
    FormControlLabel,
    FormGroup,
    InputLabel,
    MenuItem,
    Select,
    TextField
} from "@mui/material";
import dayjs from "dayjs";
import {DatePicker} from "@mui/x-date-pickers";
import axios from "axios";
import {AuthContext} from "../security/AuthContext.jsx";
import {environment} from "../security/Environment.jsx";
import {GoogleReCaptchaProvider} from "react-google-recaptcha-v3";
import {GoogleReCaptcha} from "react-google-recaptcha-v3";


export default function RequestForm(callback, deps) {
    const [serialNumber, setSerialNumber] = useState("")
    const [certificateType, setCertificateType] = useState("End")
    const [endDate, setEndDate] = useState("2000-01-01T00:00:00.000000Z")
    const [hashAlgorithm, setHashAlgorithm] = useState("SHA256")
    const [o, setO] = useState("")
    const [ou, setOu] = useState("")
    const [c, setC] = useState("")
    const [flags, setFlags] = useState([])
    const { isAuthenticated, role, isLoading } = useContext(AuthContext);
    const [token, setToken] = useState("")
    const [refreshReCaptcha, setRefreshReCaptcha] = useState(false);
    const navigate = useNavigate()



    const handleVerify = (t) => {
        setToken(t);
    }

    const recaptcha = React.useMemo( () => <GoogleReCaptcha onVerify={handleVerify} refreshReCaptcha={refreshReCaptcha} />, [refreshReCaptcha] );
    function submitHandler(event) {
        event.preventDefault()
        setRefreshReCaptcha(r => !r);
        console.log("token: ", token)
        sendRequest(event)

    }

    function sendRequest(event)
    {
        event.preventDefault();
        axios.post(environment + "/api/CertificateRequest/", {
            parentSerialNumber: serialNumber,
            o: o,
            ou: ou,
            c: c,
            endDate: endDate,
            type: certificateType.toUpperCase(),
            hashAlgorithm: hashAlgorithm,
            flags: flags.sort().join(','),
            token: token,
        }).then(res => {
            if (res.status === 200){
                navigate("/requests");
            }
        }).catch((error) => {
            console.log(error);
        });

    }

    function checkFlag(event) {
        if(event.target.checked)
            setFlags(prevFlags => [...prevFlags, event.target.value]);
        else
            setFlags(prevFlags => prevFlags.filter(flag => flag !== event.target.value))
    }

    return <>
        <div style={{}}>
            <h1 style={{textAlign: "center"}}>Generate Certificate</h1>
            <form style={{textAlign: "center"}} onSubmit={submitHandler}>
                <div>
                    <TextField sx={{m: 1, minWidth: 300}}  type="text" name="parentSerialNumber" label="Parent serial number" variant="outlined" onChange={(e) => {setSerialNumber(e.target.value)}} />
                    <FormControl sx={{ m: 1, minWidth: 150 }}>
                        <InputLabel id="demo-simple-select-helper-label">Type</InputLabel>
                        <Select value={certificateType} label="Type" onChange={(e) => {setCertificateType(e.target.value)}}>
                            {role === "Admin" ? <MenuItem value="Root">Root</MenuItem> : null}
                            <MenuItem value="Intermediate">Intermediate</MenuItem>
                            <MenuItem value="End">End</MenuItem>
                        </Select>
                    </FormControl>
                </div>
                <div>
                    <DatePicker sx={{m: 1, minWidth: 300}} defaultValue={dayjs(Date.now())} onChange={(e) => setEndDate(e.$d.toISOString())} />
                    <FormControl sx={{ m: 1, minWidth: 150 }}>
                        <InputLabel>Hash algorithm</InputLabel>
                        <Select value={hashAlgorithm} label="Hash algorithm" onChange={(e) => {setHashAlgorithm(e.target.value)}}>
                            <MenuItem value="MD5">MD5</MenuItem>
                            <MenuItem value="SHA1">SHA1</MenuItem>
                            <MenuItem value="SHA256">SHA256</MenuItem>
                            <MenuItem value="SHA384">SHA384</MenuItem>
                            <MenuItem value="SHA512">SHA512</MenuItem>
                        </Select>
                    </FormControl>
                </div>
                <div style={{textAlign: "center", display: "flex", flexDirection:"column"}}>
                    <TextField sx={{m: 1, minWidth: 470}} style={{alignSelf: "center"}} type="text" name="o" label="Organisation" variant="outlined" onChange={(e) => {setO(e.target.value)}} />
                    <TextField sx={{m: 1, minWidth: 470}} style={{alignSelf: "center"}} type="text" name="ou" label="Organisation unit" variant="outlined" onChange={(e) => {setOu(e.target.value)}} />
                    <TextField sx={{m: 1, minWidth: 470}} style={{alignSelf: "center"}} type="text" name="c" label="Country" variant="outlined" onChange={(e) => {setC(e.target.value)}} />
                </div>
                <div style={{justifyContent: "center", display: "flex", flexDirection:"row"}}>
                    <FormGroup  sx={{m: 1}} row={false}>
                        <FormControlLabel control={<Checkbox value="0" onChange={(event) => checkFlag(event)}/>} label={"EncipherOnly"}/>
                        <FormControlLabel control={<Checkbox value="2" onChange={(event) => checkFlag(event)}/>} label={"KeyCertSign"}/>
                        <FormControlLabel control={<Checkbox value="4" onChange={(event) => checkFlag(event)}/>} label={"DataEncipherment"}/>
                        <FormControlLabel control={<Checkbox value="6" onChange={(event) => checkFlag(event)}/>} label={"NonRepudiation"}/>
                    </FormGroup>
                    <FormGroup sx={{m: 1}} row={false} style={{alignSelf: "center"}}>
                        <FormControlLabel control={<Checkbox value="1" onChange={(event) => checkFlag(event)}/>} label={"CrlSign"}/>
                        <FormControlLabel control={<Checkbox value="3" onChange={(event) => checkFlag(event)}/>} label={"KeyAgreement"}/>
                        <FormControlLabel control={<Checkbox value="5" onChange={(event) => checkFlag(event)}/>} label={"KeyEncipherment"}/>
                        <FormControlLabel control={<Checkbox value="7" onChange={(event) => checkFlag(event)}/>} label={"DigitalSignature"}/>
                    </FormGroup>
                </div>`
                <Button sx={{mt: 2}} variant="contained" type="submit">Send Request</Button>
                {recaptcha}
            </form>
        </div>
    </>
}