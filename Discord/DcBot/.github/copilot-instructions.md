# 多源快訊 Bot (BOT2l4)

## 專案概覽
多平台 RSS 訂閱推播 Bot，支援 Discord、LINE、飛書三個通知平台。
每 5 分鐘抓一次 RSS Feed（Yahoo 台股、YouTube 等），推播新文章到指定頻道，並以 ID 去重避免重複推播。

---

## 專案資料結構

```
DcBot/
├── bot.py                                     # 程式唯一入口，負責啟動生命週期
├── line_userid_webhook.py                     # 工具：取得 LINE User ID 的 Webhook
├── requirements.txt                           # Python 套件依賴
├── subscriptions.json                         # 訂閱設定（正式使用）
├── subscriptions.multiplatform.example.json   # 多平台訂閱範例
├── render.yaml                                # Render 部署設定
├── Dockerfile                                 # Docker 映像設定
├── docker-compose.yml                         # 本地 Docker 執行設定
├── .env                                       # 本地環境變數（不提交 Git）
├── .env.example                               # 環境變數範本
│
├── rss_center/                                # 核心業務邏輯（SRP 分層）
│   ├── __init__.py
│   ├── models.py       # 資料結構：Subscription dataclass
│   ├── config.py       # 設定載入：env var、subscriptions.json 解析
│   ├── formatters.py   # 訊息格式化：RSS 條目 → 文字訊息
│   ├── notifiers.py    # 通知發送：Discord / LINE / 飛書
│   └── service.py      # RSS 輪詢服務：去重、分派
│
├── test/                                      # 單元與整合測試
│   ├── test_bot_cli.py             # CLI 引數解析測試
│   ├── test_rss.py                 # RSS 解析測試
│   ├── test_subscriptions.py       # subscriptions.json 載入測試
│   ├── test_subscription_center.py # 訂閱中心整合測試
│   └── test_line_userid_webhook.py # LINE Webhook 測試
│
├── .github/
│   ├── copilot-instructions.md     # AI Agent 指引（本檔）
│   └── skills/
│       └── rss-discord-bot/        # Copilot skill 專用指引
│
└── railway_error/                             # Railway 部署錯誤 log 存檔
```

---

## SRP 分層架構（bot.py）

| 函式 / 類別 | 單一職責 |
|---|---|
| `_parse_args()` | CLI 引數定義與解析 |
| `_setup_logging()` | 日誌等級設定 |
| `_run_bot()` | 非同步主迴圈執行與錯誤處理 |
| `main()` | 進入點協調（4 行） |
| `SubscriptionCenterBot` | Discord Client 連線生命週期 |
| `_async_main()` | 元件組裝與模式判斷（測試 vs 正式） |
| `_run_with_health_server()` | 正式模式：HTTP 健康檢查 + Bot |

## SRP 分層架構（rss_center/）

| 模組 | 單一職責 |
|---|---|
| `models.py` | `Subscription` dataclass 定義 |
| `config.py` | `get_env_var()`、`load_subscriptions()`、`normalize_platform()` |
| `formatters.py` | `format_feed_message()`、`format_discord_message()` |
| `notifiers.py` | `DiscordNotifier`、`LineNotifier`、`FeishuNotifier`、`NotificationRouter` |
| `service.py` | `RssPollingService`：輪詢、去重、通知分派 |

---

## 環境變數

| 變數名稱 | 說明 | 必填 |
|---|---|---|
| `DISCORD_TOKEN` | Discord Bot Token | ✅ |
| `SUBSCRIPTIONS_FILE` | 訂閱設定 JSON 路徑，預設 `subscriptions.json` | ✅ |
| `LINE_CHANNEL_ACCESS_TOKEN` | LINE Messaging API Channel Access Token | LINE 平台必填 |
| `FEISHU_WEBHOOK_URL` | 飛書 Webhook URL（全域預設） | 飛書平台必填 |
| `PORT` | HTTP 健康檢查埠號，預設 `10000` | Render 必填 |

---

## 執行指令

```bash
# 正常啟動
python bot.py

# 測試：對所有訂閱目標發送一次訊息
python bot.py --test "測試訊息"

# 測試：僅對 LINE 目標發送一次訊息
python bot.py --test-line "LINE 測試"

# 測試：對 YouTube 訂閱推送最新影片
python bot.py --test-yt

# 開啟 DEBUG 日誌
python bot.py --debug

# 執行測試套件
python -m pytest test/
```

---

## 訂閱設定格式（subscriptions.json）

```json
[
  {
    "rss_url": "https://...",
    "display_name": "頻道名稱",
    "targets": [
      { "platform": "discord", "channel_id": 123456789, "mention_user_id": 987654321 },
      { "platform": "line",    "target_id": "Uxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx" },
      { "platform": "feishu",  "webhook_url": "https://open.feishu.cn/..." }
    ]
  }
]
```

支援的 `platform` 值：`discord` / `dc`、`line` / `linebot` / `line-bot`、`feishu` / `lark`

---

## Python 套件依賴

```
discord.py==2.4.0
feedparser==6.0.11
python-dotenv==1.0.1
aiohttp==3.11.18
```

---

## 部署選項

| 平台 | 方案 | 備註 |
|---|---|---|
| **Render** | Web Service 免費方案 | 需要 `PORT` 環境變數，長期運行推薦；網址：https://render.com/ |
| **Railway** | 每月 $5 免費額度 | 2020/12/08 起不再提供完全免費運行；網址：https://railway.com/ |
| **Oracle Cloud** | Always Free VM | 完全控制，參考 `Oracle Cloud 佈署.md` |
| **Docker** | 本地或自架伺服器 | 參考 `Dockerfile` 與 `docker-compose.yml` |

---

## AI Agent 開發指引

### 修改原則
- **不要破壞去重機制**（`seen_ids_map`），避免重複推播
- **保留所有 `--test*` CLI 旗標**，這是驗證推播的主要手段
- **保留 HTTP 健康檢查端點**（`/` 和 `/health`），Render 依賴此端點
- **`rss_center/` 內使用絕對匯入**（`from rss_center.xxx import ...`），不使用相對匯入（`from .xxx`）

### 新增平台通知
1. 在 `rss_center/notifiers.py` 新增繼承 `BaseNotifier` 的類別
2. 在 `rss_center/config.py` 的 `normalize_platform()` 加入平台別名
3. 在 `bot.py` 的 `_async_main()` 的 `NotificationRouter` 字典中註冊
4. 在 `subscriptions.json` 對應目標加入新 `platform` 值

### 新增 RSS Feed
只需在 `subscriptions.json` 加入新項目，無需修改程式碼。


