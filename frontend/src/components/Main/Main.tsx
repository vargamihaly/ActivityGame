import React, {useEffect, useState} from 'react';
import {useNavigate} from 'react-router-dom';
import {useAuth} from '@/context/AuthContext';
import {useCreateGame, useJoinGame, useLeaveLobby} from "@/hooks/gameHooks";
import {useToast} from "@/hooks/use-toast";
import GameActionCard from "@/components/Main/GameActionCard";
import UserLoading from "@/components/Main/UserLoading";
import JoinGameDialog from "@/components/Main/JoinGameDialog";
import {useGame} from "@/context/GameContext";
import {Button} from "@/components/ui/button";
import {Card, CardContent, CardHeader, CardTitle} from "@/components/ui/card";
import {GAME_STATUS} from "@/interfaces/GameTypes";
import {isValidGuid} from '@/lib/guidUtils';


const Main: React.FC = () => {
    const navigate = useNavigate();
    const {user} = useAuth();
    const [joinGameId, setJoinGameId] = useState('');
    const [isJoinDialogOpen, setIsJoinDialogOpen] = useState(false);
    const {currentGame, isInGame, refreshGameDetails} = useGame();
    const {toast} = useToast();

    const createGameMutation = useCreateGame();
    const joinGameMutation = useJoinGame();
    const leaveLobbyMutation = useLeaveLobby();

    // Logging the initial state and context
    useEffect(() => {
        console.log('Main component mounted');
        console.log('User:', user);
        console.log('Current game:', currentGame);
        console.log('Is in game:', isInGame);
    }, [user, currentGame, isInGame]);

    const handleCreateGame = () => {
        console.log('Create game clicked');
        if (isInGame) {
            console.log('Already in a game');
            let isInLobby = currentGame?.status === GAME_STATUS.Waiting;
            console.log('Is in lobby:', isInLobby);

            if (isInLobby) {
                let confirmNewGame = window.confirm("You are already in a lobby. Creating a new game will remove you from the current lobby. Are you sure you want to continue?");
                console.log('User confirmed new game:', confirmNewGame);

                if (!confirmNewGame) {
                    return;
                } else {
                    console.log('Leaving current lobby...');
                    leaveLobbyMutation.mutateAsync(currentGame!.id).then(() => {
                        console.log('Left lobby successfully');
                    }).catch((error) => {
                        console.error('Error leaving lobby:', error);
                    });
                }
            } else {
                console.log('Already in an active game, showing error toast');
                toast({
                    title: "Error",
                    description: "You are already in a game.",
                    variant: "destructive",
                });
                return;
            }
        }

        console.log('Creating new game...');
        createGameMutation.mutate(undefined, {
            onSuccess: async (response) => {
                const gameId = response.data?.gameId;
                console.log('Create game response:', response.data);
                if (gameId) {
                    console.log('Game created successfully, refreshing game details');
                    refreshGameDetails();
                    navigate(`/lobby/${gameId}`);
                }
            },
            onError: (error) => {
                console.error('Error creating game:', error);
            }
        });
    }

    const handleJoinGame = () => {
        console.log('Join game clicked');
        let isValid = isValidGuid(joinGameId);
        console.log('Is valid game ID:', isValid);

        if (!isValid) {
            toast({
                title: "Error",
                description: "Please enter a valid game ID",
                variant: "destructive",
            });
            return;
        }

        if (isInGame) {
            console.log('Already in a game');
            let isInLobby = currentGame?.status === GAME_STATUS.Waiting;
            console.log('Is in lobby:', isInLobby);

            if (isInLobby) {
                let confirmNewGame = window.confirm("You are already in a lobby. Joining a new game will remove you from the current lobby. Are you sure you want to continue?");
                console.log('User confirmed joining new game:', confirmNewGame);

                if (!confirmNewGame) {
                    return;
                } else {
                    console.log('Leaving current lobby...');
                    leaveLobbyMutation.mutateAsync(currentGame!.id).then(() => {
                        console.log('Left lobby successfully');
                    }).catch((error) => {
                        console.error('Error leaving lobby:', error);
                    });
                }
            } else {
                console.log('Already in an active game, showing error toast');
                toast({
                    title: "Error",
                    description: "You are already in a game.",
                    variant: "destructive",
                });
                return;
            }
        }

        console.log('Joining game:', joinGameId);
        joinGameMutation.mutate(joinGameId, {
            onSuccess: async () => {
                console.log('Joined game successfully, refreshing game details');
                refreshGameDetails();
                navigate(`/lobby/${joinGameId}`);
            },
            onError: (error) => {
                console.error('Error joining game:', error);
            }
        });
    }

    const handleReturnToGame = () => {
        if (currentGame) {
            console.log('Returning to game:', currentGame.id);
            if (currentGame.status === GAME_STATUS.InProgress) {
                navigate(`/game/${currentGame.id}`);
            } else {
                navigate(`/lobby/${currentGame.id}`);
            }
        }
    };

    if (!user) {
        console.log('No user, rendering UserLoading');
        return <UserLoading />;
    }

    console.log('Rendering main content for user:', user);

    return (
        <div className="container mx-auto p-4">
            <div className="max-w-4xl mx-auto">
                <h1 className="text-3xl font-bold text-center mb-8">Welcome, {user.username}</h1>

                {isInGame && currentGame && (
                    <Card className="mb-8">
                        <CardHeader>
                            <CardTitle>Ongoing Game</CardTitle>
                        </CardHeader>
                        <CardContent>
                            <p>You are currently in a game (ID: {currentGame.id})</p>
                            <p>Game Status: {currentGame.status}</p>
                            <Button onClick={handleReturnToGame} className="mt-4">
                                Return to Game
                            </Button>
                        </CardContent>
                    </Card>
                )}

                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                    <GameActionCard
                        title="Create New Game"
                        description="Start a new game and invite your friends to join."
                        actionText="Create Game"
                        onAction={handleCreateGame}
                        isLoading={createGameMutation.isPending}
                        variant="create"
                        disabled={isInGame && currentGame?.status === GAME_STATUS.InProgress}

                    />
                    <GameActionCard
                        title="Join Existing Game"
                        description="Enter a game ID to join an existing game."
                        actionText="Join Game"
                        onAction={() => setIsJoinDialogOpen(true)}
                        isLoading={false}
                        variant="join"
                        disabled={isInGame && currentGame?.status === GAME_STATUS.InProgress}

                    />
                </div>
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
