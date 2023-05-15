import { Navigate } from "react-router-dom";
import {useContext} from "react";
import {AuthContext} from "./AuthContext.jsx";
export const AuthenticatedRoute = ({ children }) => {
    const { isAuthenticated, isTwoFactorVerified, isPasswordExpired
        , isLoading } = useContext(AuthContext);

    if (isLoading) {
        return <div>Loading...</div>;
    }

    if (isAuthenticated && isTwoFactorVerified && isPasswordExpired) {
        //navigate to reset password
    }
    else if (isAuthenticated && !isTwoFactorVerified){
        return <Navigate to="/twoFactor" />;
    }
    else if (!isAuthenticated){
        return <Navigate to="/login" />
    }

    return children;
};