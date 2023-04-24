import React, {useState} from "react";
import axios from "axios";
import {Link, useNavigate} from "react-router-dom";

export default function LoginForm() {
    const [email, setEmail] = useState("")
    const [password, setPassword] = useState("")
    const [error, setError] = useState("")

    const navigate = useNavigate()
    function submitHandler(event) {
        event.preventDefault()

        axios.post(`https://localhost:7018/api/User/login`, {
            email: email,
            password: password
        }).then(res => {
            if (res.status === 200){
                navigate(0);
            }
        }).catch((error) => {
            setError(error.response.data.message);
            console.log(error);
        });
    }

    return <>
        <h1>Login</h1>
        <form onSubmit={submitHandler}>
            <div>
                <input type={"text"} name="email" onChange={(e) => {setEmail(e.target.value)}}/>
            </div>
            <div>
                <input type={"password"} name="password" onChange={(e) => {setPassword(e.target.value)}}/>
            </div>
            <div>
                <span>{error}</span>
            </div>
            <div>
                <span>Don't have an account? <Link to="/register">register now</Link></span>
            </div>
            <div>
                <button type={"submit"}>Login</button>
            </div>
        </form>
    </>
}