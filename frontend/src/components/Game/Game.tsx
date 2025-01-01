import React, {useCallback, useEffect, useState} from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAuth } from '@/context/AuthContext';
import { useGame } from '@/context/GameContext';
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
import {useGameHook} from "@/hooks/gameHooks";

const Game: React.FC = () => {
    const { gameId } = useParams<{ gameId: string }>();
    const { user } = useAuth();
    const navigate = useNavigate();
    const { currentGame, setCurrentGame, setIsInGame } = useGame();
    const { toast } = useToast();

    const { endTurn, timeUp, useGameDetails } = useGameHook();
    const { data: gameDetails, refetch: reFetchGameDetails } = useGameDetails(gameId);
    const [selectedWinnerId, setSelectedWinnerId] = useState<string>('');

    const refreshGameDetails = useCallback(async () => {
        await reFetchGameDetails();
    }, [reFetchGameDetails]);

    // Initialize the timer with initialTime = 0,
    // We'll dynamically reset it after we get the game details.
    const { timeLeft, progress, startTimer, resetTimer } = useGameTimer({
        initialTime: 0,
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
            await timeUp.mutateAsync(gameId);
            await refreshGameDetails();
        } catch (error) {
            console.error('Error handling time up:', error);
            throw error;
        }
    };

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

    const { isConnected, error: sseError } = useSSE(gameId, {
        onGameEnded,
        onTurnEnded,
        refreshGameDetails,
    });

    // Fetch game details on mount/gameId change
    useEffect(() => {
        if (gameId) {
         
            reFetchGameDetails();
        }
    }, [gameId, reFetchGameDetails]);

    // Update currentGame state and timer when gameDetails change
    useEffect(() => {
        if (gameDetails) {
            setCurrentGame(gameDetails);
console.log(gameDetails);
            if (gameDetails.currentRound) {
                const createdAt = new Date(gameDetails.currentRound.createdAtUtc);
                const elapsedSeconds = Math.floor((Date.now() - createdAt.getTime()) / 1000);
                const totalRoundSeconds = gameDetails.timerInMinutes * 60;
                const calculatedTimeLeft = Math.max(totalRoundSeconds - elapsedSeconds, 0);

                resetTimer(calculatedTimeLeft);
                startTimer();
            }
        }
    }, [gameDetails, setCurrentGame, resetTimer, startTimer]);

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
            await endTurn.mutateAsync({ gameId, request: { winnerUserId: selectedWinnerId } });
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

                            {/* Winner Selection section */}
                            {isActivePlayer && (
                                <div className="mt-6 border-t pt-6">
                                    {timeLeft > 0 ? (
                                        <WinnerSelection
                                            players={currentGame.players!}
                                            activePlayerUsername={currentGame.currentRound?.activePlayerUsername ?? ''}
                                            selectedWinnerId={selectedWinnerId}
                                            onWinnerSelect={setSelectedWinnerId}
                                            onEndTurn={handleEndTurn}
                                            isEndingTurn={endTurn.isPending}
                                        />
                                    ) : (
                                        <Button onClick={handleTimeUp} className="w-full">
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
