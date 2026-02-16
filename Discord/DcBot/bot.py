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
from typing import Set

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


class RssDiscordBot(discord.Client):
    """RSS Discord Bot 主類別

    繼承自 discord.Client，負責定期抓取 RSS Feed 並推播到 Discord 頻道。
    """

    def __init__(
        self,
        *,
        channel_id: int,
        rss_url: str,
        mention_user_id: int | None = None,
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
        self.channel_id = channel_id
        self.rss_url = rss_url
        self.mention_user_id = mention_user_id
        self.test_message = test_message
        self.seen_ids: Set[str] = set()  # 已經推播過的文章 ID 集合，用於避免重複推播
        self._task: asyncio.Task | None = None  # 輪詢任務的 asyncio.Task 物件

    async def setup_hook(self) -> None:
        """Bot 啟動時的設定鉤子

        如果是測試模式則跳過，否則初始化已讀文章列表並啟動輪詢任務。
        """
        # 測試模式下不啟動輪詢任務
        if self.test_message:
            return

        # 預先載入當前 RSS Feed 中的所有文章 ID，避免首次啟動時推播所有舊文章
        await self._prime_seen_ids()
        # 建立並啟動 RSS 輪詢任務
        self._task = asyncio.create_task(self._poll_loop())

    async def on_ready(self) -> None:
        """Bot 連線就緒時的回調函數

        僅在測試模式下執行：發送測試訊息並關閉 Bot。
        """
        # 非測試模式下不執行
        if not self.test_message:
            return

        try:
            # 取得指定的 Discord 頻道
            channel = await self.fetch_channel(self.channel_id)
            # 格式化測試訊息（可能包含 @ 提及）
            message = self._format_message(self.test_message)
            # 發送測試訊息到頻道
            await channel.send(message)
            logging.info("Test message sent")
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
        feed = feedparser.parse(self.rss_url)
        for entry in feed.entries:
            entry_id = self._entry_id(entry)
            if entry_id:
                self.seen_ids.add(entry_id)
        logging.info("Primed %s items from RSS", len(self.seen_ids))

    async def _poll_loop(self) -> None:
        """RSS 輪詢主循環

        持續運行直到 Bot 關閉，每隔 POLL_INTERVAL_SECONDS 秒檢查一次 RSS Feed，
        並推播新文章到 Discord 頻道。
        """
        # 等待 Bot 完全就緒
        await self.wait_until_ready()

        # 預先取得頻道物件，避免每次輪詢都要重新取得
        try:
            channel = await self.fetch_channel(self.channel_id)
        except (
            discord.NotFound
        ):  # 404 錯誤表示頻道不存在，可能是 CHANNEL_ID 錯誤或 Bot 不在該伺服器中
            logging.error(
                "Channel not found (ID %s). Check CHANNEL_ID and that the bot is in the server.",
                self.channel_id,
            )
            return
        except discord.Forbidden:  # 403 錯誤通常表示 Bot 沒有足夠的權限訪問頻道
            logging.error(
                "Forbidden fetching channel %s. Check the bot has View Channel/Send Messages permissions.",
                self.channel_id,
            )
            return
        except discord.HTTPException as exc:
            logging.error("Failed to fetch channel %s: %s", self.channel_id, exc)
            return

        # 持續輪詢直到 Bot 關閉
        while not self.is_closed():
            try:
                await self._poll_once(channel)
            except Exception as exc:
                # 記錄例外但不中斷輪詢循環
                logging.exception("Polling failed: %s", exc)
            # 等待指定時間後再進行下一次輪詢
            await asyncio.sleep(POLL_INTERVAL_SECONDS)

    async def _poll_once(self, channel: discord.abc.Messageable) -> None:
        """執行一次 RSS Feed 輪詢

        檢查 RSS Feed 中的新文章，並將未推播過的文章發送到 Discord 頻道。

        Args:
            channel: Discord 頻道物件，用於發送訊息
        """
        # 解析 RSS Feed
        feed = feedparser.parse(self.rss_url)
        new_entries = []

        # 找出所有未推播過的新文章
        for entry in feed.entries:
            entry_id = self._entry_id(entry)
            # 跳過已推播過的文章或沒有 ID 的文章
            if not entry_id or entry_id in self.seen_ids:
                continue
            # 標記為已讀並加入新文章列表
            self.seen_ids.add(entry_id)
            new_entries.append(entry)

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
            message = self._format_message(content)
            # 發送到 Discord 頻道
            await channel.send(message)
            logging.info("Posted: %s", title)

    def _format_message(self, content: str) -> str:
        """格式化訊息內容

        如果設定了 mention_user_id，則在訊息前面加上 @ 提及該用戶。

        Args:
            content: 原始訊息內容

        Returns:
            格式化後的訊息
        """
        if self.mention_user_id:
            return f"<@{self.mention_user_id}> {content}"
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
    channel_id = int(get_env_var("CHANNEL_ID"))
    rss_url = get_env_var("RSS_URL")
    # MENTION_USER_ID 是選用的，若未設定則為 None
    mention_user_id_str = os.getenv("MENTION_USER_ID", "").strip()
    mention_user_id = int(mention_user_id_str) if mention_user_id_str else None

    # 設定 Discord Bot 的 Intents（權限）
    intents = discord.Intents.default()
    # 建立 Bot 實例
    client = RssDiscordBot(
        channel_id=channel_id,
        rss_url=rss_url,
        mention_user_id=mention_user_id,
        test_message=args.test,
        intents=intents,
    )
    # 啟動 Bot（阻塞式執行）
    client.run(token)


if __name__ == "__main__":
    main()
