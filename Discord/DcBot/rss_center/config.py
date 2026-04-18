import json
import logging
import os
from typing import Any, Dict, List, Optional

from .models import Subscription


def get_env_var(name: str) -> str:
    """取得必要的環境變數。"""
    value = os.getenv(name)
    if value is not None:
        value = value.strip()
    if not value:
        raise RuntimeError(f"Missing environment variable: {name}")
    return value


def _to_optional_int(value: Any) -> Optional[int]:
    if value in (None, ""):
        return None
    return int(value)


def normalize_platform(value: str | None) -> str:
    platform = (value or "discord").strip().lower()
    aliases = {
        "dc": "discord",
        "discord": "discord",
        "linebot": "line",
        "line-bot": "line",
        "line": "line",
        "feishu": "feishu",
        "lark": "feishu",
    }
    return aliases.get(platform, platform)


def _build_subscription(
    item: Dict[str, Any], target: Optional[Dict[str, Any]] = None
) -> Subscription:
    """建立訂閱物件，支援舊版與新版多平台格式。"""
    source = target or item
    rss_url = str(item["rss_url"]).strip()
    platform = normalize_platform(source.get("platform"))

    display_name = (
        source.get("display_name")
        or item.get("display_name")
        or source.get("youtube_channel_name")
        or item.get("youtube_channel_name")
    )

    return Subscription(
        rss_url=rss_url,
        platform=platform,
        channel_id=_to_optional_int(source.get("channel_id")),
        mention_user_id=_to_optional_int(source.get("mention_user_id")),
        webhook_url=(source.get("webhook_url") or "").strip() or None,
        target_id=(source.get("target_id") or "").strip() or None,
        display_name=display_name,
        extra=dict(source),
    )


def load_subscriptions(subs_file: str) -> List[Subscription]:
    """從 JSON 檔載入訂閱中心設定。"""
    try:
        with open(subs_file, encoding="utf-8") as f:
            parsed = json.load(f)
    except FileNotFoundError:
        raise RuntimeError(f"SUBSCRIPTIONS_FILE 找不到：{subs_file}")
    except json.JSONDecodeError as exc:
        raise RuntimeError(f"SUBSCRIPTIONS_FILE JSON 格式錯誤：{exc}") from exc

    if not isinstance(parsed, list):
        raise RuntimeError("SUBSCRIPTIONS_FILE 必須是 JSON 陣列")

    subscriptions: List[Subscription] = []
    for item in parsed:
        if "rss_url" not in item:
            raise RuntimeError(f"訂閱缺少 rss_url：{item}")

        targets = item.get("targets")
        if targets is not None:
            if not isinstance(targets, list) or not targets:
                raise RuntimeError(f"targets 必須是非空陣列：{item}")
            for target in targets:
                subscriptions.append(_build_subscription(item, target))
        else:
            subscriptions.append(_build_subscription(item))

    if not subscriptions:
        raise RuntimeError("沒有任何有效訂閱，請檢查 subscriptions.json")

    logging.info("從 %s 載入 %d 筆通知目標", subs_file, len(subscriptions))
    return subscriptions
