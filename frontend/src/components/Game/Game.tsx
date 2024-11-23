import React, {useCallback, useEffect, useState} from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAuth } from '@/context/AuthContext';
import { useGame } from '@/context/GameContext';
import {useEndTurn, useGameDetails, useTimeUp} from '@/hooks/gameHooks';
import { Card, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Loader2, MessageCircle } from 'lucide-react';
import { useToast } from '@/hooks/use-toast';
import useSSE from '@/hooks/useSSE';
import useGameTimer from '@/hooks/useGameTimer';
import CurrentRound from './CurrentRound';
import WinnerSelection from './WinnerSelection';
import TimerDisplay from './TimerDisplay';
import Leaderboard from './Leaderboard';
import ChatBox from './ChatBox';

const Game: React.FC = () => {
    const { gameId } = useParams<{ gameId: string }>();
    const { user } = useAuth();
    const navigate = useNavigate();
    const { currentGame, setCurrentGame, setIsInGame } = useGame();
    const { toast } = useToast();
    const endTurnMutation = useEndTurn();
    const timeUpMutation = useTimeUp();
    const { data: gameDetails, refetch: reFetchGameDetails } = useGameDetails(gameId);
    const [selectedWinnerId, setSelectedWinnerId] = useState<string>('');

    // Function to refresh game details
    const refreshGameDetails = useCallback(async () => {
        await reFetchGameDetails();
    }, [reFetchGameDetails]);

    // const [chatMessages, setChatMessages] = useState<ChatMessage[]>([]);
    
    
    // Initialize the game timer
    const { timeLeft, progress, startTimer, resetTimer } = useGameTimer({
        initialTime: currentGame?.timerInMinutes ? currentGame.timerInMinutes * 60 : 0,
        onTimeUp: async () => {
            if (isActivePlayer) {
                try {
                    await handleTimeUp();
                    toast({
                        title: "Time's up!",
                        description: 'Starting next round...'
                    });
                } catch (error) {
                    console.error('Error handling time up:', error);
                    toast({
                        title: 'Error',
                        description: 'Failed to handle time up',
                        variant: 'destructive'
                    });
                }
            }
        },
    });

    const handleTimeUp = async () => {
        if (!gameId) return;

        try {
            await timeUpMutation.mutateAsync(gameId);
            await refreshGameDetails();
        } catch (error) {
            console.error('Error handling time up:', error);
            throw error;
        }
    };

    // Event handler when the game ends
    const onGameEnded = useCallback(() => {
        toast({
            title: 'Game Over',
            description: 'The game has ended! Redirecting to stats page...',
        });
        setIsInGame(false);
        setCurrentGame(null);
        navigate(`/game-stats/${gameId}`);
    }, [toast, setIsInGame, setCurrentGame, navigate, gameId]);

    const onTurnEnded = useCallback(() => {
        console.log('Turn ended, resetting timer and selected winner');
        resetTimer();
        setSelectedWinnerId('');
        refreshGameDetails();
    }, [resetTimer, refreshGameDetails]);

    // Set up SSE to listen to server events
    const { isConnected, error: sseError } = useSSE(gameId, {
        onGameEnded,
        onTurnEnded,
        refreshGameDetails,
    });

    // Fetch game details on component mount and when gameId changes
    useEffect(() => {
        if (gameId) {
            reFetchGameDetails();
        }
    }, [gameId, reFetchGameDetails]);

    // Update currentGame state and reset/start timer when gameDetails change
    useEffect(() => {
        if (gameDetails) {
            setCurrentGame(gameDetails);
            resetTimer(gameDetails.timerInMinutes * 60);
            startTimer();
        }
    }, [gameDetails, setCurrentGame, resetTimer, startTimer]);

    // Handle ending the turn
    const handleEndTurn = async () => {
        if (!selectedWinnerId || !gameId) {
            toast({
                title: 'Error',
                description: 'Please select a winner before ending the turn.',
                variant: 'destructive',
            });
            return;
        }

        try {
            await endTurnMutation.mutateAsync({ gameId, request: { winnerUserId: selectedWinnerId } });
            toast({
                title: 'Success',
                description: 'Turn ended successfully. New turn started!',
            });
            setSelectedWinnerId('');
            refreshGameDetails();
        } catch (error) {
            console.error('Error ending turn:', error);
            toast({
                title: 'Error',
                description: 'Failed to end turn. Please try again.',
                variant: 'destructive',
            });
        }
    };

    if (!currentGame) {
        return (
            <div className="flex justify-center items-center h-screen">
                <Loader2 className="h-12 w-12 animate-spin text-primary" />
            </div>
        );
    }

    const currentRoundNumber = currentGame.currentRound?.roundNumber ?? 1;
    const isActivePlayer = user?.username === currentGame.currentRound?.activePlayerUsername;
    console.log('currentGame:', currentGame);
    console.log('isActivePlayer:', isActivePlayer);
    console.log('userId:', user?.id);
    console.log('activePlayerUsername:', currentGame.currentRound?.activePlayerUsername);

    return (
        <div className="container mx-auto p-4 max-w-6xl">
            <div className="mb-8 text-center">
                <h1 className="text-4xl font-bold mb-2">Activity Game</h1>
                <p className="text-xl text-gray-600">
                    Round {currentRoundNumber} of {currentGame.maxScore}
                </p>
                {!isConnected && (
                    <p className="text-red-500 mt-2">Disconnected from server. Attempting to reconnect...</p>
                )}
                {sseError && (
                    <p className="text-red-500 mt-2">Error: {sseError}</p>
                )}
            </div>

            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                <div className="lg:col-span-2 space-y-6">
                    <Card>
                        <CardContent className="p-6">
                            <CurrentRound
                                methodType={currentGame.currentRound?.methodType!}
                                word={currentGame.currentRound?.word ?? ''}
                                activePlayerUsername={currentGame.currentRound?.activePlayerUsername ?? ''}
                                isActivePlayer={isActivePlayer}
                            />

                            {/* Winner Selection section - Only shown when user is the active player */}
                            {isActivePlayer && (
                                <div className="mt-6 border-t pt-6">
                                    {timeLeft > 0 ? (
                                        <WinnerSelection
                                            players={currentGame.players!}
                                            activePlayerUsername={currentGame.currentRound?.activePlayerUsername ?? ''}
                                            selectedWinnerId={selectedWinnerId}
                                            onWinnerSelect={setSelectedWinnerId}
                                            onEndTurn={handleEndTurn}
                                            isEndingTurn={endTurnMutation.isPending}
                                        />
                                    ) : (
                                        <Button
                                            onClick={handleTimeUp}
                                            className="w-full"
                                        >
                                            Start Next Round
                                        </Button>
                                    )}
                                </div>
                            )}
                        </CardContent>
                    </Card>

                    <Card>
                        <CardContent className="p-6">
                            <h3 className="text-xl font-semibold mb-4 flex items-center">
                                <MessageCircle className="mr-2 h-5 w-5" />
                                Game Chat
                            </h3>
                            <div className="bg-secondary/10 p-4 rounded-lg h-32 mb-4">
                                <p className="text-gray-500 italic">Chat functionality coming soon...</p>
                            </div>
                            <div className="flex">
                                <input
                                    type="text"
                                    placeholder="Type your message..."
                                    className="flex-grow mr-2 p-2 border rounded"
                                    disabled
                                />
                                <Button disabled>Send</Button>
                            </div>
                        </CardContent>
                    </Card>
                </div>

                <div className="space-y-6">
                    <Card>
                        <CardContent className="p-6">
                            <TimerDisplay timeLeft={timeLeft} progress={progress} />
                        </CardContent>
                    </Card>
                    <Card>
                        <CardContent className="p-6">
                            <Leaderboard players={currentGame.players ?? []} currentUserId={user?.id ?? ''} />
                        </CardContent>
                    </Card>
                </div>
            </div>
        </div>
    );
};

export default Game;