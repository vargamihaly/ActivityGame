import React from 'react';
import { Card, CardContent } from "@/components/ui/card";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import {components} from "@/api/activitygame-schema";

type MethodType = components['schemas']['MethodType'];

interface CurrentRoundProps {
    methodType: MethodType;
    word: string;
    activePlayerUsername: string;
    isActivePlayer: boolean;
}

const CurrentRound: React.FC<CurrentRoundProps> = ({ methodType, word, activePlayerUsername, isActivePlayer }) => (
    <Card className="md:col-span-2">
        <CardContent className="p-6">
            <div className="mb-6">
                <h2 className="text-2xl font-semibold mb-3">Current Round</h2>
                <div className="bg-primary/10 p-4 rounded-lg">
                    <p className="text-lg mb-2">
                        <strong>Method:</strong> {methodType}
                    </p>
                    <p className="text-3xl font-bold text-primary">{word}</p>
                </div>
            </div>

            <div className="mb-6">
                <h3 className="text-xl font-semibold mb-2">Active Player</h3>
                <div className="flex items-center bg-secondary/20 p-3 rounded-lg">
                    <Avatar className="h-10 w-10 mr-3">
                        <AvatarFallback>{activePlayerUsername[0]}</AvatarFallback>
                    </Avatar>
                    <span className="text-lg font-medium">{activePlayerUsername}</span>
                    {isActivePlayer && (
                        <Badge
                            variant="default"
                            className="ml-auto bg-primary text-primary-foreground"
                        >
                            It's your turn!
                        </Badge>
                    )}
                </div>
            </div>
        </CardContent>
    </Card>
);

export default CurrentRound;