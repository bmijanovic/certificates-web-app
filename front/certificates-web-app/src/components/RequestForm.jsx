import React, {useState, useRef} from "react";
import {useMutation, useQueryClient} from "@tanstack/react-query";
import {useNavigate} from "react-router-dom";


export default function RequestForm() {
    const [serialNumber, setSerialNumber] = useState("")
    const [flags, setFlags] = useState([])
    console.log(flags)
    //staviti za svako polje

    const queryClient = useQueryClient()
    const navigate = useNavigate()

    function submitHandler(event) {
        event.preventDefault()
        console.log(flags.sort().join(','))
        requestCertificateMutation.mutate()
    }

    const requestCertificateMutation = useMutation(() => {
        return fetch("url apija", {
            method: "POST",
            headers: {
                "content-type": "application/json"
            },
            body: JSON.stringify({
                parentSerialNumber: serialNumber,
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
                    <select>
                        <option>Root</option>
                        <option>Intermediate</option>
                        <option>End</option>
                    </select>
                </div>
                <div>
                    <input/>
                    <select></select>
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