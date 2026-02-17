# 台股 RSS Discord Bot 📈

![Status](https://img.shields.io/badge/Status-Production%20Ready-green) ![Python](https://img.shields.io/badge/Python-3.12+-blue) ![Discord.py](https://img.shields.io/badge/Discord.py-2.4.0-5865F2)

每 **5 分鐘** 自動抓一次 Yahoo 台股 RSS，偵測新文章後推播到指定 Discord 頻道，智能避免重複推播。

### 🎯 功能特色

- ✅ **自動輪詢 RSS：** 每 5 分鐘檢查一次 RSS Feed，實時推播新文章
- ✅ **智能去重：** 使用 Set 結構追蹤已推播文章，重啟不會洗版
- ✅ **@提及用戶：** 支援在推播時自動提及指定用戶
- ✅ **24/7 運行：** 在 Railway 雲端平台持續運行，無需本地電腦開啟
- ✅ **產線級部署：** 已部署至 Railway，正式投入使用
- ✅ **簡單配置：** 只需環境變數，無複雜設定

## 📋 需求

- Python 3.10+（建議 3.12）
- Discord Bot Token（[取得教學](#取得-discord-bot-token)）
- （可選）Railway 帳號用於雲端部署

## 🚀 快速開始（本地開發）

### 1. 複製專案

```bash
git clone https://github.com/tn00627974/Exercise.git
cd Exercise/Discord/DcBot
```

### 2. 建立虛擬環境並安裝依賴

**Windows PowerShell：**
```powershell
python -m venv .venv
& .\.venv\Scripts\Activate.ps1
pip install -r requirements.txt
```

**macOS / Linux：**
```bash
python -m venv .venv
source .venv/bin/activate
pip install -r requirements.txt
```

### 3. 設定環境變數

複製 `.env.example` 為 `.env`，填入以下資訊：

```env
DISCORD_TOKEN=你的_Discord_Bot_Token
CHANNEL_ID=目標_Discord_頻道_ID
RSS_URL=https://tw.stock.yahoo.com/rss?category=tw-market
MENTION_USER_ID=要_@_提及的用戶_ID（可選）
```

**如何取得各個值？** 見[附錄](#-附錄)

### 4. 本地測試

送一則測試訊息到 Discord 頻道（馬上執行完後自動結束）：

```bash
python bot.py --test "🤖 Bot 已成功連線！"
```

✅ 如果看到訊息出現在 Discord 頻道，表示配置正確。

### 5. 正式執行

```bash
python bot.py
```

看到以下日誌表示啟動成功：
```
[INFO] Primed 50 items from RSS
[INFO] Shard ID None has connected to Gateway
```

⚠️ **注意：** 啟動時會先「記住」目前 RSS 的文章，只有後續出現的**新文章**才會推播，避免一開機就洗版。

---

## ✨ 部署狀態

### 🟢 正式環境（Railway）- 已部署 ✅

Bot 已部署在 Railway 雲端平台，**24/7 持續運行中**！

**部署資訊：**
- 🖥 **平台：** Railway.app（美國 us-west1 機房）
- 🐍 **Python：** 3.12（自動偵測）
- 📦 **構建方式：** Railpack（自動構建）
- ⏱ **運行狀態：** 持續運行
- 💰 **費用：** 在免費額度內

**實時日誌輸出：**
```
[2026-02-16 13:51:03] [INFO] logging in using static token
[2026-02-16 13:51:05] [INFO] Primed 50 items from RSS
[2026-02-16 13:51:05] [INFO] Shard ID None has connected to Gateway (Session ID: 887e9e6423c6b11b58f86a63f995abb2)
```

### 👉 雲端運行說明

Bot 現在在 Railway 上自主運行，**不需要本地電腦開啟**。

- ✅ RSS 每 5 分鐘自動檢查一次
- ✅ 新文章自動推播到 Discord 頻道
- ✅ 支援全天候 (24/7) 不中斷運行
- ✅ Railway 自動處理網路連線和故障恢復

**監控日誌方法：**
1. Railway Dashboard → 你的服務 (Discord Bot)
2. 點擊「Logs」標籤
3. 實時查看 Bot 執行日誌

---

## 🐳 本地 Docker 部署
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

## 🌐 雲端部署（Railway）

### ✅ 已部署成功的方式：GitHub 自動部署

使用者已透過 **GitHub 連接** 方式成功部署，該方式具有以下優點：

- ✅ 自動化部署：每次 `git push` 都會自動重新部署
- ✅ 版本控制：完整的程式碼版本記錄
- ✅ 環境一致：開發環境與線上環境配置相同

**現行配置：**
- GitHub Repository：`https://github.com/tn00627974/Exercise`
- Railway 自動監控：Discord/DcBot 子目錄
- 啟動命令：`python bot.py`（Railpack 自動偵測）

### 📝 部署其他選項（供參考）

| 方式 | 需要 GitHub | 難度 | 自動部署 | 適用情景 |
|------|-----------|------|--------|---------|
| **GitHub 連接** ✔️ | ✅ | 簡單 | ✅ 自動 | 推薦（已部署） |
| Railway CLI | ❌ | 中等 | 手動 | 快速測試、不想用 GitHub |
| Docker 本地執行 | ❌ | 中等 | 無 | 本地開發、離線執行 |

### 🔄 更新部署代碼

當修改程式碼後，只需推送到 GitHub，Railway 會自動部署：

```bash
git add .
git commit -m "修改說明"
git push origin main
```

Railway 會自動：
1. 拉取最新代碼
2. 檢測變更
3. 自動重新構建
4. 重新啟動 Bot

### 📊 監控 Bot 運行狀態

**在 Railway Dashboard 查看：**

1. 進入專案 → 選擇 **Discord Bot** Service
2. 點擊 **Deployments** 標籤
   - 查看最新部署狀態
   - 檢查構建是否成功
3. 點擊 **Logs** 標籤
   - 實時查看 Bot 執行日誌
   - 看到 `Primed X items from RSS` 表示正常運行

**期望看到的日誌：**
```
[INFO] logging in using static token          # Bot 認證成功
[INFO] Primed 50 items from RSS               # 初始化 RSS 源成功
[INFO] Shard ID None has connected to Gateway # 連接 Discord 成功
```

### ⚙️ 修改 Railway 環境變數

如果需要更新 Discord Token、頻道 ID 等：

1. Railway Dashboard → 你的 Service
2. 點擊 **Variables** 標籤
3. 點擊 **Raw Editor**
4. 修改對應的環境變數
5. 點擊 **Save**（Railway 會自動重新部署）

### 🆘 部署常見問題

**Q: 如何檢查 Bot 是否還在線？**
```
Railway Logs > 看最後幾秒的訊息  
如果最後是 "Shard ID None has connected to Gateway" 表示線上
```

**Q: 推播訊息沒有出現？**
1. 檢查 `CHANNEL_ID` 是否正確
2. 檢查 Bot 是否有 `View Channel` + `Send Messages` 權限
3. 查看 Railway Logs 是否有錯誤訊息

**Q: Bot 自動離線？**
1. 檢查 Railway 是否有足夠的 $5 免費額度（24/7 約用 720 小時/月）
2. 查看 Railway Dashboard 的 Service Health 狀態
3. 點擊最新 Deployment 查看是否有錯誤日誌

---

## 🆘 本地部署替代方案（不推薦）

---

## 📚 附錄

### 取得 Discord Bot Token

1. 進入 [Discord Developer Portal](https://discord.com/developers/applications)
2. 點擊「New Application」，輸入 Bot 名稱
3. 左側選單 → 「Bot」
4. 點擊「Reset Token」→ 「Yes, do it!」
5. 點擊「Copy」複製 Token
6. **⚠️ 重要：** Token 類似密碼，不要洩露給他人！

### 取得 Channel ID

1. Discord 設定 → 「Advanced」 → 開啟「Developer Mode」
2. 在頻道上方右鍵 → 「複製頻道 ID」
3. 貼到 `CHANNEL_ID` 變數中

### 取得 User ID（可選，用來提及用戶）

1. Discord 開發者模式已開啟的情況下
2. 右鍵點擊要提及的用戶 → 「複製使用者 ID」
3. 貼到 `MENTION_USER_ID` 變數中

### 項目依賴說明

| 套件 | 版本 | 用途 |
|------|------|------|
| discord.py | 2.4.0 | Discord Bot 框架 |
| feedparser | 6.0.11 | 解析 RSS Feed |
| python-dotenv | 1.0.1 | 載入 .env 環境變數 |
| audioop-lts | 0.2.0+ | Python 3.13+ 音訊支援（可選） |

## 📖 常見問題

### 本地執行相關

**Q: 執行 `python bot.py` 出現 `ModuleNotFoundError`？**
```
A: 確認已激活虛擬環境並安裝依賴：
   & .\.venv\Scripts\Activate.ps1
   pip install -r requirements.txt
```

**Q: `CHANNEL_ID` 輸入錯誤會怎樣？**
```
A: Bot 會因為找不到頻道而無法發送訊息，查看日誌會看到：
   "Channel not found (ID xxx)"
   
   檢查並更正 .env 中的 CHANNEL_ID
```

**Q: Bot 啟動時為什麼不推送已存在的文章？**
```
A: 這是設計行為。Bot 啟動時會先「記住」目前 RSS 的所有文章，
   只有啟動後新出現的文章才會推播，避免一開機就洗版。
```

### Railway 部署相關

**Q: Bot 在 Railway 上突然離線？**
```
A: 可能原因：
   1. 免費額度已用完（24/7 約用 720 小時/月）
      → 檢查 Railway Dashboard 的使用額度
      → 升級到 Hobby Plan 或手動重新部署
   
   2. 程式碼錯誤導致 Bot 崩潰
      → Railway 上會看到 Build Error 或 Runtime Error
      → 查看 Logs 頁籤找出錯誤訊息
   
   3. Discord 連線中斷
      → Railway 應該會自動重連，如沒有則手動 Redeploy
```

**Q: 修改代碼後，Railway 沒有自動部署？**
```
A: Railway 檢查部署的步驟：
   1. 確認 git push 已推送到 main 分支
   2. Railway 應該在 1-2 分鐘內自動偵測並部署
   3. 如果沒有自動部署，可手動觸發：
      - Railway Dashboard → Deployments → 點擊最新部署 → Redeploy
```

**Q: 看到 `Build Error`？**
```
A: 常見原因：
   1. requirements.txt 格式錯誤
      → 檢查每行是否為 package==version 格式
   
   2. bot.py 有語法錯誤
      → 查看 Railway Build Logs 中的詳細錯誤訊息
      → 本地執行 python bot.py 測試是否有問題
   
   3. 環境變數缺失
      → 檢查是否在 Railway Variables 中設定了必要的環境變數
```

**Q: 如何檢查 Bot 是否正常推播？**
```
A: 檢查方法：
   1. Railway Logs 應該看到每 5 分鐘的 RSS 檢查訊息
   2. 當 RSS 有新文章時，會看到類似的日誌：
      "[INFO] Found new article: 文章標題"
   3. 檢查你的 Discord 頻道是否有新推播訊息
```

### 一般常見問題

**Q: Bot 顯示 `Forbidden` 錯誤？**
```
A: Bot 缺少必要權限或無法訪問頻道。
   
   檢查清單：
   1. Bot 已加入 Discord 伺服器？
      → 檢查伺服器成員列表中是否有該 Bot
   
   2. Bot 對目標頻道有「View Channel」權限？
      → 頻道設置 → 權限 → 找到 Bot → 勾選「View Channel」
   
   3. Bot 有「Send Messages」權限？
      → 同上，勾選「Send Messages」
   
   4. 頻道是否被限制為只有特定角色能發言？
      → 頻道設置 → 權限 → 確認 Bot 有發言權
```

**Q: 推播訊息格式如何修改？**
```
A: 編輯 bot.py 中的 _format_article() 方法（約第 200-220 行）
   修改後 git push，Railway 會自動部署新版本。
```

## 📞 技術支援

遇到問題時，檢查以下資源：

1. **本 README 的常見問題部分**
2. **Railway Logs** - 查看詳細的執行日誌
3. **本地測試** - 用 `python bot.py --test` 確認程式邏輯沒問題
4. **Discord Developer Portal** - 確認 Bot 權限和配置

## 📄 授權

MIT License - 詳見 LICENSE 檔案

## 🙏 致謝

- [discord.py](https://discordpy.readthedocs.io/) - 強大的 Discord Bot 框架
- [feedparser](https://feedparser.readthedocs.io/) - RSS 解析庫
- [Railway](https://railway.app/) - 免費雲端部署平台
