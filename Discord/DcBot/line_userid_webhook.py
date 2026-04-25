"""簡易 LINE Webhook：收到訊息時印出 source.userId。"""

import logging
import os

import aiohttp
from aiohttp import web
from dotenv import load_dotenv

logging.basicConfig(
    level=logging.INFO,
    format="[%(asctime)s] %(levelname)s: %(message)s",
)


async def reply_text(reply_token: str, message: str) -> None:
    """可選：回覆使用者一則簡單文字。"""
    access_token = os.getenv("LINE_CHANNEL_ACCESS_TOKEN", "").strip()
    if not access_token or not reply_token:
        return

    headers = {
        "Authorization": f"Bearer {access_token}",
        "Content-Type": "application/json",
    }
    payload = {
        "replyToken": reply_token,
        "messages": [{"type": "text", "text": message[:1000]}],
    }

    async with aiohttp.ClientSession() as session:
        async with session.post(
            "https://api.line.me/v2/bot/message/reply",
            headers=headers,
            json=payload,
        ) as response:
            if response.status >= 400:
                body = await response.text()
                logging.error("LINE reply failed: %s %s", response.status, body)


async def handle_callback(request: web.Request) -> web.Response:
    """接收 LINE webhook event，印出 source.userId。"""
    payload = await request.json()

    for event in payload.get("events", []):
        source = event.get("source", {})
        user_id = source.get("userId")

        if user_id:
            logging.info("LINE source.userId: %s", user_id)
            print(f"LINE source.userId: {user_id}", flush=True)

        reply_token = event.get("replyToken")
        if reply_token and user_id:
            await reply_text(reply_token, f"你的 LINE userId 是：{user_id}")

    return web.json_response({"ok": True})


async def health(_: web.Request) -> web.Response:
    return web.Response(text="OK")


def create_app() -> web.Application:
    app = web.Application()
    app.router.add_get("/", health)
    app.router.add_get("/health", health)
    app.router.add_post("/callback", handle_callback)
    return app


def main() -> None:
    load_dotenv()
    port = int(os.getenv("PORT", "8080"))
    logging.info("LINE webhook server listening on port %d", port)
    web.run_app(create_app(), host="0.0.0.0", port=port)


if __name__ == "__main__":
    main()
