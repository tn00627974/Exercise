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
- `MENTION_USER_ID`：**（可選）** 右鍵用戶 → 複製使用者 ID，推播時會自動 @該用戶

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

## Docker 部署

**前置需求：**
- 安裝 [Docker Desktop](https://www.docker.com/products/docker-desktop/) 並確保已啟動
- Windows：確認右下角系統匣的 Docker 圖示顯示綠色

### 本地建置與執行

```bash
# 建置映像
docker build -t stock-rss-bot .

# 執行容器（使用 .env 檔案）
docker run -d --name stock-bot --env-file .env stock-rss-bot

# 或手動指定環境變數
# docker run -d --name stock-bot \
#   -e DISCORD_TOKEN="your_token" \
#   -e CHANNEL_ID="your_channel_id" \
#   -e RSS_URL="https://tw.stock.yahoo.com/rss" \
#   stock-rss-bot

# 查看日誌
docker logs -f stock-bot

# 停止與移除
docker stop stock-bot
docker rm stock-bot
```

### 使用 docker-compose（推薦）

建立 `docker-compose.yml`：

```yaml
version: '3.8'
services:
  bot:
    build: .
    env_file:
      - .env
    restart: unless-stopped
```

執行：

```bash
docker-compose up -d        # 背景執行
docker-compose logs -f      # 查看日誌
docker-compose down         # 停止
```

## Railway 部署

### 方式一：使用 Dockerfile（推薦）

1. Railway 建立新專案並連結此 repo
2. 在 Variables 設定：`DISCORD_TOKEN`、`CHANNEL_ID`、`RSS_URL`
3. Railway 會自動偵測 Dockerfile 並建置

### 方式二：直接執行 Python

1. Railway 建立新專案並連結此 repo
2. 在 Variables 設定：`DISCORD_TOKEN`、`CHANNEL_ID`、`RSS_URL`
3. Start Command 設定為：`python bot.py`

## 常見問題

- 看到 `Forbidden ...`：Bot 沒有該頻道的 `View Channel` / `Send Messages` 權限，或 Bot 尚未被邀請進伺服器。
