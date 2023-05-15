import React, {createContext, useState, useEffect, useContext} from 'react';
import axios from "axios";
import {environment} from "./Environment.jsx";

export const AuthContext = createContext({
    isAuthenticated: false,
    isTwoFactorVerified: false,
    isPasswordExpired: true,
    role: null,
    isLoading: true
});

export const AuthProvider = ({ children }) => {
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [isTwoFactorVerified, setIsTwoFactorVerified] = useState(false);
    const [isPasswordExpired, setIsPasswordExpired] = useState(true);
    const [role, setRole] = useState(null);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        setIsLoading(true);
        axios.get(environment + `/api/User/whoAmI`)
            .then(res => {
                if (res.status === 200){
                    setIsAuthenticated(true);
                    setIsTwoFactorVerified(true);
                    setIsPasswordExpired(false);
                    setRole(res.data.role);
                }
                setIsLoading(false);
            })
            .catch((error) => {
                if (error.response.status === 403 && error.response.data.reason === "TwoFactor"){
                    setIsAuthenticated(true);
                }
                else if (error.response.status === 403 && error.response.data.reason === "PasswordExpired"){
                    setIsAuthenticated(true);
                    setIsTwoFactorVerified(true);
                }
                setIsLoading(false);
            });
    }, []);


    return (
        <AuthContext.Provider value={{ isAuthenticated, isTwoFactorVerified, isPasswordExpired, isLoading, role }}>
            {children}
        </AuthContext.Provider>
    );
};
