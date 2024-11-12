// src/components/Main/Main.tsx
import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '@/context/AuthContext';
import { useCreateGame, useJoinGame, useLeaveLobby } from "@/hooks/gameHooks";
import { useToast } from "@/hooks/use-toast";
import { useGame } from "@/context/GameContext";
import { Trophy, Users, Play, LogIn, ArrowRight } from 'lucide-react';
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { GAME_STATUS } from "@/interfaces/GameTypes";
import { isValidGuid } from '@/lib/guidUtils';
import { Statistics } from "@/components/Statistics/Statistics";
import JoinGameDialog from "@/components/Main/JoinGameDialog";
import UserLoading from "@/components/Main/UserLoading";

const Main: React.FC = () => {
    const navigate = useNavigate();
    const { user } = useAuth();
    const [joinGameId, setJoinGameId] = useState('');
    const [isJoinDialogOpen, setIsJoinDialogOpen] = useState(false);
    const { currentGame, isInGame, refreshGameDetails } = useGame();
    const { toast } = useToast();

    const createGameMutation = useCreateGame();
    const joinGameMutation = useJoinGame();
    const leaveLobbyMutation = useLeaveLobby();

    const handleLeaveLobby = async (gameId: string) => {
        try {
            await leaveLobbyMutation.mutateAsync(gameId);
            console.log('Left lobby successfully');
        } catch (error) {
            console.error('Error leaving lobby:', error);
            toast({
                title: "Error",
                description: "Failed to leave current lobby",
                variant: "destructive",
            });
        }
    };

    const confirmLobbyAction = async (): Promise<boolean> => {
        if (!isInGame || !currentGame || currentGame.status !== GAME_STATUS.Waiting) {
            return true;
        }

        const confirmed = window.confirm(
            "You are already in a lobby. This action will remove you from the current lobby. Continue?"
        );

        if (confirmed) {
            await handleLeaveLobby(currentGame.id);
        }

        return confirmed;
    };

    const handleCreateGame = async () => {
        if (isInGame && currentGame?.status === GAME_STATUS.InProgress) {
            toast({
                title: "Active Game in Progress",
                description: "Please finish or leave your current game first.",
                variant: "destructive",
            });
            return;
        }

        if (!(await confirmLobbyAction())) {
            return;
        }

        createGameMutation.mutate(undefined, {
            onSuccess: async (response) => {
                const gameId = response.data?.gameId;
                if (gameId) {
                    await refreshGameDetails();
                    navigate(`/lobby/${gameId}`);
                }
            },
            onError: () => {
                toast({
                    title: "Error",
                    description: "Failed to create game",
                    variant: "destructive",
                });
            }
        });
    };

    const handleJoinGame = async () => {
        if (!isValidGuid(joinGameId)) {
            toast({
                title: "Invalid Game ID",
                description: "Please enter a valid game ID",
                variant: "destructive",
            });
            return;
        }

        if (isInGame && currentGame?.status === GAME_STATUS.InProgress) {
            toast({
                title: "Active Game in Progress",
                description: "Please finish or leave your current game first.",
                variant: "destructive",
            });
            return;
        }

        if (!(await confirmLobbyAction())) {
            return;
        }

        joinGameMutation.mutate(joinGameId, {
            onSuccess: async () => {
                await refreshGameDetails();
                navigate(`/lobby/${joinGameId}`);
            },
            onError: () => {
                toast({
                    title: "Error",
                    description: "Failed to join game",
                    variant: "destructive",
                });
            }
        });
    };

    const handleReturnToGame = () => {
        if (!currentGame) return;

        const path = currentGame.status === GAME_STATUS.InProgress
            ? `/game/${currentGame.id}`
            : `/lobby/${currentGame.id}`;
        navigate(path);
    };

    if (!user) return <UserLoading />;

    return (
        <div className="max-w-6xl mx-auto p-6 space-y-8">
            {/* Welcome Section */}
            <div className="text-center space-y-2">
                <h1 className="text-3xl font-bold text-gray-900">Welcome back, {user.username}</h1>
                <p className="text-gray-600">Ready for your next challenge?</p>
            </div>

            {/* Active Game Alert */}
            {isInGame && currentGame && (
                <Card className="border-blue-200 bg-blue-50">
                    <CardHeader className="pb-2">
                        <CardTitle className="text-lg font-medium text-blue-900">
                            Active Game Session
                        </CardTitle>
                    </CardHeader>
                    <CardContent>
                        <div className="flex items-center justify-between">
                            <div className="space-y-1">
                                <p className="text-blue-900">Status: {currentGame.status}</p>
                                <p className="text-sm text-blue-700">Game ID: {currentGame.id}</p>
                            </div>
                            <Button
                                onClick={handleReturnToGame}
                                className="bg-blue-600 hover:bg-blue-700"
                            >
                                Return to Game
                                <ArrowRight className="ml-2 h-4 w-4" />
                            </Button>
                        </div>
                    </CardContent>
                </Card>
            )}

            {/* Statistics Section */}
            <div className="mb-8">
                <Statistics />
            </div>

            {/* Game Actions */}
            <div className="grid md:grid-cols-2 gap-6">
                <Card className="hover:shadow-lg transition-shadow">
                    <CardContent className="p-6">
                        <div className="space-y-4">
                            <div className="flex items-center space-x-3">
                                <div className="p-2 bg-blue-100 rounded-lg">
                                    <Play className="w-6 h-6 text-blue-600" />
                                </div>
                                <h3 className="text-xl font-semibold">Create New Game</h3>
                            </div>
                            <p className="text-gray-600">Start a new game and invite your friends to join.</p>
                            <Button
                                onClick={handleCreateGame}
                                className="w-full"
                                disabled={isInGame && currentGame?.status === GAME_STATUS.InProgress || createGameMutation.isPending}
                            >
                                Create Game
                            </Button>
                        </div>
                    </CardContent>
                </Card>

                <Card className="hover:shadow-lg transition-shadow">
                    <CardContent className="p-6">
                        <div className="space-y-4">
                            <div className="flex items-center space-x-3">
                                <div className="p-2 bg-green-100 rounded-lg">
                                    <Users className="w-6 h-6 text-green-600" />
                                </div>
                                <h3 className="text-xl font-semibold">Join Existing Lobby</h3>
                            </div>
                            <p className="text-gray-600">Enter a game ID to join a lobby.</p>
                            <Button
                                onClick={() => setIsJoinDialogOpen(true)}
                                className="w-full bg-green-600 hover:bg-green-700"
                                disabled={isInGame && currentGame?.status === GAME_STATUS.InProgress}
                            >
                                Join Game
                            </Button>
                        </div>
                    </CardContent>
                </Card>
            </div>

            <JoinGameDialog
                open={isJoinDialogOpen}
                joinGameId={joinGameId}
                onOpenChange={setIsJoinDialogOpen}
                onJoinGameIdChange={setJoinGameId}
                onJoinGame={handleJoinGame}
                isJoining={joinGameMutation.isPending}
            />
        </div>
    );
};

export default Main;