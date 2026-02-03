import "./ChatOverlay.css";

import { ChatPanel } from "./chat/ChatPanel";
import type { ChatPanelProps } from "./chat/types";
import OverlayBase from "./overlay/OverlayBase";
import { SocialBar } from "./socials/Socialbar";


interface ChatOverlayProps {
    backgroundImage?: string; // Optional prop to set background image
}

const ChatOverlay: React.FC<ChatOverlayProps> = ({ backgroundImage }) => {
    function chatPanel({ messages, viewerCount, chatMessagesRef }: ChatPanelProps) {
        return <ChatPanel messages={messages} viewerCount={viewerCount} chatMessagesRef={chatMessagesRef} />
    }

    function bar() {
        return <SocialBar />
    }

    return <OverlayBase backgroundImage={backgroundImage} chat={chatPanel} bar={bar} />
};

export default ChatOverlay;