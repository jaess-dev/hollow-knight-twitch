export interface Social {
  platform: string;
  icon: string;
  username: string;
  url: string;
  className: string;
}

export const SOCIALS: Social[] = [
  {
    platform: "Instagram",
    icon: "insta",
    username: "@JaessDev",
    url: "https://instagram.com/JaessDev",
    className: "instagram",
  },
  {
    platform: "YouTube",
    icon: "yt",
    username: "@JaessDev-Gaming",
    url: "https://youtube.com/@JaessDev",
    className: "youtube",
  },
  {
    platform: "TikTok",
    icon: "tiktok",
    username: "@JaessDev",
    url: "https://tiktok.com/@JaessDev",
    className: "tiktok",
  },
];