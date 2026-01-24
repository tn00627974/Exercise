# 第 6 關：裝飾器模式

## 快速開始

### 需要完成的 6 個檔案

#### 核心實現（3 個）
- `OrderNotificationSystem/Decorators/LoggingNotificationSenderDecorator.cs`
- `OrderNotificationSystem/Decorators/RetryNotificationSenderDecorator.cs`
- `OrderNotificationSystem/Decorators/TimeoutNotificationSenderDecorator.cs`

#### 單元測試（3 個）
- `OrderNotificationSystemTests/Unit/Decorators/LoggingDecoratorTests.cs`
- `OrderNotificationSystemTests/Unit/Decorators/RetryDecoratorTests.cs`
- `OrderNotificationSystemTests/Unit/Decorators/TimeoutDecoratorTests.cs`

---

## 建立步驟

### Step 1: 建立資料夾
```bash
mkdir OrderNotificationSystem\Decorators
mkdir OrderNotificationSystemTests\Unit\Decorators
```

### Step 2: 複製代碼
參考「第6關完整代碼.md」中的完整代碼

### Step 3: 執行測試
```bash
dotnet test
```

預期結果：
```
Total: 10
Passed: 10 ?
Failed: 0
```

---

## 文檔導航

| 文檔 | 用途 |
|------|------|
| **第6關快速參考.md** | 5分鐘快速理解 |
| **第6關說明.md** | 詳細學習和講解 |
| **第6關完整代碼.md** | 直接複製的完整代碼 |
| **課程進度.md** | 整個課程的進度管理 |
| **練習專案主題.md** | 完整的課程指南 |

---

## 核心概念

### 裝飾器模式是什麼？
不修改原有代碼，動態為對象添加新功能

### 為什麼需要它？
```csharp
// 不好：代碼膨脹
public class EmailNotificationSender
{
    // 原有邏輯 + 日誌 + 重試 + 超時 = 200行混亂代碼
}

// 好：職責清晰
var sender = new EmailNotificationSender();
var withLog = new LoggingNotificationSenderDecorator(sender);
var withRetry = new RetryNotificationSenderDecorator(withLog);
var withTimeout = new TimeoutNotificationSenderDecorator(withRetry);
```

---

## 3個裝飾器

### 1. LoggingNotificationSenderDecorator
- 功能：記錄發送過程
- 職責：Console.WriteLine()
- 複雜度：簡單

### 2. RetryNotificationSenderDecorator
- 功能：失敗自動重試
- 職責：重試邏輯和延遲
- 複雜度：中等

### 3. TimeoutNotificationSenderDecorator
- 功能：設置執行超時
- 職責：Task.Run和Wait
- 複雜度：中等

---

## 常見錯誤

? 忘記 null check  
? 忘記呼叫 _innerSender.Send()  
? 命名空間不正確  

查看「第6關快速參考.md」了解詳細內容

---

## 檢查清單

- [ ] 建立 Decorators 資料夾
- [ ] 複製 3 個裝飾器檔案
- [ ] 建立 Unit/Decorators 測試資料夾
- [ ] 複製 3 個測試檔案
- [ ] 執行 dotnet test
- [ ] 確保 10 個測試全部通過

---

**完成後說「第 6 關完成」！** ??
