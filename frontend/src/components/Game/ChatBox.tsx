import {useRef, useEffect, useState} from "react";
import { Card, CardContent } from "@/components/ui/card";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Send } from 'lucide-react';
import { useAuth } from '@/context/AuthContext';

interface ChatMessage {
    id: string;
    userId: string;
    content: string;
    timestamp: string;
}

interface ChatBoxProps {
    gameId: string;
    onSendMessage: (message: string) => Promise<void>;
    messages: ChatMessage[];
}

const ChatBox: React.FC<ChatBoxProps> = ({ gameId, onSendMessage, messages }) => {
    const [message, setMessage] = useState('');
    const messagesEndRef = useRef<HTMLDivElement>(null);
    const { user } = useAuth();

    const scrollToBottom = () => {
        messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
    };

    useEffect(() => {
        scrollToBottom();
    }, [messages]);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (message.trim()) {
            await onSendMessage(message);
            setMessage('');
        }
    };

    return (
        <Card className="h-[400px] flex flex-col">
            <CardContent className="p-4 flex flex-col h-full">
                <div className="flex-1 overflow-y-auto mb-4 space-y-4">
                    {messages.map((msg) => (
                        <div
                            key={msg.id}
                            className={`flex items-start gap-2 ${
                                msg.userId === user?.id ? 'flex-row-reverse' : ''
                            }`}
                        >
                            <Avatar className="h-8 w-8">
                                <AvatarFallback>
                                    {msg.userId[0]}
                                </AvatarFallback>
                            </Avatar>
                            <div
                                className={`rounded-lg px-3 py-2 max-w-[70%] ${
                                    msg.userId === user?.id
                                        ? 'bg-primary text-primary-foreground'
                                        : 'bg-secondary'
                                }`}
                            >
                                <p>{msg.content}</p>
                            </div>
                        </div>
                    ))}
                    <div ref={messagesEndRef} />
                </div>
                <form onSubmit={handleSubmit} className="flex gap-2">
                    <Input
                        value={message}
                        onChange={(e) => setMessage(e.target.value)}
                        placeholder="Type a message..."
                        className="flex-1"
                    />
                    <Button type="submit" size="icon">
                        <Send className="h-4 w-4" />
                    </Button>
                </form>
            </CardContent>
        </Card>
    );
};

export default ChatBox;