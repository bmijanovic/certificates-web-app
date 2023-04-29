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
import {ProtectedRoute} from "./security/ProtectedRoute.jsx";
import Home from "./pages/Home.jsx";
import Register from "./pages/Register.jsx";
import ForgotPassword from "./pages/ForgotPassword.jsx";
import {UnregisteredRoute} from "./security/UnregisteredRoute.jsx";
import {LocalizationProvider} from "@mui/x-date-pickers";
import {AdapterDayjs} from "@mui/x-date-pickers/AdapterDayjs";
import CheckValidity from "./pages/CheckValidity.jsx";
import {createTheme, ThemeProvider} from "@mui/material";
import AccountActivation from "./pages/AccountActivation.jsx";
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

const router = createBrowserRouter([
    {path:"/home", element: <ProtectedRoute><Home/></ProtectedRoute>},
    {path:"/login", element: <UnregisteredRoute><Login/></UnregisteredRoute>},
    {path:"/register", element: <UnregisteredRoute><Register/></UnregisteredRoute>},
    {path:"/activateAccount", element: <UnregisteredRoute><AccountActivation/></UnregisteredRoute>},
    {path:"/forgotPassword", element: <UnregisteredRoute><ForgotPassword/></UnregisteredRoute>},
    {path:"/generate", element: <ProtectedRoute><GenerateCertificateRequest/></ProtectedRoute>},
    {path:"/requests", element: <ProtectedRoute><AllCertificateRequests/></ProtectedRoute>},
    {path:"/checkValidity", element: <ProtectedRoute><CheckValidity/></ProtectedRoute>},
    {path:"*", element: <Navigate to="/home" replace />},
])

ReactDOM.createRoot(document.getElementById('root')).render(
  <React.StrictMode>
      <ThemeProvider theme={theme}>
          <LocalizationProvider dateAdapter={AdapterDayjs}>
              <AuthProvider>
                  <QueryClientProvider client={queryClient}>
                    <RouterProvider router={router}/>
                    <ReactQueryDevtools/>
                  </QueryClientProvider>
              </AuthProvider>
          </LocalizationProvider>
      </ThemeProvider>
  </React.StrictMode>,
)
