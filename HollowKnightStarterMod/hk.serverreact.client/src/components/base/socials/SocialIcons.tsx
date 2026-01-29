import './SocialIcons.css';

import tiktok from "@/assets/icons/socials/tiktok.svg?url";
import insta from "@/assets/icons/socials/instagram.svg?url";
import yt from "@/assets/icons/socials/youtube.svg?url";


export const SocialIcons: React.FC<{ icon: string, height?: number, width?: number }> = ({ icon, height, width }) => {
    let src = "";
    switch (icon) {
        case "tiktok":
            src = tiktok;
            break;
        case "insta":
            src = insta;
            break;
        case "yt":
            src = yt;
            break;
    }

    return <img height={height} width={width} src={src} className='neon-icon' />
}