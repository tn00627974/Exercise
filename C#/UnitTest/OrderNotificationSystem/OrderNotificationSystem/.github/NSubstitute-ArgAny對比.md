# NSubstitute Arg.Any vs 具體物件的區別

## 你的問題

```csharp
_mockSender.Send(Arg.Any<Order>()).Returns(expectedResult);
_mockSender.Send(order).Returns(expectedResult);
```

這兩行有什麼不同？

---

## ?? 核心區別

| 寫法 | 含義 | 匹配範圍 | 使用頻率 |
|------|------|---------|---------|
| **`Arg.Any<Order>()`** | 匹配任何Order | ? 任何Order物件 | 95% |
| **`order`** | 只匹配這個Order | ? 特定物件實例 | 5% |

---

## 1?? `Arg.Any<Order>()` - 推薦

### 含義
**"無論傳入什麼Order，都返回這個結果"**

### 代碼
```csharp
_mockSender.Send(Arg.Any<Order>()).Returns(expectedResult);
```

### 實際效果
```csharp
var order1 = new Order { OrderNo = "ORD001" };
var order2 = new Order { OrderNo = "ORD002" };
var order3 = new Order { OrderNo = "ORD003" };

// 都會返回 expectedResult
_mockSender.Send(order1);  // ? 匹配
_mockSender.Send(order2);  // ? 匹配
_mockSender.Send(order3);  // ? 匹配
```

---

## 2?? `order` - 不推薦

### 含義
**"只有傳入這個特定物件實例，才返回結果"**

### 代碼
```csharp
var order = new Order { OrderNo = "ORD100" };
_mockSender.Send(order).Returns(expectedResult);
```

### 實際效果
```csharp
var order = new Order { OrderNo = "ORD100" };
var sameOrder = new Order { OrderNo = "ORD100" };

_mockSender.Send(order);     // ? 匹配（同一個物件實例）
_mockSender.Send(sameOrder); // ? 不匹配（不同的物件實例，即使內容相同）
```

---

## ?? 在你的測試中的問題

你同時寫了兩行：

```csharp
_mockSender.Send(Arg.Any<Order>()).Returns(expectedResult);  // 第 1 行
_mockSender.Send(order).Returns(expectedResult);              // 第 2 行
```

### 結果：第 2 行會覆蓋第 1 行！

```
第 1 行設置：匹配任何Order
  ↓
第 2 行設置：只匹配這個特定的order
  ↓
最終結果：只有第 2 行生效
```

### 修正後
```csharp
// ? 只設置一次
_mockSender.Send(Arg.Any<Order>()).Returns(expectedResult);

// ? 不要再寫第二行
// _mockSender.Send(order).Returns(expectedResult);
```

---

## ?? 什麼時候用哪種？

### 用 `Arg.Any<Order>()` ? 99% 的時候

```csharp
[Test]
public void Send_WhenSucceeds_PassThroughResult()
{
    // Arrange
    var order = new Order { OrderNo = "ORD100", Amount = 300 };
    var expectedResult = new NotificationResult { Success = true };
    
    // 推薦：任何Order都返回這個結果
    _mockSender.Send(Arg.Any<Order>()).Returns(expectedResult);

    // Act
    var result = _decorator.Send(order);

    // Assert
    Assert.That(result.Success, Is.True);
}
```

### 用 `order` ? 很少用（不推薦）

```csharp
// 不推薦的寫法（幾乎不用）
var order = new Order { OrderNo = "ORD100" };
_mockSender.Send(order).Returns(expectedResult);

// 如果真的需要根據Order的內容來返回不同結果，改用這個
_mockSender.Send(Arg.Is<Order>(o => o.OrderNo == "ORD100"))
    .Returns(expectedResult);
```

---

## ?? 進階用法：條件匹配

如果你需要根據Order的**內容**來返回不同的結果：

### 方法 1：按訂單號
```csharp
// 訂單號為 "ORD100" 時返回成功
_mockSender.Send(Arg.Is<Order>(o => o.OrderNo == "ORD100"))
    .Returns(successResult);

// 訂單號為 "ORD200" 時返回失敗
_mockSender.Send(Arg.Is<Order>(o => o.OrderNo == "ORD200"))
    .Returns(failureResult);
```

### 方法 2：按金額
```csharp
// 金額 > 1000 時返回成功
_mockSender.Send(Arg.Is<Order>(o => o.Amount > 1000))
    .Returns(successResult);

// 金額 <= 1000 時返回失敗
_mockSender.Send(Arg.Is<Order>(o => o.Amount <= 1000))
    .Returns(failureResult);
```

---

## 完整的最佳實踐例子

```csharp
[Test]
public void Send_WhenSucceeds_PassThroughResult()
{
    // Arrange
    var order = new Order { OrderNo = "ORD100", Amount = 300 };
    var expectedResult = new NotificationResult { Success = true };
    
    // ? 只設置一次，使用 Arg.Any
    _mockSender.Send(Arg.Any<Order>()).Returns(expectedResult);

    // Act
    var result = _decorator.Send(order);

    // Assert
    Assert.That(result.Success, Is.True);
    _mockSender.Received(1).Send(order);
}

[Test]
public void Send_WhenFails_PassFailureThrough()
{
    // Arrange
    var order = new Order { OrderNo = "ORD101", Amount = 150 };
    var failureResult = new NotificationResult { Success = false };
    
    // ? 也是 Arg.Any，返回不同的結果
    _mockSender.Send(Arg.Any<Order>()).Returns(failureResult);

    // Act
    var result = _decorator.Send(order);

    // Assert
    Assert.That(result.Success, Is.False);
}

[Test]
public void Send_WithDifferentOrders_BothReturnSuccess()
{
    // Arrange
    var order1 = new Order { OrderNo = "ORD100", Amount = 300 };
    var order2 = new Order { OrderNo = "ORD200", Amount = 500 };
    var successResult = new NotificationResult { Success = true };
    
    // ? 一個設置，兩個不同的Order都匹配
    _mockSender.Send(Arg.Any<Order>()).Returns(successResult);

    // Act
    var result1 = _decorator.Send(order1);
    var result2 = _decorator.Send(order2);

    // Assert
    Assert.That(result1.Success, Is.True);
    Assert.That(result2.Success, Is.True);
    _mockSender.Received(2).Send(Arg.Any<Order>());
}
```

---

## 總結

| 場景 | 用什麼 |
|------|--------|
| 大多數測試 | `Arg.Any<Order>()` |
| 測試多個Order | `Arg.Any<Order>()` |
| 根據Order內容返回不同結果 | `Arg.Is<Order>(o => ...)` |
| 只匹配特定物件實例 | `order`（幾乎不用） |

---

**記住：99% 的時候都用 `Arg.Any<T>()`** ?
