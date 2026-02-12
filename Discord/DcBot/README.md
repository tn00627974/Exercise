# Stock RSS Discord Bot

每 5 分鐘抓一次 Yahoo 台股 RSS，偵測新文章後推播到指定 Discord 頻道，並避免重複推播。

## 需求

- Python 3.10+（建議 3.12）
- Discord Bot Token

## 安裝

```bash
python -m venv .venv
```

Windows PowerShell：

```powershell
& .\.venv\Scripts\Activate.ps1
python -m pip install -r requirements.txt
```

## 環境變數

把 [.env.example](.env.example) 複製成 `.env`，填入：

- `DISCORD_TOKEN`：Discord Developer Portal → Bot → Token
- `CHANNEL_ID`：Discord 開發者模式 → 右鍵頻道 → 複製頻道 ID
- `RSS_URL`：RSS 連結（例如：`https://tw.stock.yahoo.com/rss`）

## 測試（建議先跑）

送一則測試訊息到 `CHANNEL_ID`，送完會自動結束：

```bash
python bot.py --test "測試訊息：Bot 已成功連線並可發送"
```

## 正式執行

```bash
python bot.py
```

注意：正常模式啟動時會先「記住」目前 RSS 已有的文章，避免一開機就洗版；因此只有後續出現的新文章才會推播。

## Railway 部署

1. Railway 建立新專案並連結此 repo
2. 在 Variables 設定：`DISCORD_TOKEN`、`CHANNEL_ID`、`RSS_URL`
3. Start Command 設定為：`python bot.py`

## 常見問題

- 看到 `Forbidden ...`：Bot 沒有該頻道的 `View Channel` / `Send Messages` 權限，或 Bot 尚未被邀請進伺服器。
