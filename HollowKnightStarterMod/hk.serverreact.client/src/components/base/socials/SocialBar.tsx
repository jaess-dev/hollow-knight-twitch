import './SocialBar.css';
import { SocialIcons } from './SocialIcons';
import { SOCIALS, type Social } from './socials';

interface SocialItemProps extends Social { }

export const SocialBar: React.FC = () => {
    return (
        <div className="social-bar">
            {SOCIALS.map((social, index) => (
                <SocialItem key={index} {...social} />
            ))}
        </div>
    );
};

const SocialItem: React.FC<SocialItemProps> = ({ platform, icon, username, url, className }) => (
    <div className="social-item" onClick={() => window.open(url, '_blank')}>
        {/* <div className={`social-icon ${className}`}>{icon}</div> */}
        <SocialIcons icon={icon} width={100} height={100}/>
        <div className="social-handle">
            <div className="social-label">{platform}</div>
            <div className="social-username">{username}</div>
        </div>
    </div>
);