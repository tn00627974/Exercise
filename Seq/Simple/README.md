# 簡單日誌使用指南

## 快速開始

### 1. 引用日誌類
```csharp
using SimpleLogger;

// 為你的專案建立日誌器
ProjectLogger logger = new ProjectLogger("MyProject");
```

### 2. 記錄日誌
```csharp
// 資訊日誌
logger.Info("用戶登入", service: "AuthService", module: "Login");

// 警告日誌
logger.Warning("異常值", service: "OrderService", module: "Validation");

// 錯誤日誌
try { /* 代碼 */ }
catch (Exception ex)
{
    logger.Error(ex, "操作失敗", service: "PaymentService", module: "Error");
}

// 調試日誌
logger.Debug("變數值", service: "Service", module: "Debug");
```

## 在 Seq Dashboard 查詢

```sql
-- 查詢特定專案
Project = "UserProject"

-- 查詢特定服務
Service = "AuthService"

-- 組合查詢
Project = "OrderProject" AND Level = "Error"
```

## 代碼統計
- ProjectLogger.cs: 65 行 (核心類)
- SimpleLogForm.cs: 130 行 (示例)
- 共計: 195 行 (遠低於 250 行限制)

## 特點
✅ 簡單易用
✅ 按日期分割日誌檔案
✅ 發送到 Seq Server
✅ 支援 Info/Warning/Error/Debug
✅ 完全可重複使用
