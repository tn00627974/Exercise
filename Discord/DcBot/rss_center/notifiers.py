from typing import Dict

import aiohttp
import discord

from .config import get_env_var, normalize_platform
from .formatters import format_discord_message
from .models import Subscription


class BaseNotifier:
    async def send(self, sub: Subscription, content: str) -> None:
        raise NotImplementedError


class NotificationRouter:
    """依平台將通知分派到對應 notifier。"""

    def __init__(self, notifiers: Dict[str, BaseNotifier]):
        self.notifiers = notifiers

    async def send(self, sub: Subscription, content: str) -> None:
        platform = normalize_platform(sub.platform)
        notifier = self.notifiers.get(platform)
        if notifier is None:
            raise RuntimeError(f"不支援的平台：{sub.platform}")
        await notifier.send(sub, content)
