import React from 'react'
import ReactDOM from 'react-dom/client'
import {createBrowserRouter, Navigate, RouterProvider} from "react-router-dom";
import GenerateCertificateRequest from "./pages/GenerateCertificateRequest.jsx";
import "./index.css"
import {QueryClient, QueryClientProvider} from "@tanstack/react-query";
import AllCertificateRequests from "./pages/AllCertificateRequests.jsx";
import {ReactQueryDevtools} from "@tanstack/react-query-devtools";
import Login from "./pages/Login.jsx";
import axios from "axios";
import {AuthProvider} from "./security/AuthContext.jsx";
import {AuthenticatedRoute} from "./security/AuthenticatedRoute.jsx";
import Home from "./pages/Home.jsx";
import Register from "./pages/Register.jsx";
import ForgotPassword from "./pages/ForgotPassword.jsx";
import {UnauthenticatedRoute} from "./security/UnauthenticatedRoute.jsx";
import {LocalizationProvider} from "@mui/x-date-pickers";
import {AdapterDayjs} from "@mui/x-date-pickers/AdapterDayjs";
import CheckValidity from "./pages/CheckValidity.jsx";
import {Card, createTheme, ThemeProvider} from "@mui/material";
import AccountActivation from "./pages/AccountActivation.jsx";
import AllCertificates from "./pages/AllCertificates.jsx";
import PasswordReset from "./pages/PasswordReset.jsx";
import Navbar from "./components/Navbar.jsx";
import TwoFactorVerification from "./pages/TwoFactorVerification.jsx";
import PasswordExpired from "./pages/PasswordExpired.jsx";
import {GoogleReCaptcha, GoogleReCaptchaProvider} from "react-google-recaptcha-v3";
import raw from"./a.txt"

axios.defaults.withCredentials = true

const queryClient = new QueryClient({defaultOptions: { queries: {
            staleTime: 1000 * 60 * 2
        }}})

const theme = createTheme({
    palette: {
        primary: {
            main: "#146C94",
        },
        secondary: {
            main: "#19A7CE",
            contrastText: 'white'
        },
    },
});
let key=""
const siteKey = "6LfgOwcmAAAAABwjcpMMve7S2_3OYQi6ai-X6J3p";
fetch(raw)
    .then(r => r.text())
    .then(text => {
        key=text;
    });


const router = createBrowserRouter([
    {path:"/login", element: <UnauthenticatedRoute><Login/></UnauthenticatedRoute>},
    {path:"/register", element: <UnauthenticatedRoute><Register/></UnauthenticatedRoute>},
    {path:"/activateAccount", element: <UnauthenticatedRoute><AccountActivation/></UnauthenticatedRoute>},
    {path:"/passwordReset", element: <UnauthenticatedRoute><PasswordReset/></UnauthenticatedRoute>},
    {path:"/forgotPassword", element: <UnauthenticatedRoute><ForgotPassword/></UnauthenticatedRoute>},
    {path:"/twoFactor", element: <><Navbar/><TwoFactorVerification/></>},
    {path:"/passwordExpired", element: <><Navbar/><PasswordExpired/></>},
    {path:"/home", element: <AuthenticatedRoute><Navbar/></AuthenticatedRoute>},
    {path:"/generate", element: <AuthenticatedRoute><Navbar/><GenerateCertificateRequest/></AuthenticatedRoute>},
    {path:"/requests", element: <AuthenticatedRoute><Navbar/><AllCertificateRequests/></AuthenticatedRoute>},
    {path:"/checkValidity", element: <AuthenticatedRoute><Navbar/><CheckValidity/></AuthenticatedRoute>},
    {path:"/certificates", element: <AuthenticatedRoute><Navbar/><AllCertificates/></AuthenticatedRoute>},
    {path:"*", element: <Navigate to="/home" replace />},
])
ReactDOM.createRoot(document.getElementById('root')).render(
    <React.StrictMode>
        <GoogleReCaptchaProvider reCaptchaKey={key}>
            <ThemeProvider theme={theme}>
                <LocalizationProvider dateAdapter={AdapterDayjs}>
                    <AuthProvider>
                        <QueryClientProvider client={queryClient}>

                            <RouterProvider router={router}>
                            </RouterProvider>
                            <ReactQueryDevtools/>
                        </QueryClientProvider>
                    </AuthProvider>
                </LocalizationProvider>
            </ThemeProvider>
        </GoogleReCaptchaProvider>
    </React.StrictMode>,
)
