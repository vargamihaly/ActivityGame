import React, {useState} from 'react';
import {Routes, Route, Navigate, useNavigate} from 'react-router-dom';
import {AuthProvider} from './context/AuthContext';
import {ProtectedRoute} from './components/ProtectedRoute';
import {GameProvider} from './context/GameContext';
import Layout from './components/Layout';
import Login from "@/components/Login/Login";
import Main from "@/components/Main/Main";
import Lobby from "@/components/Lobby/Lobby";
import Game from "@/components/Game/Game";
import GameStats from "@/components/GameStats/GameStats";
import ErrorBoundary from "@/ErrorBoundary";
import {QueryClient, QueryClientProvider} from "@tanstack/react-query";
import {ReactQueryDevtools} from "@tanstack/react-query-devtools";

const App: React.FC = () => {

    const [queryClient] = useState(() => new QueryClient());

    return (
        <QueryClientProvider client={queryClient}>
            <ErrorBoundary>
                <AuthProvider>
                    <GameProvider>
                        <Layout>
                            <Routes>
                                <Route path="/login" element={<Login/>}/>
                                <Route path="/" element={<ProtectedRoute><Main/></ProtectedRoute>}/>
                                <Route path="/lobby/:gameId" element={<ProtectedRoute><Lobby/></ProtectedRoute>}/>
                                <Route path="/game/:gameId" element={<ProtectedRoute><Game/></ProtectedRoute>}/>
                                <Route path="/game-stats/:gameId"
                                       element={<ProtectedRoute><GameStats/></ProtectedRoute>}/>
                                <Route path="*" element={<Navigate to="/"/>}/>
                            </Routes>
                        </Layout>
                    </GameProvider>
                </AuthProvider>
            </ErrorBoundary>
            <ReactQueryDevtools initialIsOpen={false}/>
        </QueryClientProvider>
    );
};

export default App;