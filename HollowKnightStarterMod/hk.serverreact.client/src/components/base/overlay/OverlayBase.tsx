import React, { useState, useEffect, useRef, type ReactNode } from 'react';
import "./OverlayBase.css";
import type { ChatPanelProps, Message } from '../chat/types';
import { useTwitchMessageEvent, type ChatMessage, type Emote } from '../../../lib/signalr/useTwitchHub';


interface ChatOverlayProps {
    backgroundImage?: string; // Optional prop to set background image
    chat: (props: ChatPanelProps) => ReactNode;
    bar: () => ReactNode;
}

const OverlayBase: React.FC<ChatOverlayProps> = ({ backgroundImage, chat, bar }) => {
    const [messages, setMessages] = useState<{ id: string; message: Message }[]>([]);
    const [viewerCount, setViewerCount] = useState<number>(1337);
    const chatMessagesRef = useRef<HTMLDivElement>(null);

    const addMessage = (id: string, message: ChatMessage): void => {
        setMessages(prevMessages => {
            if (prevMessages.filter(m => m.id == id).length > 0) {
                return prevMessages;
            }

            const maxMessages = 20;
            const newMessages = [...prevMessages, {
                id, message: {
                    username: message.displayName,
                    text: message.message,
                    textBlocks: renderChatMessage(message.message, message.emoteSet.emotes),
                    badge: "", //m.badges,
                    timestamp: message.tmiSent,
                }
            }];
            if (newMessages.length > maxMessages) {
                return newMessages.slice(1);
            }
            return newMessages;
        });
    };

    useTwitchMessageEvent((event) => {
        const m = event.chatMessage;
        addMessage(m.id, m);
    });

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

                    {chat({ messages: messages.map(m => m.message), viewerCount, chatMessagesRef })}
                </div>
                {bar()}
            </div>
        </>
    );
};

/* {
/     "emotes": [
/         {
/             "id": "425618",
/             "name": "LUL",
/             "startIndex": 0,
/             "endIndex": 2,
/             "imageUrl": "https://static-cdn.jtvnw.net/emoticons/v1/425618/1.0"
/         },
/         {
/             "id": "555555560",
/             "name": ":D",
/             "startIndex": 4,
/             "endIndex": 5,
/             "imageUrl": "https://static-cdn.jtvnw.net/emoticons/v1/555555560/1.0"
/         }
/     ],
/     "rawEmoteSetString": "425618:0-2/555555560:4-5"
/ }
*/
export function renderChatMessage(
    text: string,
    emotes: Emote[]
): ReactNode[] {
    if (emotes.length === 0) {
        return [text];
    }

    const sorted = [...emotes].sort(
        (a, b) => a.startIndex - b.startIndex
    );

    const nodes: ReactNode[] = [];
    let cursor = 0;

    sorted.forEach((emote, index) => {
        // text before emote
        if (cursor < emote.startIndex) {
            nodes.push(
                text.slice(cursor, emote.startIndex)
            );
        }

        // emote image
        nodes.push(
            <img
                key={`emote-${emote.id}-${index}`}
                src={emote.imageUrl}
                alt={emote.name}
                title={emote.name}
                style={{ verticalAlign: "middle" }}
            />
        );

        cursor = emote.endIndex + 1;
    });

    // remaining text
    if (cursor < text.length) {
        nodes.push(text.slice(cursor));
    }

    return nodes;
}


export default OverlayBase;