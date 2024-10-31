import React from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from '@/components/ui/dialog';
import {Input} from "@/components/ui/input";
import { Button } from '@/components/ui/button';

interface JoinGameDialogProps {
    open: boolean;
    joinGameId: string;
    onOpenChange: (open: boolean) => void;
    onJoinGameIdChange: (value: string) => void;
    onJoinGame: () => void;
    isJoining: boolean;
}

const JoinGameDialog: React.FC<JoinGameDialogProps> = ({
                                                           open,
                                                           joinGameId,
                                                           onOpenChange,
                                                           onJoinGameIdChange,
                                                           onJoinGame,
                                                           isJoining
                                                       }) => {
    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent>
                <DialogHeader>
                    <DialogTitle>Join Game</DialogTitle>
                </DialogHeader>
                <Input
                    placeholder="Enter game ID"
                    value={joinGameId}
                    onChange={(e) => onJoinGameIdChange(e.target.value)}
                />
                <DialogFooter>
                    <Button variant="outline" onClick={() => onOpenChange(false)}>Cancel</Button>
                    <Button
                        onClick={onJoinGame}
                        disabled={isJoining}
                    >
                        {isJoining ? 'Joining...' : 'Join'}
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    );
};

export default JoinGameDialog;
