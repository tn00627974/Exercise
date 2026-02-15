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

## Railway 雲端部署

### 前置準備

#### 1. 建立 Git Repository

```bash
# 初始化 Git（如果還未初始化）
git init

# 加入所有檔案
git add .

# 提交變更
git commit -m "Initial commit: Stock RSS Discord Bot"

# 在 GitHub 建立新 repository，然後連結並推送
git remote add origin https://github.com/你的用戶名/stock-rss-bot.git
git branch -M main
git push -u origin main
```

**注意：** 確保 `.gitignore` 已排除 `.env` 檔案，避免 Token 外洩。

#### 2. Railway 註冊與登入

1. 前往 [Railway.app](https://railway.app/)
2. 點擊「Login」，使用 GitHub 帳號登入（推薦）
3. 授權 Railway 存取你的 GitHub repositories

### 部署步驟

#### 方式一：使用 Dockerfile（推薦 ✅）

**優點：** 環境一致、易於維護、自動偵測

1. **建立新專案**
   - Railway 首頁點擊「New Project」
   - 選擇「Deploy from GitHub repo」
   - 選擇你的 `stock-rss-bot` repository
   - Railway 會自動偵測 `Dockerfile` 並開始建置

2. **設定環境變數**
   - 點擊部署的服務（service）
   - 切換到「Variables」頁籤
   - 點擊「+ New Variable」，逐一加入：
     ```
     DISCORD_TOKEN=你的_Discord_Bot_Token
     CHANNEL_ID=你的_Discord_頻道_ID
     RSS_URL=https://tw.stock.yahoo.com/rss?category=tw-market
     MENTION_USER_ID=要提及的用戶ID（可選）
     ```
   - 點擊「Add」儲存

3. **觸發重新部署**
   - 設定環境變數後，Railway 會自動重新部署
   - 或手動點擊 「Settings」→「Redeploy」

4. **查看部署狀態**
   - 「Deployments」頁籤可看到建置進度
   - 點擊最新的 deployment → 「View Logs」查看執行日誌
   - 看到 `Primed X items from RSS` 表示啟動成功

#### 方式二：直接執行 Python

**適用情境：** 不想使用 Docker，或想要更快的建置速度

1. **建立新專案**（同方式一）

2. **設定環境變數**（同方式一）

3. **設定 Start Command**
   - 點擊服務 → 「Settings」
   - 找到「Start Command」欄位
   - 輸入：`python bot.py`
   - 點擊「Deploy」

4. **Railway 會自動：**
   - 偵測 `requirements.txt`
   - 安裝 Python 依賴
   - 執行啟動命令

### 部署後檢查

```bash
# 確認日誌顯示：
✅ Logged in as YourBot#1234
✅ Primed XX items from RSS
✅ 每 5 分鐘應該會看到檢查 RSS 的日誌
```

### 更新程式碼

當你修改 `bot.py` 或其他檔案後：

```bash
git add .
git commit -m "更新說明"
git push
```

Railway 會自動偵測推送並重新部署（需在 Settings 啟用 Auto Deploy）。

### 費用說明

- **免費額度：** 每月 $5 USD 額度或 500 小時執行時間
- **此 Bot 用量：** 約 720 小時/月（24/7 運行）
- **建議：**
  - 監控 Railway Dashboard 的使用量
  - 超過免費額度可升級 Hobby Plan（$5/月無限時數）
  - 或設定每日定時啟動來節省時間

### 常見部署問題

**Q: 部署成功但 Bot 離線？**
- 檢查 `DISCORD_TOKEN` 是否正確
- 確認 Bot 已被邀請進 Discord 伺服器

**Q: 看到 `Forbidden` 錯誤？**
- Bot 需要 `View Channel` + `Send Messages` 權限
- 檢查 `CHANNEL_ID` 是否正確

**Q: Railway 顯示建置失敗？**
- 查看 Build Logs 找出錯誤訊息
- 確認 `Dockerfile` 和 `requirements.txt` 格式正確

## 常見問題

- 看到 `Forbidden ...`：Bot 沒有該頻道的 `View Channel` / `Send Messages` 權限，或 Bot 尚未被邀請進伺服器。
