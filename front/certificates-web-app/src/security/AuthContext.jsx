import React, { createContext, useState, useEffect } from 'react';
import axios from "axios";

export const AuthContext = createContext({
    isAuthenticated: false,
    isLoading: true
});

export const AuthProvider = ({ children }) => {
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        setIsLoading(true);
        axios.get(`https://localhost:7018/api/User/amIAuthenticated`)
            .then(res => {
                if (res.status === 200){
                    setIsAuthenticated(true);
                }
                else{
                    setIsAuthenticated(false);
                }
                setIsLoading(false);
            })
            .catch((error) => {
                setIsLoading(false);
                console.log(error);
            });
    }, []);


    return (
        <AuthContext.Provider value={{ isAuthenticated, isLoading }}>
            {children}
        </AuthContext.Provider>
    );
};
