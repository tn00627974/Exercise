# C# WinForm + Seq Server 練習指南

## 前置需求
- .NET 6.0 或更新版本
- Seq Server 本地運行 (預設: http://localhost:5341)

## 安裝步驟

### 1. 安裝 Seq Server
```bash
# 使用 Docker 快速安裝 (推薦)
docker run -d -e ACCEPT_EULA=Y -p 5341:80 datalust/seq:latest

# 或下載安裝檔: https://datalust.io/download
```

### 2. 編譯和運行應用
```bash
cd D:\工程師資料夾\Exercise\Seq
dotnet restore
dotnet build
dotnet run
```

### 3. 訪問 Seq Dashboard
打開瀏覽器訪問: http://localhost:5341

## 功能說明

### WinForm 應用功能
- **輸入日誌訊息**: 在文本框輸入要發送的日誌
- **選擇日誌等級**: Information, Warning, Error, Debug
- **發送日誌**: 將日誌發送到 Seq Server
- **自動測試**: 自動發送多條測試日誌
- **即時顯示**: 在應用中顯示已發送的日誌

### Seq Dashboard 檢視
在 Seq Server 中查看:
- 實時日誌流
- 日誌等級過濾
- 自訂屬性搜索
- 日誌統計分析

## 程式碼結構

```
├── Program.cs              # 主程式進入點 + Serilog 配置
├── MainForm.cs            # WinForm UI 和邏輯
└── SeqWinFormApp.csproj   # 專案設定
```

## 自訂配置

### 修改 Seq Server 地址
在 `Program.cs` 中修改:
```csharp
.WriteTo.Seq("http://your-seq-server:5341")
```

### 添加更多日誌屬性
```csharp
using (LogContext.PushProperty("UserId", 123))
using (LogContext.PushProperty("SessionId", Guid.NewGuid()))
{
    Log.Information("用戶操作");
}
```

## 常見問題

**Q: 連接不到 Seq Server？**
A: 確認 Seq Server 正在運行，檢查防火牆和地址配置

**Q: 如何保存日誌到文件？**
A: 在 Program.cs 中添加 `.WriteTo.File()`

**Q: 如何設定日誌保留期限？**
A: 在 Seq Server Dashboard 中設定數據保留政策
