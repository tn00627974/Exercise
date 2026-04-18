from dataclasses import dataclass, field
from typing import Any, Dict, Optional


@dataclass
class Subscription:
    """單一 RSS 對單一通知目標的訂閱設定。"""

    rss_url: str
    platform: str = "discord"
    channel_id: Optional[int] = None
    mention_user_id: Optional[int] = None
    webhook_url: Optional[str] = None
    target_id: Optional[str] = None
    display_name: Optional[str] = None
    extra: Dict[str, Any] = field(default_factory=dict)
