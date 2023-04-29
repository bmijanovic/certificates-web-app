import React, {useEffect, useState} from "react";
import {Navigate, useLocation, useNavigate} from "react-router-dom";
import axios from "axios";
import Button from "@mui/material/Button";
import {Box, Container, CssBaseline} from "@mui/material";

export default function AccountActivation() {
    const [activated, setActivated] = useState(null);

    const location = useLocation();
    const navigate = useNavigate();

    function activateAccount(code) {
        axios.post(`https://localhost:7018/api/User/activateAccount/` + code)
            .then(res => {
                if (res.status === 200){
                    setActivated(true)
                }
            }).catch((error) => {
            setActivated(false);
        });
    }

    useEffect(() => {
        const queryParams = new URLSearchParams(location.search);
        const code = queryParams.get('code');
        if (!code){
            navigate("/login");
        }
        activateAccount(code);
    }, []);

    if (activated === null) {
        return <div>Loading...</div>;
    }
    else if (activated === true){
        return <>
            <Container container="main" maxWidth="xs">
                <CssBaseline />
                <Box sx={{
                    marginTop: 8,
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                }}
                >
                    <p style={{textAlign:"center"}}>Account activated successfully. Sign in to access full features!</p>
                    <Button
                        type="submit"
                        fullWidth
                        variant="contained"
                        sx={{mt:3, mb: 3 }}
                    >
                        Back to sign in
                    </Button>
                </Box>
            </Container>
        </>;
    }
    else{
        return <Navigate to="/login"/>
    }

}