// src/context/GameContext.tsx

import React, { createContext, useContext, useState, useEffect, useCallback } from 'react';
import { components } from '@/api/activitygame-schema';
import { getCurrentGameAsync } from '@/api/games-api';
import { GAME_STATUS } from "@/interfaces/GameTypes";

type Game = components['schemas']['GameResponse'] | null;

interface GameContextType {
    currentGame: Game;
    setCurrentGame: React.Dispatch<React.SetStateAction<Game>>;
    isInGame: boolean;
    setIsInGame: React.Dispatch<React.SetStateAction<boolean>>;
    refreshGameDetails: () => Promise<void>;
    clearGameState: () => void;
}

const GameContext = createContext<GameContextType | undefined>(undefined);

export const GameProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [currentGame, setCurrentGame] = useState<Game>(null);
    const [isInGame, setIsInGame] = useState<boolean>(false);

    const refreshGameDetails = useCallback(async () => {
        try {
            const getGameResponse = await getCurrentGameAsync();
            console.log('getGameResponse:', getGameResponse);
            if (getGameResponse && getGameResponse.data) {
                if (getGameResponse.data.status === GAME_STATUS.Finished) {
                    setCurrentGame(null);
                    setIsInGame(false);
                } else {
                    setCurrentGame(getGameResponse.data);
                    setIsInGame(true);
                }
            } else {
                setCurrentGame(null);
                setIsInGame(false);
            }
        } catch (error) {
            console.error('Error refreshing game details:', error);
            setCurrentGame(null);
            setIsInGame(false);
        }
    }, [setCurrentGame, setIsInGame]);

    const clearGameState = useCallback(() => {
        setCurrentGame(null);
        setIsInGame(false);
    }, []);

    useEffect(() => {
        refreshGameDetails();
    }, [refreshGameDetails]);

    return (
        <GameContext.Provider value={{ currentGame, setCurrentGame, isInGame, setIsInGame, refreshGameDetails, clearGameState}}>
            {children}
        </GameContext.Provider>
    );
};

export const useGame = (): GameContextType => {
    const context = useContext(GameContext);
    if (context === undefined) {
        throw new Error('useGame must be used within a GameProvider');
    }
    return context;
};
