"""各平台批量推送通知器。

職責：透過 BatchQueueManager 累積消息，達到條件後合併成單次 API 呼叫。

擴充新平台：
    1. 繼承 BaseBatchNotifier
    2. 設定 _MAX_CONTENT_LENGTH（平台字元上限）
    3. 實作 _target_key(sub) → 回傳隊列識別 key
    4. 實作 _push(target_id, content) → 發送至目標平台 API
"""

import logging
from abc import ABC, abstractmethod
from typing import Dict, List, Optional

import aiohttp
import discord

from rss_center.batch_queue import BatchQueueManager
from rss_center.config import get_env_var
from rss_center.formatters import format_discord_message
from rss_center.models import Subscription
from rss_center.notifiers import BaseNotifier


class BaseBatchNotifier(BaseNotifier, ABC):
    """批量推送基底類別，封裝隊列管理、訊息合併、生命週期。

    子類只需實作：
    - _target_key(sub)：驗證 sub 並回傳隊列 key
    - _push(target_id, content)：平台專屬的 API 呼叫
    """

    _MAX_CONTENT_LENGTH: int = 5000  # 子類依平台限制覆寫

    def __init__(self, max_batch_size: int, max_wait_seconds: int) -> None:
        self._queue_manager = BatchQueueManager(
            flush_callback=self._do_flush,
            max_batch_size=max_batch_size,
            max_wait_seconds=max_wait_seconds,
        )

    async def send(self, sub: Subscription, content: str) -> None:
        """將訊息加入批量隊列（由隊列自動決定推送時機）。"""
        key = self._target_key(sub)  # 驗證並取得 key
        await self._queue_manager.enqueue(key, content)

    @abstractmethod
    def _target_key(self, sub: Subscription) -> str:
        """驗證 sub 並回傳此訂閱對應的隊列 key。"""
        ...

    @abstractmethod
    async def _push(self, target_id: str, content: str) -> None:
        """將合併後的訊息發送至目標平台（單次 API 呼叫）。"""
        ...

    async def _do_flush(self, target_id: str) -> None:
        """隊列觸發回調：drain → merge → push。"""
        messages = self._queue_manager.drain(target_id)
        if not messages:
            return
        merged = self._merge(messages)
        await self._push(target_id, merged)
        logging.info(
            "[%s] 推送 %d 條至 %s（合併為 1 次 API 呼叫）",
            self.__class__.__name__,
            len(messages),
            target_id,
        )

    def _merge(self, messages: List[str]) -> str:
        """合併多條訊息；單條直接回傳，多條加序號標頭。"""
        if len(messages) == 1:
            return messages[0]

        parts = [
            f"【{i + 1}/{len(messages)}】\n{msg}" for i, msg in enumerate(messages)
        ]
        merged = "\n\n".join(parts)

        limit = self._MAX_CONTENT_LENGTH
        if len(merged) > limit:
            logging.warning(
                "[%s] 合併訊息超出 %d 字限制，截斷",
                self.__class__.__name__,
                limit,
            )
            merged = merged[: limit - 10] + "\n⋯（已截斷）"

        return merged

    async def shutdown(self) -> None:
        """Bot 關閉前強制刷新所有待推送隊列。"""
        pending = self._queue_manager.stats()
        if any(v > 0 for v in pending.values()):
            logging.info(
                "[%s] shutdown，刷新剩餘隊列: %s",
                self.__class__.__name__,
                pending,
            )
        await self._queue_manager.shutdown()


