from typing import Optional

from .models import Subscription


def is_youtube_feed(rss_url: str) -> bool:
    """判斷是否為 YouTube RSS Feed。"""
    return "youtube.com/feeds/videos.xml" in rss_url


def format_discord_message(content: str, mention_user_id: Optional[int] = None) -> str:
    """格式化 Discord 訊息。"""
    if mention_user_id:
        return f"<@{mention_user_id}> {content}"
    return content


def format_feed_message(sub: Subscription, entry, feed_info) -> str:
    """將 RSS 條目格式化為跨平台文字訊息。"""
    is_yt = is_youtube_feed(sub.rss_url)
    channel_name = (
        sub.display_name
        or getattr(feed_info, "title", None)
        or feed_info.get("title", "YouTube 頻道" if is_yt else "RSS Feed")
    )
    title = getattr(entry, "title", "(no title)")
    link = getattr(entry, "link", "")
    icon = "🎬" if is_yt else "📰"
    return f"● {channel_name} :\n{icon} {title}\n🔗 {link}".strip()
