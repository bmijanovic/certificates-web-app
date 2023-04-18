import React, {useState} from "react";
import {useMutation} from "@tanstack/react-query";

export default function LoginForm() {
    const [email, setEmail] = useState("")
    const [password, setPassword] = useState("")

    function submitHandler(event) {
        event.preventDefault()
        requestCertificateMutation.mutate()
    }

    const requestCertificateMutation = useMutation(() => {
        return fetch("https://localhost:7018/api/User/login", {
            method: "POST",
            headers: {
                "content-type": "application/json"
            },
            body: JSON.stringify({
                email: email,
                password: password
            })
        })
    }, {
        onSuccess: (response) => {
            console.log(response.text());
        }
    })

    return <>
        <h1>Login</h1>
        <form onSubmit={submitHandler}>
            <div>
                <input type={"text"} name="email" onChange={(e) => {setEmail(e.target.value)}}/>
            </div>
            <div>
                <input type={"text"} name="password" onChange={(e) => {setPassword(e.target.value)}}/>
            </div>
            <div>
                <button type={"submit"}>Login</button>
            </div>
        </form>
    </>
}