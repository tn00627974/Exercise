"""台股 RSS Discord Bot

功能：
- 定期抓取 Yahoo 台股 RSS Feed
- 將新文章推播到指定的 Discord 頻道
- 記錄已推播的文章，避免重複推播
- 支援 @ 提及特定用戶
- 支援測試模式，發送單條測試訊息
"""

import asyncio
import argparse
import logging
import os
import json
from dataclasses import dataclass
from typing import Set, List, Optional, Dict

import discord
import feedparser
from dotenv import load_dotenv

# RSS 輪詢間隔時間（秒）- 預設為 300 秒（5 分鐘）
POLL_INTERVAL_SECONDS = 300

# 設定日誌格式和等級
logging.basicConfig(
    level=logging.INFO, format="[%(asctime)s] %(levelname)s: %(message)s"
)


def get_env_var(name: str) -> str:
    """取得必要的環境變數

    Args:
        name: 環境變數名稱

    Returns:
        環境變數的值（去除前後空白）

    Raises:
        RuntimeError: 當環境變數不存在或為空時
    """
    value = os.getenv(name)
    if value is not None:
        value = value.strip()
    if not value:
        raise RuntimeError(f"Missing environment variable: {name}")
    return value


@dataclass
class Subscription:
    channel_id: int
    rss_url: str
    mention_user_id: Optional[int] = None


class RssDiscordBot(discord.Client):
    """RSS Discord Bot 主類別

    繼承自 discord.Client，負責定期抓取 RSS Feed 並推播到 Discord 頻道。
    """

    def __init__(
        self,
        *,
        subscriptions: List[Subscription],
        test_message: str | None = None,
        **options,
    ):
        """初始化 RSS Discord Bot

        Args:
            channel_id: Discord 頻道 ID，用於發送訊息
            rss_url: RSS Feed 的 URL
            mention_user_id: 可選的用戶 ID，發送訊息時會 @ 該用戶
            test_message: 測試訊息內容，若提供則只發送一次測試訊息後退出
            **options: 傳遞給 discord.Client 的其他選項
        """
        super().__init__(**options)
        self.subscriptions = subscriptions
        # map rss_url -> set of seen ids for that feed
        self.seen_ids_map: Dict[str, Set[str]] = {}
        self.test_message = test_message
        self._tasks: List[asyncio.Task] = []  # 輪詢任務的 asyncio.Tasks

    async def setup_hook(self) -> None:
        """Bot 啟動時的設定鉤子

        如果是測試模式則跳過，否則初始化已讀文章列表並啟動輪詢任務。
        """
        # 如果是測試模式則不在這裡啟動持續輪詢（on_ready 會發送測試訊息）
        if self.test_message:
            return

        # 預先載入每個 RSS Feed 的已讀 ID 並為每個訂閱建立輪詢任務
        await self._prime_seen_ids()
        for sub in self.subscriptions:
            task = asyncio.create_task(self._poll_loop_for_subscription(sub))
            self._tasks.append(task)

    async def on_ready(self) -> None:
        """Bot 連線就緒時的回調函數

        僅在測試模式下執行：發送測試訊息並關閉 Bot。
        """
        # 非測試模式下不執行
        if not self.test_message:
            return

        # 測試模式：對每個訂閱的頻道發送一次測試訊息
        for sub in self.subscriptions:
            try:
                channel = await self.fetch_channel(sub.channel_id)
                message = self._format_message(self.test_message, sub.mention_user_id)
                await channel.send(message)
                logging.info("Test message sent to %s", sub.channel_id)
            except discord.NotFound:
                logging.error(
                    "Channel not found (ID %s). Check CHANNEL_ID and that the bot is in the server.",
                    sub.channel_id,
                )
            except discord.Forbidden:
                logging.error(
                    "Forbidden sending to channel %s. Check View Channel/Send Messages permissions.",
                    sub.channel_id,
                )
            except discord.HTTPException as exc:
                logging.error("Failed to send test message to %s: %s", sub.channel_id, exc)
        except discord.NotFound:
            logging.error(
                "Channel not found (ID %s). Check CHANNEL_ID and that the bot is in the server.",
                self.channel_id,
            )
        except discord.Forbidden:
            logging.error(
                "Forbidden sending to channel %s. Check View Channel/Send Messages permissions.",
                self.channel_id,
            )
        except discord.HTTPException as exc:
            logging.error("Failed to send test message to %s: %s", self.channel_id, exc)
        finally:
            # 測試完成後關閉 Bot
            await self.close()

    async def _prime_seen_ids(self) -> None:
        """預先載入當前 RSS Feed 中的所有文章 ID

        這個方法在 Bot 啟動時執行一次，將現有的所有文章標記為「已讀」，
        避免啟動時推播大量舊文章。
        """
        for sub in self.subscriptions:
            feed = feedparser.parse(sub.rss_url)
            seen: Set[str] = set()
            for entry in feed.entries:
                entry_id = self._entry_id(entry)
                if entry_id:
                    seen.add(entry_id)
            self.seen_ids_map[sub.rss_url] = seen
            logging.info("Primed %s items from %s", len(seen), sub.rss_url)

    async def _poll_loop_for_subscription(self, sub: Subscription) -> None:
        """RSS 輪詢主循環

        持續運行直到 Bot 關閉，每隔 POLL_INTERVAL_SECONDS 秒檢查一次 RSS Feed，
        並推播新文章到 Discord 頻道。
        """
        # 等待 Bot 完全就緒
        await self.wait_until_ready()

        # 預先取得頻道物件，避免每次輪詢都要重新取得
        try:
            channel = await self.fetch_channel(sub.channel_id)
        except discord.NotFound:
            logging.error(
                "Channel not found (ID %s). Check CHANNEL_ID and that the bot is in the server.",
                sub.channel_id,
            )
            return
        except discord.Forbidden:
            logging.error(
                "Forbidden fetching channel %s. Check the bot has View Channel/Send Messages permissions.",
                sub.channel_id,
            )
            return
        except discord.HTTPException as exc:
            logging.error("Failed to fetch channel %s: %s", sub.channel_id, exc)
            return

        # 持續輪詢直到 Bot 關閉
        while not self.is_closed():
            try:
                await self._poll_once_for_subscription(channel, sub)
            except Exception as exc:
                logging.exception("Polling failed for %s: %s", sub.rss_url, exc)
            await asyncio.sleep(POLL_INTERVAL_SECONDS)

    async def _poll_once_for_subscription(self, channel: discord.abc.Messageable, sub: Subscription) -> None:
        """執行一次 RSS Feed 輪詢

        檢查 RSS Feed 中的新文章，並將未推播過的文章發送到 Discord 頻道。

        Args:
            channel: Discord 頻道物件，用於發送訊息
        """
        feed = feedparser.parse(sub.rss_url)
        new_entries = []

        seen = self.seen_ids_map.get(sub.rss_url, set())
        for entry in feed.entries:
            entry_id = self._entry_id(entry)
            if not entry_id or entry_id in seen:
                continue
            seen.add(entry_id)
            new_entries.append(entry)
        self.seen_ids_map[sub.rss_url] = seen

        # 如果沒有新文章則直接返回
        if not new_entries:
            return

        # 反轉列表順序，以舊到新的順序推播（RSS Feed 通常是新到舊）
        for entry in reversed(new_entries):
            # 取得文章標題和連結
            title = getattr(entry, "title", "(no title)")
            link = getattr(entry, "link", "")
            # 組合訊息內容：標題 + 連結
            content = f"{title}\n{link}".strip()
            # 格式化訊息（可能包含 @ 提及）
            message = self._format_message(content, sub.mention_user_id)
            # 發送到 Discord 頻道
            await channel.send(message)
            logging.info("Posted: %s", title)

    def _format_message(self, content: str, mention_user_id: Optional[int] = None) -> str:
        """格式化訊息內容

        如果設定了 mention_user_id，則在訊息前面加上 @ 提及該用戶。

        Args:
            content: 原始訊息內容

        Returns:
            格式化後的訊息
        """
        if mention_user_id:
            return f"<@{mention_user_id}> {content}"
        return content

    @staticmethod
    def _entry_id(entry) -> str:
        """取得 RSS Feed 文章的唯一識別碼

        優先使用 entry.id，若不存在則使用 entry.link 作為識別碼。

        Args:
            entry: feedparser 解析的 RSS Feed 條目

        Returns:
            文章的唯一識別碼，若都不存在則返回空字串
        """
        return getattr(entry, "id", None) or getattr(entry, "link", None) or ""


