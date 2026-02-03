import "./ChatOverlay.css";

import { ChatFeedOverlay } from "./chat/ChatFeedOverlay";
import type { ChatPanelProps } from "./chat/types";
import OverlayBase from './overlay/OverlayBase';
import { SocialMiniBar } from "./socials/SocialMiniBar";

interface ChatOverlayProps {
    backgroundImage?: string; // Optional prop to set background image
}

const GameOverlay: React.FC<ChatOverlayProps> = ({ backgroundImage }) => {
    function chatPanel({ messages, viewerCount, chatMessagesRef }: ChatPanelProps) {
        return <ChatFeedOverlay messages={messages} viewerCount={viewerCount} chatMessagesRef={chatMessagesRef} />
    }

    function bar() {
        return <SocialMiniBar />
    }

    return <OverlayBase backgroundImage={backgroundImage} chat={chatPanel} bar={bar} />
};

export default GameOverlay;