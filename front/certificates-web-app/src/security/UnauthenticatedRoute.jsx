import {useContext} from "react";
import {AuthContext} from "./AuthContext.jsx";
import {Navigate} from "react-router-dom";

export const UnauthenticatedRoute = ({ children }) => {
    const { isAuthenticated, isTwoFactorVerified, isPasswordExpired,
        isLoading } = useContext(AuthContext);
    if (isLoading) {
        return <div>Loading...</div>;
    }

    if (isAuthenticated && isTwoFactorVerified && !isPasswordExpired) {
        return <Navigate to="/home" />;
    }
    else if (isAuthenticated && !isTwoFactorVerified){
        return <Navigate to="/twoFactor" />;
    }
    else if (isAuthenticated && isTwoFactorVerified && isPasswordExpired) {
        return <Navigate to="/passwordExpired" />;
    }

    return children;
};