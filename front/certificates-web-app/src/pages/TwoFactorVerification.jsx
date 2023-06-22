import React, {useContext} from "react";
import TwoFactorVerificationForm from "../components/TwoFactorVerificationForm.jsx";
import {AuthContext} from "../security/AuthContext.jsx";
import {Navigate} from "react-router-dom";

export default function TwoFactorVerification() {
    const { isAuthenticated, isTwoFactorVerified,
        isPasswordExpired, isLoading } = useContext(AuthContext);

    if (isLoading) {
        return <div>Loading...</div>;
    }

    if (isAuthenticated && isTwoFactorVerified){
        return <Navigate to="/home"/>;
    }
    else if (!isAuthenticated){
        return <Navigate to="/login"/>;
    }

    return <TwoFactorVerificationForm/>
}