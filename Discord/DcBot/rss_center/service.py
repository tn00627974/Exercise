import asyncio
import logging
from typing import Dict, List, Set

import feedparser

from .config import normalize_platform
from .formatters import format_feed_message, is_youtube_feed
from .models import Subscription
from .notifiers import NotificationRouter


class RssPollingService:
    """負責 RSS 輪詢、去重與通知分流。"""

    _FEEDPARSER_AGENT = (
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) "
        "AppleWebKit/537.36 (KHTML, like Gecko) "
        "Chrome/124.0.0.0 Safari/537.36"
    )

    def __init__(
        self, subscriptions: List[Subscription], router: NotificationRouter
    ) -> None:
        self.subscriptions = subscriptions
        self.router = router
        self.subscriptions_by_feed = self._group_subscriptions_by_feed(subscriptions)
        self.seen_ids_map: Dict[str, Set[str]] = {}

    @staticmethod
    def _group_subscriptions_by_feed(
        subscriptions: List[Subscription],
    ) -> Dict[str, List[Subscription]]:
        grouped: Dict[str, List[Subscription]] = {}
        for sub in subscriptions:
            grouped.setdefault(sub.rss_url, []).append(sub)
        return grouped

    async def prime_seen_ids(self) -> None:
        """啟動時預載已讀文章，避免洗版。"""
        for rss_url in self.subscriptions_by_feed:
            feed = feedparser.parse(rss_url, agent=self._FEEDPARSER_AGENT)
            seen: Set[str] = set()
            for entry in feed.entries:
                entry_id = self._entry_id(entry)
                if entry_id:
                    seen.add(entry_id)
            self.seen_ids_map[rss_url] = seen
            logging.info("Primed %s items from %s", len(seen), rss_url)

    async def poll_loop(self, poll_interval_seconds: int) -> None:
        """持續輪詢所有 RSS 來源。"""
        while True:
            for rss_url in self.subscriptions_by_feed:
                try:
                    await self.poll_once_for_feed(rss_url)
                except Exception as exc:
                    logging.exception("Polling failed for %s: %s", rss_url, exc)
            await asyncio.sleep(poll_interval_seconds)

    async def poll_once_for_feed(self, rss_url: str) -> None:
        """單次輪詢一個 RSS 來源。"""
        feed = feedparser.parse(rss_url, agent=self._FEEDPARSER_AGENT)
        new_entries = []

        seen = self.seen_ids_map.get(rss_url, set())
        for entry in feed.entries:
            entry_id = self._entry_id(entry)
            if not entry_id or entry_id in seen:
                continue
            seen.add(entry_id)
            new_entries.append(entry)
        self.seen_ids_map[rss_url] = seen

        if not new_entries:
            return

        for entry in reversed(new_entries):
            for sub in self.subscriptions_by_feed.get(rss_url, []):
                await self._notify_subscription(sub, entry, feed.feed)

    async def send_test_message(
        self, message: str, platform: str | None = None
    ) -> None:
        """對指定平台或所有通知目標發送一次測試訊息。"""
        target_platform = normalize_platform(platform) if platform else None

        for sub in self.subscriptions:
            if target_platform and normalize_platform(sub.platform) != target_platform:
                continue
            await self.router.send(sub, message)
            logging.info("Test message sent to %s target", sub.platform)

    async def send_latest_youtube(self) -> None:
        """對所有 YouTube 訂閱發送最新一篇內容。"""
        processed_urls: Set[str] = set()
        for rss_url, subscriptions in self.subscriptions_by_feed.items():
            if not is_youtube_feed(rss_url) or rss_url in processed_urls:
                continue
            processed_urls.add(rss_url)

            feed = feedparser.parse(rss_url, agent=self._FEEDPARSER_AGENT)
            if not feed.entries:
                logging.error("無法取得影片（頻道 RSS 為空）：%s", rss_url)
                continue

            entry = feed.entries[0]
            for sub in subscriptions:
                await self._notify_subscription(sub, entry, feed.feed)

    async def _notify_subscription(self, sub: Subscription, entry, feed_info) -> None:
        content = format_feed_message(sub, entry, feed_info)
        await self.router.send(sub, content)
        title = getattr(entry, "title", "(no title)")
        logging.info("Posted to %s: %s", sub.platform, title)

    @staticmethod
    def _entry_id(entry) -> str:
        return getattr(entry, "id", None) or getattr(entry, "link", None) or ""
