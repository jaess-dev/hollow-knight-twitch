import React from "react";
import "./ChatFeedOverlay.css";
import type { Message } from "./types";

interface ChatFeedOverlayProps {
    messages: Message[];
    chatMessagesRef: React.RefObject<HTMLDivElement>;
}

export const ChatFeedOverlay: React.FC<ChatFeedOverlayProps> = ({
    messages,
    chatMessagesRef,
}) => {
    return (
        <div className="chat-feed-overlay" ref={chatMessagesRef}>
            {messages.map((msg, index) => (
                <ChatFeedMessage key={index} message={msg} />
            ))}
        </div>
    );
};

interface ChatFeedMessageProps {
    message: Message;
}

const ChatFeedMessage: React.FC<ChatFeedMessageProps> = ({ message }) => {
    const now = new Date();
    const timestamp =
        message.timestamp ||
        `${now.getHours().toString().padStart(2, "0")}:${now
            .getMinutes()
            .toString()
            .padStart(2, "0")}`;

    return (
        <div className="chat-feed-message">
            <span className="chat-feed-username">{message.username}</span>
            {message.badge && (
                <span className={`chat-feed-badge badge-${message.badge.toLowerCase()}`}>
                    {message.badge}
                </span>
            )}
            <span className="chat-feed-text">{message.text}</span>
            <span className="chat-feed-timestamp">{timestamp}</span>
        </div>
    );
};
