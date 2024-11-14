import { useEffect, useState } from 'react';

interface UseSSEOptions {
    onGameStarted?: () => void;
    onGameEnded?: () => void;
    onTurnEnded?: () => void;
    onPlayerLeftLobby?: (leftPlayerId: string) => void;
    onGameSettingsUpdated?: () => void;
    onPlayerJoinedLobby?: () => void;
    refreshGameDetails?: () => Promise<void>;
}

const useSSE = (
    gameId: string | undefined,
    options: UseSSEOptions = {}
) => {
    const {
        onGameStarted,
        onGameEnded,
        onTurnEnded,
        onPlayerLeftLobby,
        onGameSettingsUpdated,
        onPlayerJoinedLobby,
        refreshGameDetails,
    } = options;
    const [isConnected, setIsConnected] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (!gameId) return;

        let isCancelled = false;
        const eventSource = new EventSource(`/api/GameEvents/${gameId}`);

        eventSource.onopen = () => {
            if (isCancelled) return;
            setIsConnected(true);
            setError(null);
            console.log('SSE connection established');
        };

        eventSource.onmessage = async (event) => {
            if (isCancelled) return;
            try {
                const data = JSON.parse(event.data);
                console.log('SSE message - event:', event);
                console.log('SSE message - event.data:', data);
                switch (data.event) {
                    case 'TurnEnded':
                        onTurnEnded && onTurnEnded();
                        break;
                    case 'GameEnded':
                        onGameEnded && onGameEnded();
                        break;
                    case 'PlayerLeftLobby':
                        onPlayerLeftLobby && onPlayerLeftLobby(data.data);
                        break;
                    case 'GameStarted':
                        onGameStarted && onGameStarted();
                        break;
                    case 'GameSettingsUpdated':
                        onGameSettingsUpdated && onGameSettingsUpdated();
                        break;
                    case 'UserJoinedLobby':
                        onPlayerJoinedLobby && onPlayerJoinedLobby();
                        break;
                    default:
                        refreshGameDetails && (await refreshGameDetails());
                        break;
                }
            } catch (err) {
                console.error('Error parsing SSE message:', err);
                setError('Error parsing server message');
            }
        };

        eventSource.onerror = (event) => {
            if (isCancelled) return;
            console.error('SSE Error:', event);
            setIsConnected(false);
            setError('Server connection lost');
            eventSource.close();
        };

        return () => {
            isCancelled = true;
            eventSource.close();
        };
    }, [
        gameId,
        onGameStarted,
        onGameEnded,
        onTurnEnded,
        onPlayerLeftLobby,
        onGameSettingsUpdated,
        onPlayerJoinedLobby,
        refreshGameDetails,
    ]);

    return { isConnected, error };
};

export default useSSE;