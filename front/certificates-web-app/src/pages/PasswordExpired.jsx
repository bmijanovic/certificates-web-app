import React, {useContext} from "react";
import {AuthContext} from "../security/AuthContext.jsx";
import {Navigate} from "react-router-dom";
import PasswordExpiredForm from "../components/PasswordExpiredForm.jsx";

export default function PasswordExpired() {
    const { isAuthenticated, isTwoFactorVerified,
        isPasswordExpired, isLoading } = useContext(AuthContext);

    if (isLoading) {
        return <div>Loading...</div>;
    }

    if (isAuthenticated && isTwoFactorVerified && !isPasswordExpired){
        return <Navigate to="/home"/>;
    }
    else if (isAuthenticated && !isTwoFactorVerified){
        return <Navigate to="/twoFactor"/>;
    }
    else if (!isAuthenticated) {
        return <Navigate to="/login"/>;
    }

    return <PasswordExpiredForm/>
}