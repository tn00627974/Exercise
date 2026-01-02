# Go 新手學習（語言 + 框架）路線圖

> 目標：能獨立開發、測試、部署一個可維護的 Go Web/API 服務，並逐步擴展到資料庫、快取、佇列與微服務。

---

## 0) 環境與工具（0.5～1 天）
- 安裝：Go（建議最新穩定版）、VS Code、Git
- 熟悉指令：
  - `go version`、`go env`
  - `go mod init`、`go mod tidy`
  - `go run`、`go build`
- 專案結構概念：Module、Package、Workspace（多模組可先略）

**里程碑**：能建立一個 module、跑起來 `Hello World`，並用 Git 管理。

---

## 1) Go 語言基礎（3～7 天）
**必學語法與概念**
- 變數/常數、型別（數值、字串、bool）
- 流程控制：`if`、`switch`、`for`
- 函式：多回傳值、具名回傳、defer
- 資料結構：array/slice/map、struct、tag（為 JSON 做準備）
- 指標（理解即可，不必鑽太深）
- 介面（interface）與方法接收者（value vs pointer receiver）
- 錯誤處理：`error`、`fmt.Errorf`、`errors.Is/As`
- 常用標準庫：`time`、`strings`、`strconv`、`encoding/json`

**里程碑**：完成 5～10 題小練習（字串處理、排序、簡易 CRUD in-memory）。

---

## 2) 測試與品質（1～3 天）
- 單元測試：`testing`、table-driven tests
- 斷言風格（可用 `testify`，但先會原生）
- 基準測試（選修）：`go test -bench`
- 靜態檢查：
  - `go test ./...`
  - `go vet ./...`
  - `golangci-lint`（建議導入）

**範例**：`examples/go_basics/main_test.go`（table-driven/subtests/benchmark/example）

**里程碑**：為核心函式寫出可讀且可維護的單元測試。

---

## 3) 併發與 Context（3～7 天）
- goroutine、channel、select
- 常見同步：`sync.Mutex`、`sync.WaitGroup`、`sync.Once`
- `context.Context`：超時、取消、request-scoped value
- 常見陷阱：race condition、goroutine leak

**里程碑**：做一個「同時呼叫多個 API/任務，整合結果並支援 timeout」的小專案。

---

## 4) Web 基礎：先會標準庫 net/http（2～5 天）
- `http.Handler`、middleware 概念
- routing（標準庫較陽春，先理解再換框架）
- JSON API：request/response、status code、error response 格式
- logging、panic recovery（框架通常內建但要理解原理）

**里程碑**：用 `net/http` 寫出一個 CRUD API（in-memory），含基本 middleware（log、recover）。

---

## 5) 選一個框架（建議只選一個主修）
### 推薦（依常見度/上手度）
- **Gin**：生態成熟、範例多、效能好（主流）
- **Echo**：API 直覺、middleware 完整
- **Fiber**：類 Express 風格（注意：基於 fasthttp，與 net/http 生態有些差異）

**框架必學**
- 路由/路由群組、middleware
- binding/validation（表單、JSON）
- 統一錯誤處理（error handler）
- 設定管理（env + config）
- 結構化 logging（建議 `zap` 或 `zerolog`）

**里程碑**：把第 4 階段 CRUD API 搬到框架，並加入 validation 與統一錯誤格式。

---

## 6) 資料庫（SQL 優先）（5～14 天）
- SQL 基礎：schema、index、transaction、isolation（理解即可）
- Go 連線：`database/sql` + driver（PostgreSQL 或 MySQL）
- 資料存取層選擇其一：
  - **sqlc**（偏推薦）：SQL 寫得清楚、型別安全
  - **GORM**：開發快，但要注意效能與 query 可控性
  - **sqlx**：介於兩者
- Migration：`golang-migrate`（常用）

**里程碑**：把 CRUD 改成 DB 版本，支援 transaction、分页、基本查詢與索引。

---

## 7) 專案架構與可維護性（3～10 天）
- 分層/模組：handler/controller、service、repository、domain
- 依賴注入（手動即可，避免過度複雜）
- 設定與 secrets：環境變數、不要把密碼進 repo
- OpenAPI/Swagger：
  - Gin 常用：swaggo（或用 API-first 工具）

**里程碑**：專案結構清晰、測試容易、能產生 API 文件。

---

## 8) 快取、佇列與背景工作（選修但實務常用）（3～10 天）
- Redis：cache、rate limit、distributed lock（先理解再用）
- 背景工作：
  - 簡單：goroutine + channel + worker pool
  - 進階：RabbitMQ/Kafka（先了解概念）
- 排程：cron（例如 `robfig/cron`）

**里程碑**：加上 Redis 快取與一個背景任務（例如寄信/縮圖/同步）。

---

## 9) 可觀測性與營運（3～10 天）
- Logging：request id、trace id
- Metrics：Prometheus client
- Tracing：OpenTelemetry
- Health checks：`/healthz`、`/readyz`

**里程碑**：能在本機用 Prometheus/Grafana 看到基本指標，並能追蹤 request。

---

## 10) 部署（Docker 優先）（2～7 天）
- Dockerfile（multi-stage build）
- 設定：12-factor（env config）
- 上線環境（擇一）：
  - VM（Nginx + systemd）
  - Docker Compose
  - Kubernetes（進階）
- CI/CD（GitHub Actions）：build、test、lint、docker build/push

**里程碑**：一鍵（CI）跑測試、產物打包、部署到測試環境。

---

## 11) 進階方向（按需求選）
- gRPC + protobuf（服務間通訊）
- 微服務模式：API gateway、service discovery（概念為主）
- DDD（大型專案）
- 效能調校：pprof、benchmark、逃逸分析
- 安全：JWT/OAuth2、CORS、CSRF（依場景）、輸入驗證、依賴漏洞掃描

---

# 建議的「第一個實戰專案」清單（由易到難）
1. **Todo REST API**（in-memory → DB）
2. **會員系統**：註冊/登入（JWT）、權限（RBAC）
3. **短網址**：Redis 快取 + rate limit
4. **訂單系統雛形**：transaction + background job
5. **gRPC 服務**：搭配 REST gateway（選修）

---

# 學習節奏建議（範例）
- 第 1 週：語言基礎 + 測試
- 第 2 週：併發 + net/http
- 第 3 週：Gin/Echo + 專案架構
- 第 4 週：資料庫 + migration + Docker 部署

---