import type { ReactNode } from "react";

export interface ChatPanelProps {
    messages: Message[];
    viewerCount: number;
    chatMessagesRef: React.RefObject<HTMLDivElement>;
}

export type Message = {
    username: string;
    text: string;
    textBlocks?: ReactNode[];
    badge: string;
    timestamp: string;
}
