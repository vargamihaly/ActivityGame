import { useEffect, useRef, useState, useCallback } from 'react';
import * as signalR from '@microsoft/signalr';
import { useGame } from '@/context/GameContext';

const useSignalR = (
    gameId: string | undefined,
    onGameStarted?: () => void,
    onGameEnded?: (winnerId: string) => void
) => {
    const { refreshGameDetails } = useGame();
    const connection = useRef<signalR.HubConnection | null>(null);
    const [isConnected, setIsConnected] = useState(false);
    const reconnectAttempts = useRef(0);
    const mountedRef = useRef(true);

    const startConnection = useCallback(async () => {
        if (!gameId || !connection.current || !mountedRef.current) return;

        try {
            if (connection.current.state === signalR.HubConnectionState.Disconnected) {
                await connection.current.start();
                console.log("SignalR Connected");
                setIsConnected(true);
                reconnectAttempts.current = 0;
            }
        } catch (err) {
            console.error("SignalR Connection Error: ", err);
            reconnectAttempts.current++;
            const delay = Math.min(1000 * Math.pow(2, reconnectAttempts.current), 30000);
            console.log(`Attempting to reconnect in ${delay}ms...`);
            if (mountedRef.current) {
                setTimeout(startConnection, delay);
            }
        }
    }, [gameId]);

    useEffect(() => {
        mountedRef.current = true;
        if (!gameId) return;

        connection.current = new signalR.HubConnectionBuilder()
            .withUrl('http://localhost:8081/hubs/game', {
                skipNegotiation: true,
                transport: signalR.HttpTransportType.WebSockets
            })
            .configureLogging(signalR.LogLevel.Debug)
            .withAutomaticReconnect()
            .build();

        connection.current.onreconnecting((error) => {
            console.log("SignalR Reconnecting:", error);
            setIsConnected(false);
        });

        connection.current.onreconnected((connectionId) => {
            console.log("SignalR Reconnected:", connectionId);
            setIsConnected(true);
        });

        connection.current.onclose((error) => {
            console.log("SignalR Connection closed", error);
            setIsConnected(false);
        });

        connection.current.on('ReceiveGameUpdate', () => {
            console.log("Received game update");
            refreshGameDetails();
        });

        connection.current.on('GameStarted', () => {
            console.log("Game started");
            refreshGameDetails();
            if (onGameStarted) onGameStarted();
        });

        connection.current.on('TurnEnded', () => {
            console.log("Turn ended");
            refreshGameDetails();
        });

        connection.current.on('GameEnded', (winnerId: string) => {
            console.log("Game ended", winnerId);
            refreshGameDetails();
            if (onGameEnded) onGameEnded(winnerId);
        });

        startConnection();

        return () => {
            mountedRef.current = false;
            if (connection.current) {
                connection.current.stop();
            }
        };
    }, [gameId, refreshGameDetails, onGameStarted, onGameEnded, startConnection]);

    const joinGame = useCallback(async () => {
        if (connection.current && gameId && isConnected) {
            try {
                await connection.current.invoke('JoinGame', gameId);
                console.log("Joined game successfully");
            } catch (err) {
                console.error("Error joining game: ", err);
                if (err instanceof Error && err.message.includes("Method does not exist")) {
                    console.warn("JoinGame method does not exist on the server");
                }
            }
        } else {
            console.warn("Cannot join game: Connection not established or game ID missing");
        }
    }, [gameId, isConnected]);

    const leaveGame = useCallback(async () => {
        if (connection.current && gameId && isConnected) {
            try {
                await connection.current.invoke('LeaveGame', gameId);
                console.log("Left game successfully");
            } catch (err) {
                console.error("Error leaving game: ", err);
                if (err instanceof Error && err.message.includes("Method does not exist")) {
                    console.warn("LeaveGame method does not exist on the server");
                }
            }
        } else {
            console.warn("Cannot leave game: Connection not established or game ID missing");
        }
    }, [gameId, isConnected]);


    return { joinGame, leaveGame, isConnected };
};

export default useSignalR;