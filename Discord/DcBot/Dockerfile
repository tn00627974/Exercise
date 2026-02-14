# 使用官方 Python 3.12 精簡版映像
FROM python:3.12-slim

# 設定工作目錄
WORKDIR /app

# 先複製依賴檔案並安裝（利用 Docker 快取層）
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# 複製專案檔案
COPY bot.py .

# 設定啟動命令
CMD ["python", "bot.py"]
