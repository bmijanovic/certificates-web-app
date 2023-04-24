import React from "react";
import {useQuery} from "@tanstack/react-query";
import CertificateRequestCard from "../components/CertificateRequestCard.jsx";


export default function AllCertificateRequests() {
    //https://localhost:7018/api/CertificateRequest
    const certificateRequestsQuery = useQuery({
        queryKey: ["certificateRequest"],
        queryFn: () => fetch("https://localhost:7018/api/CertificateRequest").then(res => res.json())
    })



    return <>
        <h1>Certificate Requests</h1>
        {certificateRequestsQuery.isLoading ? <p>Loading...</p> : <div>
            {certificateRequestsQuery.data.map(item => <CertificateRequestCard key={item.id} item={item}/>)}
        </div>
        }
    </>
}