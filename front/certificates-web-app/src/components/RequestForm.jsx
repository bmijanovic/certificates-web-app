import React, {useState, useRef} from "react";
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


export default function RequestForm() {
    const [serialNumber, setSerialNumber] = useState("")
    const [certificateType, setCertificateType] = useState("Root")
    const [endDate, setEndDate] = useState("2000-01-01T00:00:00.000000Z")
    const [hashAlgorithm, setHashAlgorithm] = useState("SHA256")
    const [o, setO] = useState("")
    const [ou, setOu] = useState("")
    const [c, setC] = useState("")
    const [flags, setFlags] = useState([])
    //staviti za svako polje

    const queryClient = useQueryClient()
    const navigate = useNavigate()

    function submitHandler(event) {
        event.preventDefault()
        console.log(serialNumber)
        console.log(certificateType.toUpperCase())
        console.log(endDate)
        console.log(hashAlgorithm)
        console.log(o)
        console.log(ou)
        console.log(c)
        console.log(flags.sort().join(','))
        // requestCertificateMutation.mutate()
        sendRequest(event)
    }

    function sendRequest(event)
    {
        event.preventDefault();
        axios.post("https://localhost:7018/api/CertificateRequest/", {
            parentSerialNumber: serialNumber,
            o: o,
            ou: ou,
            c: c,
            endDate: endDate,
            type: certificateType.toUpperCase(),
            hashAlgorithm: hashAlgorithm,
            flags: flags.sort().join(',')
        }).then(res => {
            if (res.status === 200){
                navigate("/requests");
            }
        }).catch((error) => {
            setError(error.response.data.message);
            console.log(error);
        });
    }

    function checkFlag(event) {
        if(event.target.checked)
            setFlags(prevFlags => [...prevFlags, event.target.value]);
        else
            setFlags(prevFlags => prevFlags.filter(flag => flag !== event.target.value))
    }

    // const requestCertificateMutation = useMutation(() => {
    //     return fetch("https://localhost:7018/api/CertificateRequest/", {
    //         method: "POST",
    //         headers: {
    //             "content-type": "application/json"
    //         },
    //         body: JSON.stringify({
    //             parentSerialNumber: serialNumber,
    //             o: o,
    //             ou: ou,
    //             c: c,
    //             endDate: endDate,
    //             type: certificateType.toUpperCase(),
    //             hashAlgorithm: hashAlgorithm,
    //             flags: flags.sort().join(',')
    //         })
    //     })
    // }, {
    //     onSuccess: () => {
    //         queryClient.invalidateQueries({queryKey: ["certificateRequest"]})
    //         navigate("/requests")
    //     }
    // })



    return <>
        <div style={{}}>
            <h1 style={{textAlign: "center"}}>Generate Certificate</h1>
            <form style={{textAlign: "center"}} onSubmit={submitHandler}>
                <div>
                    <TextField sx={{m: 1, minWidth: 300}}  type="text" name="parentSerialNumber" label="Parent serial number" variant="outlined" onChange={(e) => {setSerialNumber(e.target.value)}} />
                    <FormControl sx={{ m: 1, minWidth: 150 }}>
                        <InputLabel id="demo-simple-select-helper-label">Type</InputLabel>
                        <Select value={certificateType} label="Type" onChange={(e) => {setCertificateType(e.target.value)}}>
                            <MenuItem value=""><em>None</em></MenuItem>
                            <MenuItem value="Root">Root</MenuItem>
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
                            <MenuItem value=""><em>None</em></MenuItem>
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
                </div>
                <Button sx={{mt: 2}} variant="outlined" type="submit">Send Request</Button>
            </form>
        </div>
    </>
}