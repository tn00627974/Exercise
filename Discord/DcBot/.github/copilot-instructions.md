# 台股 RSS Discord Bot (BOT2l4)

功能：
- 每 5 分鐘抓一次 Yahoo 台股 RSS
- 推播新文章到 Discord 指定頻道
- 避免重複推播

部署：
- 雲端免費方案推薦：Railway , Render 
- Python 套件：discord.py, feedparser, python-dotenv
- Railway 網址 : https://railway.com/ 

    備註 : 20201208 開始 Railway 宣布免費方案將不再提供免費運行時間，改為每月付費 $5 的免費額度，適合需要短期運行的專案，但可能會有資源限制。
- Render 網址 : https://render.com/

    備註 : Render 的 Web Service 免費方案提供運行，但需要HTTP 端口（PORT 環境變數），適合需要長期運行的專案，但可能會有資源限制。
- Oracle Cloud 免費方案 : https://www.oracle.com/cloud/free/

    可參考 [Oracle Cloud 佈署.md](../Oracle%20Cloud%20佈署.md) 部署指南。
    備註 : Oracle Cloud 的 Always Free 方案提供運行，但需要設定虛擬機器，適合需要完全控制環境的專案，但部署較複雜。

環境變數：
- DISCORD_TOKEN
- SUBSCRIPTIONS_FILE=subscriptions.json


