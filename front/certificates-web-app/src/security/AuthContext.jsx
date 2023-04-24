import React, {createContext, useState, useEffect, useContext} from 'react';
import axios from "axios";

export const AuthContext = createContext({
    isAuthenticated: false,
    role: null,
    isLoading: true
});

export const AuthProvider = ({ children }) => {
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [role, setRole] = useState(null);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        setIsLoading(true);
        axios.get(`https://localhost:7018/api/User/whoAmI`)
            .then(res => {
                if (res.status === 200){
                    setIsAuthenticated(true);
                    console.log(res.data.role);
                    setRole(res.data.role);
                }
                else{
                    setIsAuthenticated(false);
                }
                setIsLoading(false);
            })
            .catch((error) => {
                setIsLoading(false);
            });
    }, []);


    return (
        <AuthContext.Provider value={{ isAuthenticated, isLoading, role }}>
            {children}
        </AuthContext.Provider>
    );
};
