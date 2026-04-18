# 工作流程參考

## 快速檢查

### 本機環境
1. 建立或啟用虛擬環境。
2. 從 [requirements.txt](../../../../requirements.txt) 安裝套件。
3. 確認 `.env` 檔案包含以下變數：
   - `DISCORD_TOKEN`
   - `SUBSCRIPTIONS_FILE=subscriptions.json`

### 常用指令

```powershell
python bot.py --test "Bot test message"
python bot.py --test-yt
python test_rss.py
python test_subscriptions.py
```

## 重要檔案
- 主程式邏輯： [bot.py](../../../../bot.py)
- 部署設定： [Dockerfile](../../../../Dockerfile)、[docker-compose.yml](../../../../docker-compose.yml)、[render.yaml](../../../../render.yaml)
- 專案說明： [README.md](../../../../README.md)

## 疑難排解建議
- 若 Discord 傳送失敗，請確認機器人已加入伺服器並具有傳送訊息的權限。
- 若訂閱的 feed 為空，請檢查 feed 的 URL 與 user-agent 行為是否被封鎖。
- 若部署在 Render，請確認健康檢查伺服器仍監聽 `PORT` 環境變數指定的埠號。
- 若使用 Railway，請在每次部署後檢視服務日誌尋找錯誤資訊。
