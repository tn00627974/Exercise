from typing import Dict

import aiohttp
import discord

from .config import get_env_var, normalize_platform
from .formatters import format_discord_message
from .models import Subscription


class BaseNotifier:
    async def send(self, sub: Subscription, content: str) -> None:
        raise NotImplementedError


class DiscordNotifier(BaseNotifier):
    """負責 Discord 發送。"""

    def __init__(self, client: discord.Client):
        self.client = client
        self._channel_cache: Dict[int, discord.abc.Messageable] = {}

    async def send(self, sub: Subscription, content: str) -> None:
        if not sub.channel_id:
            raise RuntimeError("discord 訂閱缺少 channel_id")

        channel = self._channel_cache.get(sub.channel_id)
        if channel is None:
            channel = await self.client.fetch_channel(sub.channel_id)
            self._channel_cache[sub.channel_id] = channel

        message = format_discord_message(content, sub.mention_user_id)
        await channel.send(message)


class LineNotifier(BaseNotifier):
    """負責 LINE 發送。"""

    def __init__(self, session: aiohttp.ClientSession):
        self.session = session

    async def send(self, sub: Subscription, content: str) -> None:
        if not sub.target_id:
            raise RuntimeError("line 訂閱缺少 target_id")

        headers = {
            "Authorization": f"Bearer {get_env_var('LINE_CHANNEL_ACCESS_TOKEN')}",
            "Content-Type": "application/json",
        }
        payload = {
            "to": sub.target_id,
            "messages": [{"type": "text", "text": content[:5000]}],
        }
        async with self.session.post(
            "https://api.line.me/v2/bot/message/push",
            headers=headers,
            json=payload,
        ) as response:
            body = await response.text()
            if response.status >= 400:
                raise RuntimeError(f"LINE 推送失敗: {response.status} {body}")


class FeishuNotifier(BaseNotifier):
    """負責飛書發送。"""

    def __init__(self, session: aiohttp.ClientSession):
        self.session = session

    async def send(self, sub: Subscription, content: str) -> None:
        webhook_url = sub.webhook_url or get_env_var("FEISHU_WEBHOOK_URL")
        payload = {"msg_type": "text", "content": {"text": content[:4000]}}

        async with self.session.post(webhook_url, json=payload) as response:
            body = await response.text()
            if response.status >= 400:
                raise RuntimeError(f"飛書推送失敗: {response.status} {body}")


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
