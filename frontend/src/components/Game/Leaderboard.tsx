import React from 'react';
import { Card, CardContent } from "@/components/ui/card";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import { Crown } from 'lucide-react';
import {components} from "@/api/activitygame-schema";

type PlayerResponse = components['schemas']['PlayerResponse'];

interface LeaderboardProps {
    players: PlayerResponse[];
    currentUserId: string;
}

const Leaderboard: React.FC<LeaderboardProps> = ({ players, currentUserId }) => (
    <Card>
        <CardContent className="p-6">
            <h3 className="text-xl font-semibold mb-4 flex items-center">
                <Crown className="mr-2 h-5 w-5"/>
                Leaderboard
            </h3>
            <ul className="space-y-3">
                {players.sort((a, b) => (b.score ?? 0) - (a.score ?? 0)).map((player, index) => (
                    <li key={player.id} className="flex items-center justify-between bg-secondary/10 p-2 rounded">
                        <div className="flex items-center">
                            <span className="font-semibold mr-2">{index + 1}.</span>
                            <Avatar className="h-8 w-8 mr-2">
                                <AvatarFallback>{player.username![0]}</AvatarFallback>
                            </Avatar>
                            <span>{player.id === currentUserId ? `${player.username} (You)` : player.username}</span>
                        </div>
                        <Badge variant={index === 0 ? "default" : "secondary"}>{player.score}</Badge>
                    </li>
                ))}
            </ul>
        </CardContent>
    </Card>
);

export default Leaderboard;