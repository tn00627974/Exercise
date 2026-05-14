# 多專案日誌預覽完整指南

## 架構設計

### 日誌層級結構
```
Enterprise
├── Project (ECommerceApp, BlogApp, AnalyticsApp)
│   ├── Service (UserService, OrderService, PaymentService)
│   │   ├── Module (Authentication, OrderCreation, Payment)
│   │   │   └── Action (Login, CreateOrder, ProcessPayment)
```

## 在 Seq Server 中按專案查詢

### 查詢語法示例

```sql
-- 查詢特定專案的所有日誌
Project = "ECommerceApp"

-- 查詢特定專案的特定服務
Project = "ECommerceApp" AND Service = "UserService"

-- 查詢特定專案的錯誤
Project = "ECommerceApp" AND Level = "Error"

-- 跨專案查詢特定模組
Module = "Authentication" AND (Project = "ECommerceApp" OR Project = "BlogApp")

-- 追蹤關聯操作
CorrelationId = "123-456" AND Project = "ECommerceApp"

-- 查詢最近 1 小時的警告和錯誤
Project = "ECommerceApp" AND (Level = "Warning" OR Level = "Error") AND @Timestamp > now() - 1h
```

## 日誌配置結構

### 每個專案的日誌配置
```csharp
LogContext.PushProperty("Project", "ECommerceApp");
LogContext.PushProperty("Service", "UserService");
LogContext.PushProperty("Environment", "Production");
LogContext.PushProperty("Version", "1.0.0");
```

## 目錄結構（推薦）

```
/src
├── ECommerceApp/
│   ├── Services/
│   │   ├── UserService.cs
│   │   ├── OrderService.cs
│   │   └── PaymentService.cs
│   ├── Logger/
│   │   └── ECommerceLogger.cs
│   └── appsettings.json
├── BlogApp/
│   ├── Services/
│   └── Logger/
└── AnalyticsApp/
    ├── Services/
    └── Logger/
```

## 日誌命名規範

### 專案命名
- `ECommerceApp` - 電商應用
- `BlogApp` - 博客應用
- `AnalyticsApp` - 分析應用

### 服務命名
- `UserService` - 用戶服務
- `OrderService` - 訂單服務
- `PaymentService` - 支付服務
- `AuthService` - 身份驗證服務
- `NotificationService` - 通知服務

### 模組命名
- `Authentication` - 身份驗證
- `Authorization` - 授權
- `Database` - 數據庫操作
- `Cache` - 緩存操作
- `ExternalAPI` - 外部 API 呼叫

## 在 Seq Dashboard 中建立檢視

### 創建保存的查詢

1. **ECommerceApp 錯誤監控**
   ```sql
   Project = "ECommerceApp" AND Level = "Error"
   ```

2. **所有專案效能追蹤**
   ```sql
   @Timestamp > now() - 1h
   ```

3. **跨專案支付操作**
   ```sql
   Service = "PaymentService" OR Service = "OrderService"
   ```

## WinForm 日誌預覽器功能

### 篩選條件
- ✅ 按專案篩選
- ✅ 按服務篩選
- ✅ 按日誌等級篩選
- ✅ 關鍵字搜尋

### 顯示信息
- 📊 日誌統計 (總數、資訊、警告、錯誤)
- 📅 時間戳
- 🔗 關聯 ID（用於追蹤操作流程）
- 📝 完整訊息和例外

### 交互功能
- 🔄 實時重新載入
- 🖱️ 雙擊查看詳情
- 🎨 按等級著色（綠=資訊、黃=警告、紅=錯誤）

## 使用場景

### 場景 1: 監控特定專案
```
篩選: Project = ECommerceApp
→ 立即看到該專案的所有日誌
```

### 場景 2: 追蹤用戶操作
```
篩選: CorrelationId = "xxx-xxx-xxx"
→ 看到該用戶在所有專案中的操作流程
```

### 場景 3: 故障排查
```
篩選: Project = "BlogApp" AND Level = "Error"
→ 找到 BlogApp 的所有錯誤日誌
→ 雙擊查看完整異常堆疊
```

## 最佳實踐

1. **專案獨立性** - 每個專案使用唯一的 Project 屬性
2. **服務清晰** - 明確標註所屬服務
3. **關聯追蹤** - 使用 CorrelationId 追蹤業務流程
4. **等級適當** - 正確分配日誌等級
5. **敏感信息** - 避免記錄密碼、令牌、個人隱私
6. **本地備份** - 同時保存到本地檔案作為備份
7. **定期清理** - 在 Seq 中設定日誌保留政策
