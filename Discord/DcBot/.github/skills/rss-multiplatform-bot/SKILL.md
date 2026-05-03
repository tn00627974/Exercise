---
name: rss-multiplatform-bot
description: >
  Complete domain knowledge for the multi-platform RSS notification bot (BOT2l4).
  Use for any task: adding platforms, modifying batch logic, fixing RSS feeds,
  writing tests, adjusting formatters, or deploying to Render/Docker/Oracle Cloud.
argument-hint: 'Describe what you want to change: add platform, fix feed, update formatter, write test, or deploy.'
user-invocable: true
---

# 多平台 RSS 通知 Bot — 完整技術技能

Use this skill for any work in this repository. It contains the full architecture,
data flow, extension patterns, and guardrails needed to modify or extend the bot safely.

---

## 1. Entry Point & Startup Flow

File: [bot.py](../../../bot.py)

```
main()
 └─ _parse_args()          # CLI flags
 └─ _setup_logging()       # log level
 └─ _run_bot()             # asyncio.run(_async_main())
      └─ _async_main()
           ├─ load_subscriptions()          # parse subscriptions.json
           ├─ DiscordBatchNotifier(client)
           ├─ LineBatchNotifier(session)
           ├─ FeishuBatchNotifier(session)
           ├─ NotificationRouter({...})
           ├─ RssPollingService(subs, router)
           └─ _run_with_health_server()     # production mode only
                ├─ aiohttp web on GET / and /health → "OK"
                └─ client.start(token)
```

**Test modes** skip `_run_with_health_server` and call `client.start()` directly,
then close after one delivery cycle.

---

## 2. Module Map (rss_center/)

All imports inside `rss_center/` use **absolute imports** (`from rss_center.xxx import ...`).
Never use relative imports (`from .xxx`).

| File | Class / Function | Responsibility |
|---|---|---|
| `models.py` | `Subscription` dataclass | Single subscription → single target mapping |
| `config.py` | `get_env_var()` | Read required env var; raises if missing |
| `config.py` | `load_subscriptions(path)` | Parse subscriptions.json → `List[Subscription]` |
| `config.py` | `normalize_platform(value)` | Alias → canonical platform string |
| `config.py` | `_build_subscription(item, target)` | Build one `Subscription` from JSON dicts |
| `formatters.py` | `format_feed_message(sub, entry, feed_info)` | RSS entry → cross-platform text string |
| `formatters.py` | `format_discord_message(content, mention_user_id)` | Prepend `<@id>` mention if set |
| `formatters.py` | `is_youtube_feed(rss_url)` | Detect YouTube feed by URL pattern |
| `notifiers.py` | `BaseNotifier` | Abstract `send(sub, content)` |
| `notifiers.py` | `NotificationRouter` | Dispatch `send()` to correct notifier by platform |
| `batch_queue.py` | `BatchQueue` | Per-target queue; fires on count OR timeout |
| `batch_queue.py` | `BatchQueueManager` | Dict of `BatchQueue`; manages lifecycle |
| `batch_notifier.py` | `BaseBatchNotifier(BaseNotifier)` | Enqueue → merge → push; subclass per platform |
| `batch_notifier.py` | `DiscordBatchNotifier` | Sends to Discord channel via `client.fetch_channel` |
| `batch_notifier.py` | `LineBatchNotifier` | Sends to LINE via Messaging API push endpoint |
| `batch_notifier.py` | `FeishuBatchNotifier` | Sends to Feishu/Lark via webhook URL |
| `service.py` | `RssPollingService` | Poll feeds, deduplicate, dispatch to router |

---

## 3. Data Flow (normal mode)

```
subscriptions.json
      │ load_subscriptions()
      ▼
List[Subscription]
      │
      ▼
RssPollingService.poll_loop()
  │ feedparser.parse(rss_url)
  │ seen_ids_map dedup (per rss_url)
  │ reversed new_entries (oldest first)
  ▼
format_feed_message(sub, entry, feed_info)
      │
      ▼
NotificationRouter.send(sub, content)
  │ normalize_platform(sub.platform)
  ▼
{discord|line|feishu}BatchNotifier.send(sub, content)
  │ BatchQueueManager.enqueue(key, content)
  │   triggers on: count >= max_batch_size  OR  time >= max_wait_seconds
  ▼
BaseBatchNotifier._do_flush(target_id)
  │ drain() → _merge() → _push()
  ▼
Platform API call (one HTTP/WS call per flush)
```

---

## 4. Subscription dataclass

```python
@dataclass
class Subscription:
    rss_url: str
    platform: str = "discord"         # normalized by normalize_platform()
    channel_id: Optional[int] = None  # Discord channel
    mention_user_id: Optional[int] = None
    webhook_url: Optional[str] = None  # Feishu
    target_id: Optional[str] = None    # LINE user/group ID
    display_name: Optional[str] = None
    extra: Dict[str, Any] = field(default_factory=dict)
```

One JSON `rss_url` item with N `targets[]` entries produces N `Subscription` objects.

---

## 5. subscriptions.json Schema

```json
[
  {
    "rss_url": "https://...",
    "display_name": "頻道名稱",
    "targets": [
      { "platform": "discord", "channel_id": 123456789, "mention_user_id": 987654321 },
      { "platform": "line",    "target_id": "Uxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx" },
      { "platform": "feishu",  "webhook_url": "https://open.feishu.cn/open-apis/bot/v2/hook/..." }
    ]
  }
]
```

