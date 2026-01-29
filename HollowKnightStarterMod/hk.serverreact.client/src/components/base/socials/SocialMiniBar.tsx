import { SocialIcons } from "./SocialIcons";
import "./SocialMiniBar.css";
import { SOCIALS } from "./socials";



export const SocialMiniBar: React.FC = () => {
    return (
        <div className="social-mini-overlay">
            {SOCIALS.map((social, index) => (
                <div
                    key={index}
                    className="social-mini-item"
                    onClick={() => window.open(social.url, "_blank")}
                >
                    <span className={`social-mini-icon ${social.className}`}>
                        <SocialIcons icon={social.icon} height={20} />
                    </span>
                    <span className="social-mini-username">{social.username}</span>
                </div>
            ))}
        </div>
    );
};
