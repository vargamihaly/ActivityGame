import React from 'react';
import { Card, CardContent } from "@/components/ui/card";
import { Progress } from "@/components/ui/progress";
import { Timer } from 'lucide-react';

interface TimerDisplayProps {
    timeLeft: number;
    progress: number;
}

const TimerDisplay: React.FC<TimerDisplayProps> = ({ timeLeft, progress }) => {
    const formatTimeLeft = (seconds: number) => {
        const minutes = Math.floor(seconds / 60);
        const remainingSeconds = seconds % 60;
        return `${minutes}:${remainingSeconds < 10 ? '0' : ''}${remainingSeconds}`;
    };

    return (
        <Card>
            <CardContent className="p-6">
                <h3 className="text-xl font-semibold mb-4 flex items-center">
                    <Timer className="mr-2 h-5 w-5"/>
                    Time Remaining
                </h3>
                <div className="text-4xl font-bold text-center mb-4">
                    {formatTimeLeft(timeLeft)}
                </div>
                <Progress value={progress} className="w-full"/>
            </CardContent>
        </Card>
    );
};

export default TimerDisplay;