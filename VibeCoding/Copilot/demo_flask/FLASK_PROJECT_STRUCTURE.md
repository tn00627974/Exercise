# 小型 Flask 專案建議結構

下面是為小型專案所建議的資料夾與檔案結構，以及各項目的簡短說明，方便當作專案模板或 README 補充說明。

```
project_root/
├─ app/                       # 主程式碼包（package）
│  ├─ __init__.py             # create_app() factory，或匯出藍圖
│  ├─ main.py                 # 可直接啟動的 runner（或使用 python -m app.main）
│  ├─ controllers/            # 路由藍圖（例如 health_controller.py, user_controller.py）
│  ├─ models/                 # 資料模型（若有）
│  ├─ templates/              # Jinja2 模板（HTML）
│  └─ static/                 # 靜態檔案：CSS/JS/Images
├─ tests/                     # 單元測試與整合測試（例如 test_health.py）
├─ requirements.txt 或 pyproject.toml
├─ config.py 或 instance/config.py
├─ .env                       # 開發用環境變數（勿放敏感資料）
├─ migrations/ (選用)         # DB migration（Flask-Migrate）
├─ README.md
└─ FLASK_PROJECT_STRUCTURE.md  #（此檔）
```

要點說明：
- app/__init__.py 建議採用 factory pattern（提供 create_app），讓測試與不同環境更容易切換。
- 使用 Blueprint 分組路由（controllers 或 blueprints 資料夾），保持單一檔案責任小。
- 把設定抽成 config.py 或從環境讀取，避免在程式碼中寫死環境設定。
- tests/ 放自動化測試，並在 CI 中執行。

快速啟動（開發）：
1. 建立虛擬環境並啟用：
   python -m venv venv
   venv\Scripts\activate
2. 安裝相依：
   pip install -r requirements.txt
3. 啟動應用（開發）：
   python -m app.main
   或
   python app\main.py

範例檔案重點：
- app/__init__.py: 提供 create_app()，負責註冊藍圖與設定。
- app/main.py: 只在直接執行時啟動 server（if __name__ == '__main__'）。

此結構針對小型專案，若專案成長可再分出 services/, schemas/, adapters/ 等模組以維持可維護性。
