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
from aiohttp import web
from dotenv import load_dotenv
import time

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
        test_yt: bool = False,
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
        self.test_yt = test_yt
        self._tasks: List[asyncio.Task] = []  # 輪詢任務的 asyncio.Tasks

    async def setup_hook(self) -> None:
        """Bot 啟動時的設定鉤子

        如果是測試模式則跳過，否則初始化已讀文章列表並啟動輪詢任務。
        """
        # 如果是測試模式則不在這裡啟動持續輪詢（on_ready 會發送測試訊息）
        if self.test_message or self.test_yt:
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
        if not self.test_message and not self.test_yt:
            return

        if self.test_yt:
            # --test-yt：對每個 YouTube 訂閱抓最新一部影片以 Embed 格式推送
            yt_subs = [
                s for s in self.subscriptions if self._is_youtube_feed(s.rss_url)
            ]
            if not yt_subs:
                logging.error(
                    "subscriptions 中沒有任何 YouTube feed，請確認 rss_url 包含 youtube.com/feeds/videos.xml"
                )
            for sub in yt_subs:
                try:
                    feed = feedparser.parse(sub.rss_url, agent=self._FEEDPARSER_AGENT)
                    if not feed.entries:
                        logging.error("無法取得影片（頻道 RSS 為空）：%s", sub.rss_url)
                        continue
                    entry = feed.entries[0]  # 最新一部
                    title = getattr(entry, "title", "(no title)")
                    link = getattr(entry, "link", "")
                    content = f"{title}\n{link}".strip()
                    message = self._format_message(content, sub.mention_user_id)
                    channel = await self.fetch_channel(sub.channel_id)
                    await channel.send(message)
                    logging.info(
                        "YT test sent to %s：%s",
                        sub.channel_id,
                        title,
                    )
                except discord.NotFound:
                    logging.error("找不到頻道 ID %s", sub.channel_id)
                except discord.Forbidden:
                    logging.error("無法傳送到頻道 %s，請確認 Bot 權限", sub.channel_id)
                except discord.HTTPException as exc:
                    logging.error("傳送失敗 %s：%s", sub.channel_id, exc)
        else:
            # --test：對每個訂閱的頻道發送一次純文字測試訊息
            for sub in self.subscriptions:
                try:
                    channel = await self.fetch_channel(sub.channel_id)
                    message = self._format_message(
                        self.test_message, sub.mention_user_id
                    )
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
                    logging.error(
                        "Failed to send test message to %s: %s", sub.channel_id, exc
                    )

        # 測試完成，短暫延遲後關閉 Bot（確保所有訊息都已發送）
        await asyncio.sleep(1)
        await self.close()

    # feedparser 預設的 User-Agent 容易被 Cloudflare 封鎖，改用瀏覽器 UA
    _FEEDPARSER_AGENT = (
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) "
        "AppleWebKit/537.36 (KHTML, like Gecko) "
        "Chrome/124.0.0.0 Safari/537.36"
    )

    async def _prime_seen_ids(self) -> None:
        """預先載入當前 RSS Feed 中的所有文章 ID

        這個方法在 Bot 啟動時執行一次，將現有的所有文章標記為「已讀」，
        避免啟動時推播大量舊文章。
        """
        for sub in self.subscriptions:
            feed = feedparser.parse(sub.rss_url, agent=self._FEEDPARSER_AGENT)
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

    async def _poll_once_for_subscription(
        self, channel: discord.abc.Messageable, sub: Subscription
    ) -> None:
        """執行一次 RSS Feed 輪詢

        檢查 RSS Feed 中的新文章，並將未推播過的文章發送到 Discord 頻道。

        Args:
            channel: Discord 頻道物件，用於發送訊息
        """
        feed = feedparser.parse(sub.rss_url, agent=self._FEEDPARSER_AGENT)
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
        is_yt = self._is_youtube_feed(sub.rss_url)
        for entry in reversed(new_entries):
            title = getattr(entry, "title", "(no title)")
            link = getattr(entry, "link", "")
            if is_yt:
                # YouTube：直接送連結，Discord 會自動展開成影片預覽
                content = f"{title}\n{link}".strip()
                message = self._format_message(content, sub.mention_user_id)
                await channel.send(message)
            else:
                # 一般 RSS：送純文字
                content = f"{title}\n{link}".strip()
                message = self._format_message(content, sub.mention_user_id)
                await channel.send(message)
            logging.info("Posted: %s", title)

    def _format_message(
        self, content: str, mention_user_id: Optional[int] = None
    ) -> str:
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

    @staticmethod
    def _is_youtube_feed(rss_url: str) -> bool:
        """判斷是否為 YouTube RSS Feed"""
        return "youtube.com/feeds/videos.xml" in rss_url

    @staticmethod
    def _build_yt_embed(entry, feed_info) -> discord.Embed:
        """建立 YouTube 新影片的 Discord Embed"""
        title = getattr(entry, "title", None) or entry.get("title", "（無標題）")
        link = getattr(entry, "link", None) or entry.get("link", "")
        channel_name = getattr(feed_info, "title", None) or feed_info.get(
            "title", "YouTube 頻道"
        )
        channel_url = getattr(feed_info, "link", None) or feed_info.get("link", "")
        published = getattr(entry, "published", None) or entry.get("published", "")

        embed = discord.Embed(
            title=title,
            url=link if link else None,
            color=discord.Color.red(),
        )

        if channel_name:
            embed.set_author(name=channel_name, url=channel_url or None)

        # 標題 + 連結固定顯示在 description（確保 Embed 不空白）
        desc_lines = [f"**{title}**"]
        if link:
            desc_lines.append(link)
        summary = getattr(entry, "summary", None) or entry.get("summary", "")
        if summary:
            desc_lines.append(summary[:200] + ("…" if len(summary) > 200 else ""))
        embed.description = "\n".join(desc_lines)

        # 縮圖（media:thumbnail）
        thumbnails = entry.get("media_thumbnail") or []
        if thumbnails:
            embed.set_image(url=thumbnails[0].get("url", ""))

        if published:
            embed.set_footer(text=f"📅 {published}")

        return embed


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
    parser.add_argument(
        "--test-yt",
        action="store_true",
        help="Send the latest YouTube video as an Embed to all YT-subscribed channels and exit",
    )
    args = parser.parse_args()

    # 載入 .env 檔案中的環境變數
    load_dotenv()

    # 取得必要的環境變數
    token = get_env_var("DISCORD_TOKEN")

    # 優先順序：SUBSCRIPTIONS_FILE > SUBSCRIPTIONS（JSON 字串）> 單一 CHANNEL_ID/RSS_URL
    subscriptions: List[Subscription] = []
    subs_file = os.getenv("SUBSCRIPTIONS_FILE", "").strip()
    subs_env = os.getenv("SUBSCRIPTIONS", "").strip()

    if subs_file:
        # 從外部 JSON 檔案讀取（推薦方式，方便維護）
        try:
            with open(subs_file, encoding="utf-8") as f:
                parsed = json.load(f)
        except FileNotFoundError:
            raise RuntimeError(f"SUBSCRIPTIONS_FILE 找不到：{subs_file}")
        except json.JSONDecodeError as exc:
            raise RuntimeError(f"SUBSCRIPTIONS_FILE JSON 格式錯誤：{exc}") from exc
        for item in parsed:
            cid = int(item["channel_id"])
            url = item["rss_url"]
            mid = (
                int(item["mention_user_id"])
                if item.get("mention_user_id") not in (None, "")
                else None
            )
            subscriptions.append(
                Subscription(channel_id=cid, rss_url=url, mention_user_id=mid)
            )
        logging.info("從 %s 載入 %d 筆訂閱", subs_file, len(subscriptions))
    elif subs_env:
        # 從環境變數 JSON 字串解析（向下相容）
        try:
            parsed = json.loads(subs_env)
            for item in parsed:
                cid = int(item["channel_id"])
                url = item["rss_url"]
                mid = (
                    int(item["mention_user_id"])
                    if item.get("mention_user_id") not in (None, "")
                    else None
                )
                subscriptions.append(
                    Subscription(channel_id=cid, rss_url=url, mention_user_id=mid)
                )
        except Exception as exc:
            raise RuntimeError(
                "Invalid SUBSCRIPTIONS format; must be JSON array of objects"
            ) from exc
    else:
        # 向下相容：使用單一 CHANNEL_ID / RSS_URL
        channel_id = int(get_env_var("CHANNEL_ID"))
        rss_url = get_env_var("RSS_URL")
        mention_user_id_str = os.getenv("MENTION_USER_ID", "").strip()
        mention_user_id = int(mention_user_id_str) if mention_user_id_str else None
        subscriptions.append(
            Subscription(
                channel_id=channel_id, rss_url=rss_url, mention_user_id=mention_user_id
            )
        )

    # 設定 Discord Bot 的 Intents（權限）
    intents = discord.Intents.default()
    # 建立 Bot 實例
    client = RssDiscordBot(
        subscriptions=subscriptions,
        test_message=args.test,
        test_yt=args.test_yt,
        intents=intents,
    )

    if args.test or args.test_yt:
        # 測試模式：不需要 HTTP 伺服器，直接執行
        client.run(token)
    else:
        # 正式模式：同時啟動 HTTP 健康檢查伺服器與 Discord Bot
        try:
            asyncio.run(_async_main(client, token))

        # 防止 Cloudflare 1015 連線過多導致的 Bot 崩潰，捕獲所有異常並在 30 秒後重試
        except Exception as ex:
            logging.exception("Bot crashed, restart in 30s")
            time.sleep(60)


async def _async_main(client: discord.Client, token: str) -> None:
    """非同步主函數：同時啟動健康檢查 HTTP 伺服器和 Discord Bot

    Render Web Service 需要監聽 HTTP 端口，否則服務會被視為無效。
    此函數同時啟動一個輕量 aiohttp 伺服器回應健康檢查，以及 Discord Bot。
    """

    # 建立健康檢查 HTTP 伺服器
    async def health(request: web.Request) -> web.Response:
        return web.Response(text="OK")

    app = web.Application()
    app.router.add_get("/", health)
    app.router.add_get("/health", health)

    runner = web.AppRunner(app)
    await runner.setup()
    port = int(os.getenv("PORT", "10000"))
    site = web.TCPSite(runner, "0.0.0.0", port)
    await site.start()
    logging.info("Health check server running on port %d", port)

    try:
        while True:
            client = RssDiscordBot(
                subscriptions=client.subscriptions, intents=discord.Intents.default()
            )
            try:
                async with client:
                    await client.start(token)
            except Exception as e:
                logging.exception("Bot crashed, retry in 60s")
                await asyncio.sleep(60)
    finally:
        await runner.cleanup()


if __name__ == "__main__":
    main()
