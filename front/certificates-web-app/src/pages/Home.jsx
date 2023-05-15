import React from "react";
import axios from "axios";
import {useNavigate} from "react-router-dom";
import {environment} from "../security/Environment.jsx";

export default function Home() {
    const navigate = useNavigate()

    function logout(event) {
        event.preventDefault()

        axios.post(environment + `/api/User/logout`)
            .then(res => {
            if (res.status === 200){
                navigate(0)
            }
        }).catch((error) => {
            console.log(error);
        });
    }

    return <>
        {/*<div>Home</div><button onClick={logout}>logout</button>*/}
    </>
}