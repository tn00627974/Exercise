# 台股 RSS Discord Bot (BOT2l4)

功能：
- 每 5 分鐘抓一次 Yahoo 台股 RSS
- 推播新文章到 Discord 指定頻道
- 避免重複推播

部署：
- 雲端免費方案推薦：Railway 
- Python 套件：discord.py, feedparser, python-dotenv
- 網址 : https://railway.com/

環境變數：
- DISCORD_TOKEN
- CHANNEL_ID
- RSS_URL
- CHECK_INTERVAL (單位：秒，預設 300 秒)
- MENTION_USER_ID (測試用，推播時提及特定用戶，加入--test 參數啟用)


