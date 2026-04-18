"""台股 RSS 訂閱中心啟動入口。

分層架構：
- config：載入設定
- service：RSS 輪詢與去重
- notifiers：各平台通知
- bot.py：程式啟動與生命週期管理
"""

import argparse
import asyncio
import logging
import os
import time
from typing import Optional

import aiohttp
import discord
from aiohttp import web
from dotenv import load_dotenv

from rss_center.config import get_env_var, load_subscriptions
from rss_center.notifiers import (
    DiscordNotifier,
    FeishuNotifier,
    LineNotifier,
    NotificationRouter,
)
from rss_center.service import RssPollingService

POLL_INTERVAL_SECONDS = 300

logging.basicConfig(
    level=logging.INFO, format="[%(asctime)s] %(levelname)s: %(message)s"
)


class SubscriptionCenterBot(discord.Client):
    """僅負責 Discord 連線生命週期。"""

    def __init__(
        self,
        *,
        test_message: str | None = None,
        test_yt: bool = False,
        **options,
    ):
        super().__init__(**options)
        self.service: Optional[RssPollingService] = None
        self.test_message = test_message
        self.test_yt = test_yt
        self._poll_task: asyncio.Task | None = None

    def set_service(self, service: RssPollingService) -> None:
        self.service = service

    def _require_service(self) -> RssPollingService:
        if self.service is None:
            raise RuntimeError("RssPollingService 尚未初始化")
        return self.service

    async def setup_hook(self) -> None:
        if self.test_message or self.test_yt:
            return

        service = self._require_service()
        await service.prime_seen_ids()
        self._poll_task = asyncio.create_task(service.poll_loop(POLL_INTERVAL_SECONDS))

    async def on_ready(self) -> None:
        if not self.test_message and not self.test_yt:
            return

        service = self._require_service()
        if self.test_yt:
            await service.send_latest_youtube()
        else:
            await service.send_test_message(self.test_message or "RSS 訂閱中心測試成功")

        await asyncio.sleep(1)
        await self.close()

    async def close(self) -> None:
        if self._poll_task is not None:
            self._poll_task.cancel()
        await super().close()


async def _async_main(
    token: str,
    *,
    test_message: str | None,
    test_yt: bool,
) -> None:
    """非同步主函數：組裝 SRP 分層元件並執行。"""
    subscriptions = load_subscriptions(get_env_var("SUBSCRIPTIONS_FILE"))

    async with aiohttp.ClientSession() as session:
        client = SubscriptionCenterBot(
            test_message=test_message,
            test_yt=test_yt,
            intents=discord.Intents.default(),
        )

        router = NotificationRouter(
            {
                "discord": DiscordNotifier(client),
                "line": LineNotifier(session),
                "feishu": FeishuNotifier(session),
            }
        )
        client.set_service(RssPollingService(subscriptions, router))

        if test_message or test_yt:
            async with client:
                await client.start(token)
            return

        await _run_with_health_server(client, token)


async def _run_with_health_server(client: discord.Client, token: str) -> None:
    """正式模式：同時啟動健康檢查 HTTP 伺服器與 Bot。"""

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
        async with client:
            await client.start(token)
    finally:
        await runner.cleanup()


def main() -> None:
    """主程式進入點。"""
    parser = argparse.ArgumentParser(description="RSS subscription center")
    parser.add_argument(
        "--test",
        metavar="MESSAGE",
        help="Send a one-off test message to all subscribed targets and exit",
    )
    parser.add_argument(
        "--test-yt",
        action="store_true",
        help="Send the latest YouTube video to all YT-subscribed targets and exit",
    )
    args = parser.parse_args()

    load_dotenv()
    token = get_env_var("DISCORD_TOKEN")

    try:
        asyncio.run(
            _async_main(
                token,
                test_message=args.test,
                test_yt=args.test_yt,
            )
        )
    except Exception:
        logging.exception("Bot crashed, restart in 60s")
        time.sleep(60)


if __name__ == "__main__":
    main()