class LineBatchNotifier(BaseBatchNotifier):
    """LINE 批量推送通知器。

    累積達 max_batch_size 條或超過 max_wait_seconds，合併為 1 次 API 呼叫，
    節省 LINE 月額度（上限 200 封）。
    """

    _MAX_CONTENT_LENGTH = 5000

    def __init__(
        self,
        session: aiohttp.ClientSession,
        max_batch_size: int = 10,  # 10 條合併為 1 次 API 呼叫
        max_wait_seconds: int = 3600,  # 1 小時
    ) -> None:
        super().__init__(max_batch_size, max_wait_seconds)
        self._session = session

    def _target_key(self, sub: Subscription) -> str:
        if not sub.target_id:
            raise RuntimeError("line 訂閱缺少 target_id")
        return sub.target_id

    async def _push(self, target_id: str, content: str) -> None:
        token = get_env_var("LINE_CHANNEL_ACCESS_TOKEN")
        headers = {
            "Authorization": f"Bearer {token}",
            "Content-Type": "application/json",
        }
        payload = {
            "to": target_id,
            "messages": [{"type": "text", "text": content[: self._MAX_CONTENT_LENGTH]}],
        }
        async with self._session.post(
            "https://api.line.me/v2/bot/message/push",
            headers=headers,
            json=payload,
        ) as resp:
            body = await resp.text()
            if resp.status == 429:
                # 可能是月額度超限，也可能是短期速率限制
                body_lower = body.lower()
                if "quota" in body_lower or "monthly" in body_lower:
                    logging.warning("LINE 超過每月訊息量: %s", body)
                    raise RuntimeError("LINE 超過每月訊息量，請檢查帳戶狀態")
                else:
                    logging.warning("LINE 速率限制: %s", body)
                    raise RuntimeError("LINE 速率限制，請稍後再試")

            elif resp.status >= 400:
                raise RuntimeError(f"LINE 推送失敗: {resp.status} {body}")

            elif resp.status >= 500:
                logging.error("LINE 伺服器錯誤: %s", body)
                raise RuntimeError("LINE 伺服器錯誤，請稍後再試")


class DiscordBatchNotifier(BaseBatchNotifier):
    """Discord 批量推送通知器。
    預設 max_batch_size=1（等同即時推送），未來可調高以節省 API 呼叫。
    隊列 key 為 str(channel_id)，同頻道的訊息才會合併。
    """

    _MAX_CONTENT_LENGTH = 5000  # Discord 單則訊息上限

    def __init__(
        self,
        client: discord.Client,
        max_batch_size: int = 5,  # 5 條合併為 1 次 API 呼叫
        max_wait_seconds: int = 900,  # 15 分鐘
    ) -> None:
        super().__init__(max_batch_size, max_wait_seconds)
        self.client = client
        self._channel_cache: Dict[int, discord.abc.Messageable] = {}
        # 記錄每個 channel_id 對應的 mention_user_id
        self._mention_map: Dict[str, Optional[int]] = {}

    def _target_key(self, sub: Subscription) -> str:
        if not sub.channel_id:
            raise RuntimeError("discord 訂閱缺少 channel_id")
        key = str(sub.channel_id)
        # 首次見到此 channel 時記錄 mention 設定
        self._mention_map.setdefault(key, sub.mention_user_id)
        return key

    async def _push(self, target_id: str, content: str) -> None:
        channel_id = int(target_id)
        channel = self._channel_cache.get(channel_id)
        if channel is None:
            channel = await self.client.fetch_channel(channel_id)
            self._channel_cache[channel_id] = channel

        mention_user_id = self._mention_map.get(target_id)
        message = format_discord_message(
            content[: self._MAX_CONTENT_LENGTH], mention_user_id
        )
        await channel.send(message)


# TODO : 未來可啟用飛書 FeishuBatchNotifier
class FeishuBatchNotifier(BaseBatchNotifier):
    """負責飛書發送。
    預設 max_batch_size=1（等同即時推送），未來可調高以節省 API 呼叫。
    隊列 key 為 str(channel_id)，同頻道的訊息才會合併。
    """

    _MAX_CONTENT_LENGTH = 5000

    def __init__(
        self,
        session: aiohttp.ClientSession,
        max_batch_size: int = 1,  # 即時推送
        max_wait_seconds: int = 1,  # 即時推送
    ) -> None:
        super().__init__(max_batch_size, max_wait_seconds)
        self._session = session

    def _target_key(self, sub: Subscription) -> str:
        webhook_url = sub.webhook_url or get_env_var("FEISHU_WEBHOOK_URL")
        if not webhook_url:
            raise RuntimeError("飛書訂閱缺少 webhook_url")
        return webhook_url

    async def _push(self, target_id: str, content: str) -> None:
        payload = {
            "msg_type": "text",
            "content": {"text": content[: self._MAX_CONTENT_LENGTH]},
        }
        async with self._session.post(target_id, json=payload) as response:
            body = await response.text()
            if response.status >= 400:
                raise RuntimeError(f"飛書推送失敗: {response.status} {body}")
