import React from "react";
import axios from "axios";
import {useNavigate} from "react-router-dom";

export default function Home() {
    const navigate = useNavigate()

    function logout(event) {
        event.preventDefault()

        axios.post(`https://localhost:7018/api/User/logout`)
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