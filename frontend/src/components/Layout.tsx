import React from 'react';
import { Link as RouterLink, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '@/context/AuthContext';
import { useGame } from '@/context/GameContext';
import { Button } from "@/components/ui/button"
import { LogOut, Home, ArrowLeft } from 'lucide-react'
import { Toaster } from "@/components/ui/toaster";
import {GAME_STATUS} from "@/interfaces/GameTypes";


const Layout: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const { user, logout } = useAuth();
    const { isInGame, currentGame } = useGame();
    const location = useLocation();
    const navigate = useNavigate();

    const handleReturnToGame = () => {
        if (currentGame) {
            if (currentGame.status === GAME_STATUS.InProgress) {
                navigate(`/game/${currentGame.id}`);
            } else {
                navigate(`/lobby/${currentGame.id}`);
            }
        }
    };

    return (
        <div className="min-h-screen flex flex-col">
            <header className="bg-primary text-primary-foreground shadow-md">
                <div className="container mx-auto px-4">
                    <div className="flex items-center justify-between h-16">
                        <h1 className="text-2xl font-bold">Activity Game</h1>
                        {user && (
                            <nav className="flex items-center space-x-4">
                                <Button
                                    variant={location.pathname === '/' ? 'secondary' : 'ghost'}
                                    asChild
                                >
                                    <RouterLink to="/">
                                        <Home className="mr-2 h-4 w-4"/>
                                        Main
                                    </RouterLink>
                                </Button>
                                {isInGame && (
                                    <Button variant="ghost" onClick={handleReturnToGame}>
                                        <ArrowLeft className="mr-2 h-4 w-4"/>
                                        Return to Game
                                    </Button>
                                )}
                                <Button variant="ghost" onClick={logout}>
                                    <LogOut className="mr-2 h-4 w-4"/>
                                    Logout
                                </Button>
                            </nav>
                        )}
                    </div>
                </div>
            </header>
            <main className="flex-grow container mx-auto px-4 py-8">
                {children}
            </main>
            <Toaster/>
        </div>
    );
};

export default Layout;