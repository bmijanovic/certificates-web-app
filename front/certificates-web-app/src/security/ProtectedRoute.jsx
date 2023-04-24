import { Navigate } from "react-router-dom";
import {useContext} from "react";
import {AuthContext} from "./AuthContext.jsx";
export const ProtectedRoute = ({ children }) => {
    const { isAuthenticated } = useContext(AuthContext);
    const { isLoading } = useContext(AuthContext);

    if (isLoading) {
        return <div>Loading...</div>;
    }

    if (!isAuthenticated) {
        return <Navigate to="/login" />;
    }

    return children;
};