// src/components/Statistics/Statistics.tsx
import React from 'react';
import { useGlobalStatistics, useUserStatistics } from "@/hooks/gameHooks";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Trophy, User, Users } from 'lucide-react';
import { Loader2 } from 'lucide-react';

export const Statistics: React.FC = () => {
    const {
        data: globalStats,
        isLoading: isGlobalLoading
    } = useGlobalStatistics();
    const {
        data: userStats,
        isLoading: isUserLoading
    } = useUserStatistics();

    if (isGlobalLoading || isUserLoading) {
        return (
            <div className="flex justify-center items-center p-4">
                <Loader2 className="h-8 w-8 animate-spin" />
            </div>
        );
    }

    return (
        <div className="grid gap-4 md:grid-cols-2">
            {userStats && (
                <Card>
                    <CardHeader>
                        <CardTitle className="flex items-center gap-2">
                            <User className="h-5 w-5" />
                            Your Statistics
                        </CardTitle>
                    </CardHeader>
                    <CardContent>
                        <div className="space-y-2">
                            <p>Games Played: {userStats.gamesPlayed}</p>
                            <p>Games Won: {userStats.gamesWon}</p>
                            <p>Win Rate: {userStats.winRate}%</p>
                            <p>Average Score: {userStats.averageScore}</p>
                        </div>
                    </CardContent>
                </Card>
            )}

            {globalStats && (
                <Card>
                    <CardHeader>
                        <CardTitle className="flex items-center gap-2">
                            <Users className="h-5 w-5" />
                            Global Statistics
                        </CardTitle>
                    </CardHeader>
                    <CardContent>
                        <div className="space-y-4">
                            <div>
                                <p>Average Score: {globalStats.averageScore}</p>
                                <p>Win Rate: {globalStats.winRate}%</p>
                            </div>

                            <div>
                                <h4 className="font-semibold flex items-center gap-2 mb-2">
                                    <Trophy className="h-4 w-4 text-yellow-500" />
                                    Top Players
                                </h4>
                                <div className="space-y-2">
                                    {globalStats.playerRankings!.map((player, index) => (
                                        <div key={player.username} className="flex justify-between items-center">
                                            <span>{index + 1}. {player.username}</span>
                                            <span className="font-medium">{player.totalScore}</span>
                                        </div>
                                    ))}
                                </div>
                            </div>
                        </div>
                    </CardContent>
                </Card>
            )}
        </div>
    );
};