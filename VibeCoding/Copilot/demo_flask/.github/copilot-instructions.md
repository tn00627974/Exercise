<!-- Copilot / AI agent 指示（demo_flask） -->

# 快速導覽

- **目的：** 小型的 Flask 範例，採用應用程式工廠（application factory）模式與 Blueprint 組織。核心程式碼位於 `app/`，測試位於 `tests/`。
- **重要檔案：** [app/__init__.py](app/__init__.py#L1-L20)（工廠函式）、[app/main.py](app/main.py#L1-L20)（啟動程式）、[app/controllers/](app/controllers/)（路由 Blueprints）、[tests/test_health.py](tests/test_health.py#L1-L20)。

# 架構與慣例

- 專案使用工廠模式：在 `app/__init__.py` 中有 `create_app()`，在建立測試用或腳本用的 app 實例時應優先使用 `create_app()`。
- 路由以 Flask Blueprint 組織，放在 `app/controllers/`。每個 controller 都會註冊一個帶有 `url_prefix` 的 `Blueprint`（參見 `health_controller.py` 與 `user_controller.py`）。控制器函式範例會回傳 `(body, status_code)` 的 tuple。
- 測試透過 `create_app()` 建立 app，並使用 `app.test_client()` 發送請求（參見 `tests/test_health.py`）。撰寫整合測試時請遵循此模式。

# 開發者如何執行

- 安裝相依套件：`pip install -r requirements.txt`（專案鎖定了 Flask 與 pytest）。
- 開發執行應用程式：

```powershell
python -m app.main
# 或
python app\main.py
```

- 伺服器預設：host 為 `0.0.0.0`、port 為 `5000`，在 `app/main.py` 中 `debug=True`。

# 執行測試與快速檢查

- 執行測試：`pytest -q`（範例測試 `tests/test_health.py` 會呼叫 `create_app()` 並使用 `test_client`）。
- 新增測試時，請透過 `create_app()` 建立全新的 app 實例，避免全域狀態干擾。

# 新增程式碼的慣例

- 新增 HTTP endpoint 時，請在 `app/controllers/` 下建立 Blueprint，並在 `create_app()` 中以 `app.register_blueprint(...)` 註冊。
- 回傳 JSON payload 並明確回傳狀態碼，採用 `(dict_or_json, int)` 的回傳格式以符合現有控制器風格。
- 讓控制器保持精簡且無副作用；商業邏輯應放在獨立模組（例如新增 `services/` 或 `models/`）以利測試。

# 除錯小提示

- 在互動式 REPL 或單元測試中使用 `create_app()`，可在不啟動伺服器下測試路由。
- 若要重現測試中失敗的請求，可在本機以相同的 `client.get(...)` 呼叫使用 `test_client()` 進行排查。

# 專案特定注意事項

- `user_controller.py` 對 GET endpoint 回傳了 201（不標準）。新增 endpoint 時，僅在有特別理由時跟隨現有回應碼，否則 GET 建議回傳 200。
- 專案目前沒有外部整合（資料庫、佇列等）；若需模擬此類整合，請以適合測試的 adapter 介面進行封裝。

# 可變更的檔案位置

- 新增或修改 Blueprint：放在 `app/controllers/*.py` 並在 `create_app()`（見 [app/__init__.py](app/__init__.py#L1-L40)）中註冊。
- 調整執行設定：修改 `app/main.py`。
- 新增測試：放在 `tests/`，並使用 `pytest`。

# 不確定時的做法

- 仿照現有控制器與測試的簡潔、明確風格；優先可讀性與小型函式設計。

---
如需補充專案資訊（CI 指令、環境變數或外部服務等），請告訴我要補哪些內容，我會更新此檔案。
