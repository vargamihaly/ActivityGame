import React from 'react';
import { Button } from "@/components/ui/button";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import {components} from "@/api/activitygame-schema";

type PlayerResponse = components['schemas']['PlayerResponse'];

interface WinnerSelectionProps {
    players: PlayerResponse[];
    activePlayerUsername: string;
    selectedWinnerId: string;
    onWinnerSelect: (winnerId: string) => void;
    onEndTurn: () => void;
    isEndingTurn: boolean;
}

const WinnerSelection: React.FC<WinnerSelectionProps> = ({
                                                             players,
                                                             activePlayerUsername,
                                                             selectedWinnerId,
                                                             onWinnerSelect,
                                                             onEndTurn,
                                                             isEndingTurn
                                                         }) => (
    <div className="space-y-4">
        <Select onValueChange={onWinnerSelect}>
            <SelectTrigger className="w-full">
                <SelectValue placeholder="Select Winner"/>
            </SelectTrigger>
            <SelectContent>
                {players
                    .filter(player => player.username !== activePlayerUsername)
                    .map((player) => (
                        <SelectItem key={player.id} value={player.id!}>{player.username}</SelectItem>
                    ))}
            </SelectContent>
        </Select>

        <Button
            onClick={onEndTurn}
            disabled={!selectedWinnerId || isEndingTurn}
            className="w-full"
        >
            {isEndingTurn ? 'Ending Turn...' : 'End Turn'}
        </Button>
    </div>
);

export default WinnerSelection;