def main() -> None:
    """主程式進入點

    解析命令列參數、載入環境變數，並啟動 Discord Bot。

    環境變數：
        DISCORD_TOKEN: Discord Bot 的 Token（必要）
        CHANNEL_ID: Discord 頻道 ID（必要）
        RSS_URL: RSS Feed 的 URL（必要）
        MENTION_USER_ID: 要提及的用戶 ID（選用）

    命令列參數：
        --test MESSAGE: 發送測試訊息後立即退出
    """
    # 設定命令列參數解析器
    parser = argparse.ArgumentParser(description="RSS to Discord bot")
    parser.add_argument(
        "--test",
        metavar="MESSAGE",
        help="Send a one-off test message to CHANNEL_ID and exit",
    )
    args = parser.parse_args()

    # 載入 .env 檔案中的環境變數
    load_dotenv()

    # 取得必要的環境變數
    token = get_env_var("DISCORD_TOKEN")
    # 支援多訂閱：從 SUBSCRIPTIONS（JSON）解析
    subs_env = os.getenv("SUBSCRIPTIONS", "").strip()
    subscriptions: List[Subscription] = []
    if subs_env:
        try:
            parsed = json.loads(subs_env)
            for item in parsed:
                cid = int(item["channel_id"])
                url = item["rss_url"]
                mid = int(item["mention_user_id"]) if item.get("mention_user_id") not in (None, "") else None
                subscriptions.append(Subscription(channel_id=cid, rss_url=url, mention_user_id=mid))
        except Exception as exc:
            raise RuntimeError("Invalid SUBSCRIPTIONS format; must be JSON array of objects") from exc
    else:
        # 向下相容：使用單一 CHANNEL_ID / RSS_URL
        channel_id = int(get_env_var("CHANNEL_ID"))
        rss_url = get_env_var("RSS_URL")
        mention_user_id_str = os.getenv("MENTION_USER_ID", "").strip()
        mention_user_id = int(mention_user_id_str) if mention_user_id_str else None
        subscriptions.append(Subscription(channel_id=channel_id, rss_url=rss_url, mention_user_id=mention_user_id))

    # 設定 Discord Bot 的 Intents（權限）
    intents = discord.Intents.default()
    # 建立 Bot 實例
    client = RssDiscordBot(
        subscriptions=subscriptions,
        test_message=args.test,
        intents=intents,
    )
    # 啟動 Bot（阻塞式執行）
    client.run(token)


if __name__ == "__main__":
    main()
