import './ChatPanel.css';
import type { Message } from './types';

export interface ChatPanelProps {
    messages: Message[];
    viewerCount: number;
    chatMessagesRef: React.RefObject<HTMLDivElement>;
}

interface ChatMessageProps {
    message: Message;
}

export const ChatPanel: React.FC<ChatPanelProps> = ({ messages, viewerCount, chatMessagesRef }) => (
    <div className="chat-panel">
        <div className="chat-corner chat-corner-tl"></div>
        <div className="chat-corner chat-corner-br"></div>

        <div className="chat-header">
            <div className="chat-title">LIVE CHAT</div>
            <div className="chat-status">
                <span className="status-indicator"></span>
                <span>STREAM ONLINE - {viewerCount.toLocaleString()} VIEWERS</span>
            </div>
        </div>

        <div className="chat-messages" ref={chatMessagesRef}>
            {messages.map((msg, index) => (
                <ChatMessage key={index} message={msg} />
            ))}
        </div>
    </div>
);

const ChatMessage: React.FC<ChatMessageProps> = ({ message }) => {
    const now = new Date();
    const timestamp = message.timestamp || `${now.getHours().toString().padStart(2, '0')}:${now.getMinutes().toString().padStart(2, '0')}`;

    return (
        <div className="message">
            <div className="message-header">
                <span className="username">{message.username}</span>
                {message.badge && <span className="badge">{message.badge}</span>}
                <span className="timestamp">{timestamp}</span>
            </div>
            <div className="message-text">{message.text}</div>
        </div>
    );
};