Supported `platform` aliases:

| JSON value | Canonical |
|---|---|
| `discord`, `dc` | `discord` |
| `line`, `linebot`, `line-bot` | `line` |
| `feishu`, `lark` | `feishu` |

---

## 6. Environment Variables

| Variable | Required | Notes |
|---|---|---|
| `DISCORD_TOKEN` | Always | Discord Bot Token |
| `SUBSCRIPTIONS_FILE` | Always | Default: `subscriptions.json` |
| `LINE_CHANNEL_ACCESS_TOKEN` | LINE only | LINE Messaging API token |
| `FEISHU_WEBHOOK_URL` | Feishu only | Global fallback if `webhook_url` not in JSON |
| `PORT` | Render | Health server port, default `10000` |

`get_env_var(name)` raises `RuntimeError` immediately if a required var is missing.

---

## 7. CLI Flags

```bash
python bot.py                         # normal polling mode
python bot.py --test "訊息"            # send to ALL targets once, then exit
python bot.py --test-line "訊息"       # send to LINE targets only, then exit
python bot.py --test-yt               # send latest YouTube video to YT targets, then exit
python bot.py --debug                 # set log level DEBUG
python -m pytest test/               # run test suite
```

---

## 8. BatchNotifier Extension Pattern

Each platform notifier controls two flush triggers:

| Parameter | Discord default | LINE default | Feishu default |
|---|---|---|---|
| `max_batch_size` | 1 | 10 | 1 |
| `max_wait_seconds` | 1 | 3600 | 1 |
| `_MAX_CONTENT_LENGTH` | 2000 | 5000 | 5000 |

Queue key:
- Discord → `str(channel_id)`
- LINE → `target_id`
- Feishu → `webhook_url`

On `bot.close()`, all batch notifiers call `shutdown()` to flush remaining queued messages.

---

## 9. How to Add a New Platform

1. **`rss_center/notifiers.py`** — add class inheriting `BaseNotifier` (or skip if using batch).
2. **`rss_center/batch_notifier.py`** — add class inheriting `BaseBatchNotifier`:
   - Set `_MAX_CONTENT_LENGTH`
   - Implement `_target_key(sub) → str`
   - Implement `async _push(target_id, content) → None`
3. **`rss_center/config.py`** — add aliases to `normalize_platform()`.
4. **`bot.py` `_async_main()`** — instantiate notifier and register in `NotificationRouter` dict and `client.register_batch_notifier()`.
5. **`subscriptions.json`** — add `{ "platform": "newplatform", ... }` to `targets[]`.

---

## 10. How to Add a New RSS Feed

Edit `subscriptions.json` only. No code changes needed.

```json
{
  "rss_url": "https://example.com/feed.xml",
  "display_name": "顯示名稱",
  "targets": [
    { "platform": "discord", "channel_id": 123456789 }
  ]
}
```

---

## 11. Deduplication Mechanism

`RssPollingService.seen_ids_map: Dict[str, Set[str]]`

- Key: `rss_url`
- Value: set of `entry.id` (fallback: `entry.link`)
- Populated by `prime_seen_ids()` at startup to prevent flood on restart
- Never clear or bypass `seen_ids_map`; it is the sole guard against duplicate posts

---

## 12. Health Check

Endpoint: `GET /` and `GET /health` → `200 OK` text body `"OK"`

- Required by Render free tier to keep the service alive
- Runs concurrently with the Discord bot inside `_run_with_health_server()`
- Port configurable via `PORT` env var (default `10000`)
- Must not be removed or moved to a separate process

---

## 13. Test Files

| File | What it covers |
|---|---|
| `test/test_bot_cli.py` | `_parse_args()` flag parsing |
| `test/test_rss.py` | RSS entry parsing with feedparser |
| `test/test_subscriptions.py` | `load_subscriptions()` JSON schema |
| `test/test_subscription_center.py` | Integration: poll → format → notify flow |
| `test/test_line_userid_webhook.py` | LINE webhook helper tool |

---

## 14. Guardrails

- **Do not** remove or bypass `seen_ids_map` deduplication.
- **Do not** remove `--test`, `--test-yt`, `--test-line` CLI flags.
- **Do not** remove the HTTP health endpoints `/` and `/health`.
- **Do not** use relative imports inside `rss_center/`.
- **Do not** block the asyncio event loop with synchronous I/O.
- **Do not** call `feedparser.parse()` with a custom agent string other than the Chrome UA already in `_FEEDPARSER_AGENT` — some feeds reject bot UAs.
- When modifying `_build_subscription()`, preserve backward compatibility with old single-target JSON format (no `targets[]` key).

---

## 15. Deployment Cheatsheet

| Platform | How | Key requirement |
|---|---|---|
| Render | Web Service free tier | Set `PORT`, health endpoint alive |
| Railway | Docker or Nixpacks | `$5/month` credit model since 2024 |
| Oracle Cloud | Always Free VM | SSH + systemd or Docker Compose |
| Docker local | `docker compose up` | `.env` file with all env vars |

Start command for all: `python bot.py`
