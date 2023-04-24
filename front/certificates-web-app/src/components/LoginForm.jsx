import React, {useState} from "react";
import axios from "axios";
import {Link, useNavigate} from "react-router-dom";
import {InputLabel, TextField} from "@mui/material";
import Button from "@mui/material/Button";

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
            setError("Invalid email or password!");
        });
    }

    return <>
        <div style={{textAlign: "center", alignItems: "center", margin:"auto"}}>
            <h1>Login</h1>
            <form onSubmit={submitHandler}>
                <div>
                    <TextField sx={{m: 1, minWidth: 300}}  type="text" name="email" label="Email" variant="outlined" onChange={(e) => {setEmail(e.target.value)}} />
                </div>
                <div>
                    <TextField sx={{m: 1, minWidth: 300}}  type="password" name="password" label="Password" variant="outlined" onChange={(e) => {setPassword(e.target.value)}} />
                </div>
                <div>
                    <InputLabel style={{color:"red"}}>{error}</InputLabel>
                </div>
                <div>
                    <InputLabel>Don't have an account? <Link to="/register">Register now</Link></InputLabel>
                </div>
                <div>
                    <Button sx={{mt: 2}} variant="outlined" type="submit">Login</Button>
                </div>
            </form>
        </div>
    </>
}