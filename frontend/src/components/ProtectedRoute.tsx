import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '@/context/AuthContext';

export const ProtectedRoute: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const { user, loading } = useAuth();
    const location = useLocation();

    console.log("ProtectedRoute: Rendering, user:", user, "loading:", loading);

    if (loading) {
        console.log("ProtectedRoute: Still loading, showing loading indicator");
        return <div>Loading...</div>;
    }

    if (!user) {
        console.log("ProtectedRoute: No user, redirecting to login");
        return <Navigate to="/login" state={{ from: location }} replace />;
    }

    console.log("ProtectedRoute: User authenticated, rendering children");
    return <>{children}</>;
};