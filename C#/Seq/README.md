# 如何在 ASP.NET Core 專案中整合 Seq 記錄提供者

## 📋 概述

**Seq** 是一個功能強大的結構化日誌伺服器（Log Server），能幫助開發者輕鬆管理和查詢應用程式的日誌。本文介紹如何透過官方的 `Seq.Extensions.Logging` 套件，將 ASP.NET Core 應用程式的日誌無縫整合到 Seq 中。

### 主要優勢
- ✅ 官方支援，無需額外配置
- ✅ 與 Microsoft.Extensions.Logging 完全相容
- ✅ 結構化日誌管理
- ✅ 即時日誌查詢和分析

---

## 📌 前置要求

- Docker 已安裝
- .NET 6.0 或更高版本
- ASP.NET Core 基礎知識
- 文字編輯器或 Visual Studio

---

## 🚀 設定步驟

### 1️⃣ 啟動 Seq 伺服器（Docker）

透過 Docker 快速啟動一個 Seq 伺服器：

```bash
docker run --name seq -d --restart unless-stopped -e ACCEPT_EULA=Y -p 5341:80 datalust/seq:latest
```

**預設會監聽** `http://localhost:5341`

### 2️⃣ 建立 ASP.NET Core MVC 專案

```bash
dotnet new mvc -n mvc1
cd mvc1
```

### 3️⃣ 安裝 Seq NuGet 套件

```bash
dotnet add package Seq.Extensions.Logging
```

### 4️⃣ 配置 Seq Provider

編輯 **Program.cs** 檔案，在日誌系統中加入 Seq Provider。

#### 方式一：簡單配置（預設）

```csharp
builder.Logging.AddSeq();
```

#### 方式二：指定伺服器和 API Key

```csharp
builder.Logging.AddSeq(
    serverUrl: "http://localhost:5341", 
    apiKey: "your-api-key"
);
```

> **❓ 關於 API Key**
> - **是否必需？** 否，本地開發環境通常**不需要** API Key
> - **何時需要？** 生產環境或需要認證的 Seq 伺服器才需要設定
> - **如何取得？** 參考下方「📌 如何生成 API Key」章節

#### 方式三：從設定檔讀取（推薦）

```csharp
builder.Logging.AddSeq(builder.Configuration.GetSection("Seq"));
```

### 5️⃣ 設定 appsettings.json

在 **appsettings.json** 中加入 Seq 的配置區段：

```json
{
  "Seq": {
    "ServerUrl": "http://localhost:5341",
    "ApiKey": "1234567890",
    "MinimumLevel": "Trace",
    "LevelOverride": {
      "Microsoft": "Warning"
    }
  }
}
```

| 參數 | 說明 |
|------|------|
| `ServerUrl` | Seq 伺服器位址 |
| `ApiKey` | API 金鑰（可選） |
| `MinimumLevel` | 最低日誌等級（Trace, Debug, Information 等） |
| `LevelOverride` | 針對特定命名空間的等級覆蓋 |

---

## 📝 日誌等級說明

| 等級 | 數值 | 用途 |
|------|------|------|
| **Trace** | 0 | 最詳細的信息，通常只在開發環境中啟用 |
| **Debug** | 1 | 偵錯信息，幫助診斷問題 |
| **Information** | 2 | 一般信息，記錄應用程式的重要事件 |
| **Warning** | 3 | 警告，表示潛在的問題 |
| **Error** | 4 | 錯誤，表示發生了問題 |
| **Critical** | 5 | 嚴重錯誤，應用程式可能無法繼續運行 |

---

## 📌 如何生成 API Key

### 🤔 什麼是 API Key？

API Key 是用來驗證應用程式身份的安全令牌，確保只有授權的應用程式能向 Seq 發送日誌。

### ⚙️ 本地開發環境

**通常不需要 API Key**，因為預設的 Seq Docker 容器沒有啟用安全認證。

直接使用以下配置即可：
```csharp
builder.Logging.AddSeq("http://localhost:5341");
```

### 🔐 生產環境或需要認證的情況

#### 步驟 1：登入 Seq 管理介面

1. 開啟瀏覽器，訪問 `http://localhost:5341`
2. 點擊右上角的 **設定**（⚙️ Settings）
3. 選擇 **API 金鑰**（API Keys）

#### 步驟 2：建立新的 API Key

1. 點擊 **建立 API Key**（Create API Key）按鈕
2. 填入 **Token 名稱**（例如：`my-app-logger`）
3. 選擇 **權限**（通常選擇 `Ingest` — 用於記錄事件）
4. 點擊 **保存**

#### 步驟 3：複製並使用 API Key

1. 複製生成的 Token 值
2. 在 `appsettings.json` 中貼上：

```json
{
  "Seq": {
    "ServerUrl": "http://localhost:5341",
    "ApiKey": "paste-your-token-here",
    "MinimumLevel": "Trace"
  }
}
```

### 🛡️ 安全建議

