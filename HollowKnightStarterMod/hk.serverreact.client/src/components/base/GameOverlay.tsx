import React, { useState, useEffect, useRef } from 'react';
import "./ChatOverlay.css";
import { SocialBar } from './socials/Socialbar';
import { ChatFeedOverlay } from './chat/ChatFeedOverlay';
import type { Message } from './chat/types';

interface ChatOverlayProps {
    backgroundImage?: string; // Optional prop to set background image
}

const GameOverlay: React.FC<ChatOverlayProps> = ({ backgroundImage }) => {
    const [messages, setMessages] = useState<Message[]>([]);
    const [viewerCount, setViewerCount] = useState<number>(1337);
    const chatMessagesRef = useRef<HTMLDivElement>(null);

    const demoMessages: Message[] = [
        { username: 'CyberNinja', text: 'This overlay is sick! 🔥', badge: 'MOD', timestamp: '12:34' },
        { username: 'PixelWarrior', text: 'GG WP! That was an insane play!', badge: 'SUB', timestamp: '12:35' },
        { username: 'NeonDreamer', text: 'Love the neon aesthetic', badge: 'VIP', timestamp: '12:36' },
        { username: 'CodeMaster', text: 'How did you even do that?!', badge: '', timestamp: '12:37' },
        { username: 'GlitchHunter', text: 'First time here, loving it already', badge: '', timestamp: '12:38' },
        { username: 'StreamKing', text: 'Followed on all socials! 💜', badge: 'SUB', timestamp: '12:39' }
    ];

    const randomUsernames: string[] = [
        'TechVoyager',
        'DataStream',
        'QuantumLeap',
        'CircuitBreaker',
        'ByteBender',
        'PixelPhantom',
        'NeonNinja'
    ];

    const randomMessages: string[] = [
        'This is amazing!',
        'Keep it up!',
        'Epic stream today',
        'Can you show that again?',
        'Loving the energy!',
        'PogChamp',
        'LFG!',
        '10/10 content',
        'Just followed!',
        'New sub hype! 🎉',
        'This game looks sick',
        'What keyboard are you using?'
    ];

    const addMessage = (message: Message): void => {
        setMessages(prevMessages => {
            const maxMessages = 20;
            const newMessages = [...prevMessages, message];
            if (newMessages.length > maxMessages) {
                return newMessages.slice(1);
            }
            return newMessages;
        });
    };

    useEffect(() => {
        // Load demo messages on start
        let messageIndex = 0;
        const loadDemoMessage = (): void => {
            if (messageIndex < demoMessages.length) {
                addMessage(demoMessages[messageIndex]);
                messageIndex++;
                setTimeout(loadDemoMessage, 800);
            }
        };

        const timer = setTimeout(loadDemoMessage, 500);

        return () => clearTimeout(timer);
    }, []);

    useEffect(() => {
        // Simulate random messages
        const interval = setInterval(() => {
            const randomUser = randomUsernames[Math.floor(Math.random() * randomUsernames.length)];
            const randomMsg = randomMessages[Math.floor(Math.random() * randomMessages.length)];
            const badges: string[] = ['', '', '', 'SUB', 'VIP', 'MOD'];
            const randomBadge = badges[Math.floor(Math.random() * badges.length)];

            const now = new Date();
            const timestamp = `${now.getHours().toString().padStart(2, '0')}:${now.getMinutes().toString().padStart(2, '0')}`;

            addMessage({
                username: randomUser,
                text: randomMsg,
                badge: randomBadge,
                timestamp: timestamp
            });
        }, Math.random() * 7000 + 8000);

        return () => clearInterval(interval);
    }, []);

    useEffect(() => {
        // Update viewer count randomly
        const interval = setInterval(() => {
            const viewers = Math.floor(Math.random() * 200) + 1300;
            setViewerCount(viewers);
        }, 15000);

        return () => clearInterval(interval);
    }, []);

    useEffect(() => {
        // Auto-scroll to bottom when new messages arrive
        if (chatMessagesRef.current) {
            chatMessagesRef.current.scrollTop = chatMessagesRef.current.scrollHeight;
        }
    }, [messages]);

    return (
        <>
            <link rel="preconnect" href="https://fonts.googleapis.com" />
            <link rel="preconnect" href="https://fonts.gstatic.com" crossOrigin="anonymous" />
            <link href="https://fonts.googleapis.com/css2?family=Orbitron:wght@400;700;900&family=Share+Tech+Mono&display=swap" rel="stylesheet" />

            <div className="overlay-container background" style={{ backgroundImage: backgroundImage }}>
                <div className="main-content">
                    <div className='hole' />
                    <ChatFeedOverlay messages={messages} chatMessagesRef={chatMessagesRef} />
                </div>
                <SocialBar />
            </div>
        </>
    );
};



export default GameOverlay;