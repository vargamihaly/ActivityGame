import { useState, useEffect, useCallback } from 'react';

interface UseGameTimerProps {
    initialTime: number;
    onTimeUp?: () => void;
}

const useGameTimer = ({ initialTime, onTimeUp }: UseGameTimerProps) => {
    const [timeLeft, setTimeLeft] = useState(initialTime);
    const [isActive, setIsActive] = useState(false);
    const [progress, setProgress] = useState(100);

    const startTimer = useCallback(() => setIsActive(true), []);
    const pauseTimer = useCallback(() => setIsActive(false), []);
    const resetTimer = useCallback((newTime: number = initialTime) => {
        setTimeLeft(newTime);
        setProgress(100);
        setIsActive(false);
    }, [initialTime]);

    useEffect(() => {
        let interval: NodeJS.Timeout | null = null;

        if (isActive && timeLeft > 0) {
            interval = setInterval(() => {
                setTimeLeft((prevTime) => {
                    const newTime = prevTime - 1;
                    setProgress((newTime / initialTime) * 100);
                    return newTime;
                });
            }, 1000);
        } else if (timeLeft === 0) {
            onTimeUp?.();
        }

        return () => {
            if (interval) clearInterval(interval);
        };
    }, [isActive, timeLeft, initialTime, onTimeUp]);

    return { timeLeft, progress, startTimer, pauseTimer, resetTimer };
};

export default useGameTimer;