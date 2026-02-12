import asyncio
import argparse
import logging
import os
from typing import Set

import discord
import feedparser
from dotenv import load_dotenv

POLL_INTERVAL_SECONDS = 300

logging.basicConfig(level=logging.INFO, format="[%(asctime)s] %(levelname)s: %(message)s")


def get_env_var(name: str) -> str:
    value = os.getenv(name)
    if value is not None:
        value = value.strip()
    if not value:
        raise RuntimeError(f"Missing environment variable: {name}")
    return value


class RssDiscordBot(discord.Client):
    def __init__(self, *, channel_id: int, rss_url: str, test_message: str | None = None, **options):
        super().__init__(**options)
        self.channel_id = channel_id
        self.rss_url = rss_url
        self.test_message = test_message
        self.seen_ids: Set[str] = set()
        self._task: asyncio.Task | None = None

    async def setup_hook(self) -> None:
        if self.test_message:
            return

        await self._prime_seen_ids()
        self._task = asyncio.create_task(self._poll_loop())

    async def on_ready(self) -> None:
        if not self.test_message:
            return

        try:
            channel = await self.fetch_channel(self.channel_id)
            await channel.send(self.test_message)
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
            await self.close()

    async def _prime_seen_ids(self) -> None:
        feed = feedparser.parse(self.rss_url)
        for entry in feed.entries:
            entry_id = self._entry_id(entry)
            if entry_id:
                self.seen_ids.add(entry_id)
        logging.info("Primed %s items from RSS", len(self.seen_ids))

    async def _poll_loop(self) -> None:
        await self.wait_until_ready()
        try:
            channel = await self.fetch_channel(self.channel_id)
        except discord.NotFound:
            logging.error(
                "Channel not found (ID %s). Check CHANNEL_ID and that the bot is in the server.",
                self.channel_id,
            )
            return
        except discord.Forbidden:
            logging.error(
                "Forbidden fetching channel %s. Check the bot has View Channel/Send Messages permissions.",
                self.channel_id,
            )
            return
        except discord.HTTPException as exc:
            logging.error("Failed to fetch channel %s: %s", self.channel_id, exc)
            return

        while not self.is_closed():
            try:
                await self._poll_once(channel)
            except Exception as exc:
                logging.exception("Polling failed: %s", exc)
            await asyncio.sleep(POLL_INTERVAL_SECONDS)

    async def _poll_once(self, channel: discord.abc.Messageable) -> None:
        feed = feedparser.parse(self.rss_url)
        new_entries = []
        for entry in feed.entries:
            entry_id = self._entry_id(entry)
            if not entry_id or entry_id in self.seen_ids:
                continue
            self.seen_ids.add(entry_id)
            new_entries.append(entry)

        if not new_entries:
            return

        for entry in reversed(new_entries):
            title = getattr(entry, "title", "(no title)")
            link = getattr(entry, "link", "")
            message = f"{title}\n{link}".strip()
            await channel.send(message)
            logging.info("Posted: %s", title)

    @staticmethod
    def _entry_id(entry) -> str:
        return getattr(entry, "id", None) or getattr(entry, "link", None) or ""


def main() -> None:
    parser = argparse.ArgumentParser(description="RSS to Discord bot")
    parser.add_argument(
        "--test",
        metavar="MESSAGE",
        help="Send a one-off test message to CHANNEL_ID and exit",
    )
    args = parser.parse_args()

    load_dotenv()

    token = get_env_var("DISCORD_TOKEN")
    channel_id = int(get_env_var("CHANNEL_ID"))
    rss_url = get_env_var("RSS_URL")

    intents = discord.Intents.default()
    client = RssDiscordBot(
        channel_id=channel_id,
        rss_url=rss_url,
        test_message=args.test,
        intents=intents,
    )
    client.run(token)


if __name__ == "__main__":
    main()