| 項目 | 建議 |
|------|------|
| **不要在代碼中硬編碼** | 使用環境變數或 `appsettings.json` 配置 |
| **保管 API Key** | 視同密碼，不要提交到版本控制系統 |
| **定期輪換** | 生產環境建議定期更新 API Key |
| **使用環境變數** | 設定 `SeqApiKey` 環境變數，避免洩露 |

#### 使用環境變數的方式：

```csharp
builder.Logging.AddSeq(
    serverUrl: Environment.GetEnvironmentVariable("SEQ_SERVER_URL") ?? "http://localhost:5341",
    apiKey: Environment.GetEnvironmentVariable("SEQ_API_KEY")
);
```

---

## 🧪 測試日誌記錄

### 1️⃣ 修改 HomeController 類別

編輯 **Controllers/HomeController.cs**，加入各種日誌等級的紀錄：

```csharp
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using mvc1.Models;

namespace mvc1.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        // 記錄不同等級的日誌
        _logger.LogTrace("Hello, world!");
        _logger.LogDebug("Hello, world!");
        _logger.LogInformation("Hello, world!");
        _logger.LogWarning("Hello, world!");
        _logger.LogError("Hello, world!");
        _logger.LogCritical("Hello, world!");

        return View();
    }
}
```

### 2️⃣ 調整開發環境設定

編輯 **appsettings.Development.json**，設定最低日誌等級以捕獲所有日誌：

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Trace",
      "Microsoft.AspNetCore": "Trace"
    }
  }
}
```

### 3️⃣ 啟動應用程式

```bash
dotnet run
```

### 4️⃣ 驗證日誌

1. 瀏覽 `http://localhost:5000`（或應用程式的本地位址）
2. 存取首頁以觸發日誌記錄
3. 開啟 Seq 首頁 → **`http://localhost:5341`**
4. ✅ 日誌應該已成功寫入 Seq！

---

## 🔧 進階配置

### 結構化日誌記錄

建議使用結構化日誌，這樣更容易在 Seq 中搜索和過濾：

```csharp
_logger.LogInformation("User {UserId} logged in at {LoginTime}", userId, DateTime.UtcNow);
_logger.LogError(new Exception("Database error"), "Failed to fetch user {UserId}", userId);
```

### 自訂 Seq 設定

在 `Program.cs` 中使用配置委派進行更細粒度的控制：

```csharp
builder.Logging.AddSeq(opts => {
    opts.ServerUrl = "http://localhost:5341";
    opts.MinimumLevel = LogLevel.Trace;
    opts.LevelOverride = new Dictionary<string, LogEventLevel> 
    { 
        ["Microsoft"] = LogEventLevel.Warning 
    };
});
```

---

## ❓ 常見問題與故障排除

### ❌ 問題：無法連接到 Seq 伺服器

**解決方案：**
1. 確認 Docker 容器已運行：`docker ps`
2. 檢查防火牆設定，確保 5341 埠未被阻止
3. 嘗試手動訪問 `http://localhost:5341`

### ❌ 問題：日誌未出現在 Seq 中

**解決方案：**
1. 檢查 `appsettings.json` 中的 `MinimumLevel` 配置
2. 確認 `AddSeq()` 已在 `Program.cs` 中呼叫
3. 查看應用程式控制台是否有錯誤信息
4. 檢查 `appsettings.Development.json` 的日誌等級設定

### ❌ 問題：API Key 驗證失敗

**解決方案：**
1. 在 Seq 中生成新的 API Key
2. 確保 API Key 在配置中正確設定
3. 檢查是否有多餘的空格或換行符

---

## 📚 相關資源

- [Seq 官方文檔](https://docs.datalust.co/)
- [Seq.Extensions.Logging NuGet 套件](https://www.nuget.org/packages/Seq.Extensions.Logging/)
- [Microsoft.Extensions.Logging 文檔](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging)
- [Docker Seq 鏡像](https://hub.docker.com/r/datalust/seq)

---

## 📋 快速檢查清單

- [ ] Docker 已安裝並執行
- [ ] Seq 伺服器已啟動（`docker run ...`）
- [ ] 專案已建立（`dotnet new mvc -n mvc1`）
- [ ] Seq.Extensions.Logging 已安裝
- [ ] `AddSeq()` 已在 `Program.cs` 中配置
- [ ] `appsettings.json` 已更新
- [ ] `appsettings.Development.json` 的日誌等級已設定
- [ ] HomeController 已修改以記錄日誌
- [ ] 應用程式已啟動並測試
- [ ] Seq 首頁可以訪問，日誌已顯示

---

## 💡 最佳實踐

✅ **使用結構化日誌** — 記錄包含上下文的結構化數據  
✅ **合理設定日誌等級** — 避免在生產環境中記錄過多 Trace/Debug 日誌  
✅ **定期查看日誌** — 使用 Seq 的查詢和篩選功能分析日誌  
✅ **設定警報規則** — 在 Seq 中配置自動警報以監控錯誤  
✅ **使用 API Key** — 在生產環境中安全地管理 API 金鑰  

---

*最後更新：2026 年 5 月*