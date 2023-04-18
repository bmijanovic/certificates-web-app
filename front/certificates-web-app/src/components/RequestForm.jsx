import React, {useState, useRef} from "react";
import {useMutation, useQueryClient} from "@tanstack/react-query";
import {useNavigate} from "react-router-dom";


export default function RequestForm() {
    const [serialNumber, setSerialNumber] = useState("")
    const [certificateType, setCertificateType] = useState("ROOT")
    const [endDate, setEndDate] = useState("2000-01-01T00:00:00.000000Z")
    const [hashAlgorithm, setHashAlgorithm] = useState("")
    const [flags, setFlags] = useState([])
    //staviti za svako polje

    const queryClient = useQueryClient()
    const navigate = useNavigate()

    function submitHandler(event) {
        event.preventDefault()
        console.log(flags.sort().join(','))
        console.log(serialNumber)
        console.log(endDate)
        console.log(hashAlgorithm)
        console.log(certificateType.toUpperCase())
        requestCertificateMutation.mutate()
    }

    const requestCertificateMutation = useMutation(() => {
        return fetch("https://localhost:7018/api/CertificateRequest/", {
            method: "POST",
            headers: {
                "content-type": "application/json"
            },
            body: JSON.stringify({
                parentSerialNumber: serialNumber,
                o: "test",
                ou: "string",
                c: "string",
                endDate: endDate,
                type: certificateType,
                hashAlgorithm: hashAlgorithm,
                flags: flags.sort().join(',')
            })
        })
    }, {
        onSuccess: () => {
            queryClient.invalidateQueries({queryKey: ["certificateRequest"]})
            navigate("/requests")
        }
    })



    return <>
        <div>
            <h1>Generate Certificate</h1>
            <form onSubmit={submitHandler}>
                <div>
                    <input type="text" name="parentSerialNumber" onChange={(e) => {setSerialNumber(e.target.value)}}/>
                    <select onChange={(e) => {setCertificateType(e.target.value)}}>
                        <option>Root</option>
                        <option>Intermediate</option>
                        <option>End</option>
                    </select>
                </div>
                <div>
                    <input type="datetime-local" onChange={(e) => setEndDate(e.target.value)}/>
                    <select onChange={(e) => setHashAlgorithm(e.target.value)}>
                        <option>MD5</option>
                        <option>SHA1</option>
                        <option>SHA256</option>
                        <option>SHA384</option>
                        <option>SHA512</option>
                    </select>
                </div>
                <div>
                    <input value="0" onChange={(event) => {if(event.target.checked) setFlags(prevFlags => [...prevFlags, event.target.value]); else setFlags(prevFlags => prevFlags.filter(flag => flag !== event.target.value))}} type="checkbox"/>
                    <input value="1" onChange={(event) => {if(event.target.checked) setFlags(prevFlags => [...prevFlags, event.target.value]); else setFlags(prevFlags => prevFlags.filter(flag => flag !== event.target.value))}} type="checkbox"/>
                    <input value="2" onChange={(event) => {if(event.target.checked) setFlags(prevFlags => [...prevFlags, event.target.value]); else setFlags(prevFlags => prevFlags.filter(flag => flag !== event.target.value))}} type="checkbox"/>
                    <input value="3" onChange={(event) => {if(event.target.checked) setFlags(prevFlags => [...prevFlags, event.target.value]); else setFlags(prevFlags => prevFlags.filter(flag => flag !== event.target.value))}} type="checkbox"/>
                    <input value="4" onChange={(event) => {if(event.target.checked) setFlags(prevFlags => [...prevFlags, event.target.value]); else setFlags(prevFlags => prevFlags.filter(flag => flag !== event.target.value))}} type="checkbox"/>
                    <input value="5" onChange={(event) => {if(event.target.checked) setFlags(prevFlags => [...prevFlags, event.target.value]); else setFlags(prevFlags => prevFlags.filter(flag => flag !== event.target.value))}} type="checkbox"/>
                    <input value="6" onChange={(event) => {if(event.target.checked) setFlags(prevFlags => [...prevFlags, event.target.value]); else setFlags(prevFlags => prevFlags.filter(flag => flag !== event.target.value))}} type="checkbox"/>
                    <input value="7" onChange={(event) => {if(event.target.checked) setFlags(prevFlags => [...prevFlags, event.target.value]); else setFlags(prevFlags => prevFlags.filter(flag => flag !== event.target.value))}} type="checkbox"/>
                </div>
                <button type="submit">Send Request</button>
            </form>
        </div>
    </>
}