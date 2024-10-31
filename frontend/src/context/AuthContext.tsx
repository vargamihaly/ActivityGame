import React, { createContext, ReactNode, useContext, useEffect, useState } from "react";
import { useGoogleLogin } from "@react-oauth/google";
import { getMeAsync, postLogoutAsync, postRegisterAsync, setAuthToken } from "@/api/games-api";
import { useToast } from "@/hooks/use-toast";
import {components} from "@/api/activitygame-schema";
type UserResponseApiResponse = components["schemas"]["UserResponseApiResponse"];

interface User {
    email: string;
    id: string;
    isHost: boolean;
    score: number;
    username: string;
}


interface AuthContextType {
    user: User | null;
    loading: boolean;
    login: () => void;
    logout: () => void;
    setHostStatus: (isHost: boolean) => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
    const [user, setUser] = useState<User | null>(null);
    const [loading, setLoading] = useState(true);
    const { toast } = useToast();

    const setHostStatus = (isHost: boolean) => {
        setUser(prevUser =>
            prevUser ? { ...prevUser, data: { ...prevUser, isHost } } : null
        );
    };

    const login = useGoogleLogin({
        onSuccess: async (tokenResponse) => {
            try {
                setAuthToken(tokenResponse.access_token);
                const backendResponse = await postRegisterAsync();
                console.log("Backend response:");
                console.log(backendResponse);
                const response = await getMeAsync();
                console.log("AuthProvider: getMe response", response.data);
                console.log("AuthProvider1: getMe response", response);
                const userData = (response as UserResponseApiResponse);
                if (userData.success) {
                    setUser(userData.data as User);
                    toast({
                        title: "Login Successful",
                        description: "You have successfully logged in.",
                        variant: "default"
                    });
                }
                if (!userData.success) {
                    console.log("Error registering user:", userData.message);
                    setAuthToken(null);
                    toast({
                        title: "Login Failed",
                        description: "An error occurred during login. Please try again.",
                        variant: "destructive"
                    });
                }
            } catch (error) {
                console.error('Error registering user:', error);
                setAuthToken(null);
                toast({
                    title: "Login Failed",
                    description: "An error occurred during login. Please try again.",
                    variant: "destructive"
                });
            }
        },
        onError: (error) => {
            console.error('Google Login Failed:', error);
            toast({
                title: "Google Login Failed",
                description: "An error occurred during Google login. Please try again.",
                variant: "destructive"
            });
        },
    });

    const logout = async () => {
        try {
            await postLogoutAsync();
            setUser(null);
            setAuthToken(null);
            toast({
                title: "Logout Successful",
                description: "You have been logged out successfully.",
                variant: "default"
            });
        } catch (error) {
            console.error('Error logging out:', error);
            toast({
                title: "Logout Error",
                description: "An error occurred during logout. Please try again.",
                variant: "destructive"
            });
        }
    };

    useEffect(() => {
        const checkAuth = async () => {
            console.log("AuthProvider: Checking authentication");
            setLoading(true);
            try {
                const response = await getMeAsync();
                console.log("AuthProvider: getMe response", response.data);
                setUser(response.data as User);
            } catch (error) {
                console.error('AuthProvider: Error checking authentication:', error);
                setUser(null);
                setAuthToken(null);
            } finally {
                setLoading(false);
                console.log("AuthProvider: Authentication check complete, loading:", false);
            }
        };
        checkAuth();
    }, []);

    console.log("AuthProvider: Rendering, user:", user, "loading:", loading);

    return (
        <AuthContext.Provider value={{ user, loading, login, logout, setHostStatus }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (context === undefined) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
};