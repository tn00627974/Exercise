# Arg.Any vs 具體物件 - 終極簡單說明

## 用最簡單的例子

假設你有一個朋友，你告訴他：

### 方式 1：`Arg.Any<Order>()`
```
你說：「只要有人給我Order，我就給他一個成功的結果」
朋友A 給你Order → 得到成功
朋友B 給你Order → 得到成功  
朋友C 給你Order → 得到成功
```

### 方式 2：`order` 具體物件
```
你說：「只有拿著這個特定的Order，我才給他成功的結果」
    ↑ 指著某一張紙條

朋友A 拿著一張紙條 → 成功
朋友A 拿著另一張一樣的紙條 → 失敗（不是同一張紙條！）
```

---

## 程式碼角度看

### 方式 1：`Arg.Any<Order>()` - 我不關心你給什麼

```csharp
// Mock 設定：無論什麼Order都返回成功
_mockSender.Send(Arg.Any<Order>()).Returns(successResult);

// 呼叫 1
var order1 = new Order { OrderNo = "ORD001" };
_mockSender.Send(order1);  // ? 匹配！返回 successResult

// 呼叫 2
var order2 = new Order { OrderNo = "ORD002" };
_mockSender.Send(order2);  // ? 匹配！返回 successResult

// 呼叫 3
var order3 = new Order { OrderNo = "ORD003" };
_mockSender.Send(order3);  // ? 匹配！返回 successResult

// Mock 完全不在乎 order 是什麼內容，只要是 Order 型別就行
```

### 方式 2：`order` - 我只認這一個物件實例

```csharp
// 事先準備一個 order
var order = new Order { OrderNo = "ORD001" };

// Mock 設定：只有這個特定的 order 才返回成功
_mockSender.Send(order).Returns(successResult);

// 呼叫 1 - 用同一個 order
_mockSender.Send(order);  // ? 匹配！返回 successResult

// 呼叫 2 - 建立一個內容相同但不同實例的 order
var sameOrder = new Order { OrderNo = "ORD001" };  // 內容完全相同！
_mockSender.Send(sameOrder);  // ? 不匹配！（因為是不同的物件實例）

// 呼叫 3 - 建立另一個 order
var differentOrder = new Order { OrderNo = "ORD002" };
_mockSender.Send(differentOrder);  // ? 不匹配！
```

---

## 用類比來理解

### `Arg.Any<Order>()` - 停車場保安
```
保安：「誰來都讓進」
A 車來 → 放行
B 車來 → 放行
C 車來 → 放行
（不管是什麼車，只要是車就行）
```

### `order` 具體物件 - VIP 停車位
```
停車位：「只有這台車（編號：ABC123）才能停」
你的車（編號：ABC123）來 → 可以停
別人的同型號車（編號：XYZ456）來 → 不能停
（即使是同一個型號，也不行）
```

---

## 實際在你的測試裡發生了什麼

你的原始代碼：
```csharp
_mockSender.Send(Arg.Any<Order>()).Returns(expectedResult);  // 設定 1
_mockSender.Send(order).Returns(expectedResult);              // 設定 2
```

### 執行過程：

```
Step 1: 執行第 1 行
  設定：任何 Order 都返回 expectedResult
  Mock 記錄：「好的，我會對任何 Order 返回 expectedResult」

Step 2: 執行第 2 行
  設定：只有這個特定的 order 返回 expectedResult
  Mock 記錄：「等等，新的設定來了！我要覆蓋之前的設定」
           「現在只有這個特定的 order 才返回 expectedResult」

Step 3: 測試執行 _mockSender.Send(order)
  Mock 查詢：「是這個 order 嗎？」
           「是！」
           「返回 expectedResult」
```

**結果：第 2 行把第 1 行覆蓋了！**

---

## 實際測試對比

### ? 錯誤寫法（兩行設定）

```csharp
[Test]
public void Send_Test()
{
    var order = new Order { OrderNo = "ORD001" };
    var expectedResult = new NotificationResult { Success = true };
    
    // 設定 1：任何 Order 都返回成功
    _mockSender.Send(Arg.Any<Order>()).Returns(expectedResult);
    
    // 設定 2：只有這個 order 返回成功（覆蓋設定 1！）
    _mockSender.Send(order).Returns(expectedResult);
    
    // 測試
    _mockSender.Send(order);  // 返回 expectedResult（因為匹配設定 2）
}
```

### ? 正確寫法（只有一行設定）

```csharp
[Test]
public void Send_Test()
{
    var order = new Order { OrderNo = "ORD001" };
    var expectedResult = new NotificationResult { Success = true };
    
    // 只有一個設定：任何 Order 都返回成功
    _mockSender.Send(Arg.Any<Order>()).Returns(expectedResult);
    
    // 測試
    _mockSender.Send(order);  // 返回 expectedResult（因為匹配設定）
}
```

---

## 物件實例 vs 物件內容的區別

C# 中物件的相等性：

```csharp
var order1 = new Order { OrderNo = "ORD001" };
var order2 = new Order { OrderNo = "ORD001" };

// 內容相同，但是不同的物件實例
order1 == order2        // ? false
object.ReferenceEquals(order1, order2)  // ? false（不是同一個物件）

// 同一個物件實例
order1 == order1        // ? true
object.ReferenceEquals(order1, order1)  // ? true（同一個物件）
```

**Mock 框架預設用 `==` 或 `ReferenceEquals` 比較**

所以：
- `Arg.Any<Order>()` 不做比較，接受一切
- `order` 做 `==` 比較，只有同一個實例才符合

---

## 最終總結

| 寫法 | 檢查什麼 | 結果 |
|------|---------|------|
| `Arg.Any<Order>()` | 型別 | 任何 Order 都符合 ? |
| `order` | 物件實例 | 只有這個特定實例符合 ? |

**在 99% 的測試中，你只需要 `Arg.Any<Order>()`**

---

## 現在你的測試

已經修正為：

```csharp
_mockSender.Send(Arg.Any<Order>()).Returns(expectedResult);
// 刪除了第二行，只保留這一行
```

這樣就對了！?

---

## 想要更複雜的匹配？

如果你想根據 Order 的**內容**來返回不同結果：

```csharp
// 訂單號是 ORD001 時返回成功
_mockSender.Send(Arg.Is<Order>(o => o.OrderNo == "ORD001"))
    .Returns(successResult);

// 訂單號是 ORD002 時返回失敗
_mockSender.Send(Arg.Is<Order>(o => o.OrderNo == "ORD002"))
    .Returns(failureResult);
```

但這是進階用法，你現在不需要。

---

**記住：`Arg.Any<T>()` = 接受任何東西，只看型別** ?
