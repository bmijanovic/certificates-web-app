import React from "react";
import {useQuery} from "@tanstack/react-query";

export default function CertificateRequestCard({item}) {

    return <>
        <div>
            <div>
                <img/>
                <h4>{item.name}</h4>
                <p>Root</p>
            </div>
            <hr/>
            <p>datum</p>
            <div>
                <h5>Serial Number</h5>
                <h3>3789128931283</h3>
                <button>View Details</button>
                <div>
                    <button>Accept</button>
                    <button>Decline</button>
                </div>
            </div>
        </div>
    </>
}