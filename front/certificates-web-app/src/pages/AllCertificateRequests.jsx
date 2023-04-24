import React from "react";
import {useQuery} from "@tanstack/react-query";
import CertificateRequestCard from "../components/CertificateRequestCard.jsx";
import axios from "axios";
import {Grid} from "@mui/material";


export default function AllCertificateRequests() {
    //https://localhost:7018/api/CertificateRequest
    // const certificateRequestsQuery = useQuery({
    //     queryKey: ["certificateRequest"],
    //     queryFn: () => fetch("https://localhost:7018/api/CertificateRequest").then(res => res.json())
    // })

    const certificateRequestsQuery = useQuery({
        queryKey: ["certificateRequest"],
        queryFn: () => axios.get("https://localhost:7018/api/CertificateRequest").then(res => res.data).catch(err => {console.log(err)})
    })



    return <>
        <div style={{textAlign: "center"}}>
            <h1>Certificate Requests</h1>
            {certificateRequestsQuery.isLoading ? <p>Loading...</p> : <div>
                <Grid container sx={{bx:3}} spacing={5}>
                    {certificateRequestsQuery.data.map(item => <CertificateRequestCard key={item.id} data={item}/>)}
                </Grid>
            </div>
            }
        </div>
    </>
}