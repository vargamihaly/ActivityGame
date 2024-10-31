import React from 'react';
import { Card, CardHeader, CardFooter, CardTitle, CardDescription } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { PlusCircle, Users } from 'lucide-react';

interface GameActionCardProps {
    title: string;
    description: string;
    actionText: string;
    onAction: () => void;
    isLoading: boolean;
    disabled?: boolean;
    variant?: 'create' | 'join';
}

const GameActionCard: React.FC<GameActionCardProps> = ({ disabled = false, ...props }) => {
    return (
        <Card>
            <CardHeader>
                <CardTitle>{props.title}</CardTitle>
                <CardDescription>{props.description}</CardDescription>
            </CardHeader>
            <CardFooter>
                <Button
                    className="w-full"
                    onClick={props.onAction}
                    disabled={props.isLoading || disabled}
                >
                    {props.variant === 'create' ? (
                        <PlusCircle className="mr-2 h-4 w-4" />
                    ) : (
                        <Users className="mr-2 h-4 w-4" />
                    )}
                    {props.isLoading ? `${props.actionText}...` : props.actionText}
                </Button>
            </CardFooter>
        </Card>
    );
};

export default GameActionCard;
