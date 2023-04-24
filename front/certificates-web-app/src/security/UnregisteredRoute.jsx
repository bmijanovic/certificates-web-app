import {useContext} from "react";
import {AuthContext} from "./AuthContext.jsx";
import {Navigate} from "react-router-dom";

export const UnregisteredRoute = ({ children }) => {
    const { isAuthenticated } = useContext(AuthContext);
    const { isLoading } = useContext(AuthContext);

    if (isLoading) {
        return <div>Loading...</div>;
    }

    if (isAuthenticated) {
        return <Navigate to="/home" />;
    }

    return children;
};