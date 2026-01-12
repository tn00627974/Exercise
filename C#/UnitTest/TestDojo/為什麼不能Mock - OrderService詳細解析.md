# 為什麼 OrderService 不能 Mock？詳細解析

## ?? 核心問題

```csharp
public class OrderService
{
    private PaymentProcessor paymentProcessor;
    private ShippingService shippingService;

    public OrderService()
    {
        // ? 硬編碼依賴
        paymentProcessor = new PaymentProcessor();
        shippingService = new ShippingService();
    }

    public bool PlaceOrder(string orderId, decimal amount)
    {
        if (!paymentProcessor.Charge(orderId, amount))
            return false;

        return shippingService.Schedule(orderId);
    }
}
```

## ?? 為什麼不能 Mock？

### 原因 1??：沒有介面
```csharp
// ? 無法寫成這樣
var mockPayment = Substitute.For<PaymentProcessor>();
// 因為 Substitute.For<> 只能用在介面上

// ? 介面根本不存在
// 沒有 IPaymentProcessor
// 沒有 IShippingService
```

### 原因 2??：沒有建構函式參數
```csharp
public OrderService()  // ← 沒有參數！
{
    // 不管怎樣都會建立真實物件
    paymentProcessor = new PaymentProcessor();
    shippingService = new ShippingService();
}

// 想測試的話
var service = new OrderService();  // ← 只能這樣呼叫
// 無法傳入 Mock！
```

### 原因 3??：依賴硬編碼在類別內部
```csharp
// 即使你想傳入 Mock，也沒地方傳
var mockPayment = Substitute.For<IPaymentProcessor>();
var service = new OrderService(mockPayment);  // ? 編譯錯誤！
// OrderService 沒有接受參數的建構函式
```

---

## ? 如何修復？改成有介面的設計

### 第一步：建立介面

```csharp
// 定義介面
public interface IPaymentProcessor
{
    bool Charge(string orderId, decimal amount);
}

public interface IShippingService
{
    bool Schedule(string orderId);
}
```

### 第二步：修改 OrderService

```csharp
public class GoodOrderService
{
    private readonly IPaymentProcessor _paymentProcessor;
    private readonly IShippingService _shippingService;

    // ? 透過建構函式接受介面
    public GoodOrderService(
        IPaymentProcessor paymentProcessor,
        IShippingService shippingService)
    {
        _paymentProcessor = paymentProcessor;
        _shippingService = shippingService;
    }

    public bool PlaceOrder(string orderId, decimal amount)
    {
        if (!_paymentProcessor.Charge(orderId, amount))
            return false;

        return _shippingService.Schedule(orderId);
    }
}
```

### 第三步：現在可以 Mock 了！

```csharp
[Test]
public void PlaceOrder_PaymentFails_ReturnsFalse()
{
    // ? 現在可以建立 Mock
    var paymentMock = Substitute.For<IPaymentProcessor>();
    var shippingMock = Substitute.For<IShippingService>();

    // ? 配置 Mock 行為
    paymentMock.Charge(Arg.Any<string>(), Arg.Any<decimal>()).Returns(false);

    // ? 傳入 Mock
    var service = new GoodOrderService(paymentMock, shippingMock);

    // Act
    var result = service.PlaceOrder("ORDER001", 100m);

    // Assert
    Assert.IsFalse(result);
}
```

---

## ?? 對比：改前改後

### ? 改前（不能 Mock）

```csharp
public class OrderService
{
    private PaymentProcessor paymentProcessor;
    private ShippingService shippingService;

    public OrderService()  // ← 沒有參數
    {
        paymentProcessor = new PaymentProcessor();  // ← 硬編碼
        shippingService = new ShippingService();    // ← 硬編碼
    }

    public bool PlaceOrder(string orderId, decimal amount)
    {
        if (!paymentProcessor.Charge(orderId, amount))
            return false;
        return shippingService.Schedule(orderId);
    }
}

// 測試時
var service = new OrderService();  // 無法傳入 Mock
// ? 會執行真實的 PaymentProcessor
// ? 會執行真實的 ShippingService
// ? 會真的扣款和寄貨！
```

### ? 改後（可以 Mock）

```csharp
public interface IPaymentProcessor
{
    bool Charge(string orderId, decimal amount);
}

public interface IShippingService
{
    bool Schedule(string orderId);
}

public class OrderService
{
    private readonly IPaymentProcessor _paymentProcessor;      // ← 依賴介面
    private readonly IShippingService _shippingService;        // ← 依賴介面

    public OrderService(                                        // ← 有參數
        IPaymentProcessor paymentProcessor,
        IShippingService shippingService)
    {
        _paymentProcessor = paymentProcessor;
        _shippingService = shippingService;
    }

    public bool PlaceOrder(string orderId, decimal amount)
    {
        if (!_paymentProcessor.Charge(orderId, amount))
            return false;
        return _shippingService.Schedule(orderId);
    }
}

// 測試時
var paymentMock = Substitute.For<IPaymentProcessor>();
var shippingMock = Substitute.For<IShippingService>();
var service = new OrderService(paymentMock, shippingMock);  // ? 傳入 Mock

// ? 不會執行真實的 PaymentProcessor
// ? 不會執行真實的 ShippingService
// ? 不會真的扣款和寄貨
```

---

## ?? 三個關鍵要素

要讓一個類別能被 Mock 測試，需要三個要素：

| 要素 | 必要條件 | 為什麼 |
|------|---------|--------|
| **1. 介面** | 依賴必須是介面 | Mock 是基於介面建立的 |
| **2. 建構函式參數** | 透過建構函式接受依賴 | 這樣才能傳入 Mock |
| **3. 不硬編碼** | 不在類別內部建立依賴 | 否則無法替換 |

### 檢查清單

```csharp
public class MyService
{
    // ? 要素 1：依賴是介面
    private readonly IMyDependency _dependency;

    // ? 要素 2：建構函式有參數
    public MyService(IMyDependency dependency)
    {
        // ? 要素 3：直接接收，不硬編碼
        _dependency = dependency;
    }
}

// 現在可以 Mock
var mock = Substitute.For<IMyDependency>();
var service = new MyService(mock);  // ? 成功傳入 Mock
```

---

## ?? 逐步診斷

### OrderService 缺少什麼？

```csharp
public class OrderService
{
    // ? 缺少 1??：沒有介面
    // 應該是：private readonly IPaymentProcessor _paymentProcessor;
    private PaymentProcessor paymentProcessor;

    // ? 缺少 2??：沒有建構函式參數
    public OrderService()
    // 應該是：public OrderService(IPaymentProcessor processor, ...)
    {
        // ? 缺少 3??：硬編碼依賴
        paymentProcessor = new PaymentProcessor();
        // 應該是：_paymentProcessor = processor;
    }
}
```

---

## ?? 記住這個流程

```
想要 Mock 測試？
     ↓
需要介面？
     ↓
需要建構函式參數？
     ↓
需要注入依賴？
     ↓
? 現在可以 Mock 了
```

---

## ?? 完整的重構步驟

### 步驟 1：確認你要 Mock 的依賴
```csharp
// OrderService 依賴：
// - PaymentProcessor（需要 Mock）
// - ShippingService（需要 Mock）
```

### 步驟 2：為每個依賴建立介面
```csharp
public interface IPaymentProcessor
{
    bool Charge(string orderId, decimal amount);
}

public interface IShippingService
{
    bool Schedule(string orderId);
}
```

### 步驟 3：修改實現類別使用介面
```csharp
public class PaymentProcessor : IPaymentProcessor
{
    public bool Charge(string orderId, decimal amount)
    {
        // 真實實現...
        return true;
    }
}

public class ShippingService : IShippingService
{
    public bool Schedule(string orderId)
    {
        // 真實實現...
        return true;
    }
}
```

### 步驟 4：修改 OrderService 依賴介面
```csharp
public class OrderService
{
    private readonly IPaymentProcessor _paymentProcessor;
    private readonly IShippingService _shippingService;

    public OrderService(
        IPaymentProcessor paymentProcessor,
        IShippingService shippingService)
    {
        _paymentProcessor = paymentProcessor;
        _shippingService = shippingService;
    }

    public bool PlaceOrder(string orderId, decimal amount)
    {
        if (!_paymentProcessor.Charge(orderId, amount))
            return false;
        return _shippingService.Schedule(orderId);
    }
}
```

### 步驟 5：生產環境使用真實實現
```csharp
var paymentProcessor = new PaymentProcessor();
var shippingService = new ShippingService();
var orderService = new OrderService(paymentProcessor, shippingService);
orderService.PlaceOrder("ORDER001", 100m);
```

### 步驟 6：測試環境使用 Mock
```csharp
[Test]
public void PlaceOrder_PaymentFails_ReturnsFalse()
{
    var paymentMock = Substitute.For<IPaymentProcessor>();
    var shippingMock = Substitute.For<IShippingService>();

    paymentMock.Charge(Arg.Any<string>(), Arg.Any<decimal>()).Returns(false);

    var service = new OrderService(paymentMock, shippingMock);
    var result = service.PlaceOrder("ORDER001", 100m);

    Assert.IsFalse(result);
}
```

---

## ?? 最後的重點

### ? 這樣不能 Mock
```
硬編碼依賴 + 沒有介面 + 沒有建構函式參數
```

### ? 這樣可以 Mock
```
介面依賴 + 建構函式注入 + 沒有硬編碼
```

**簡單說：你要給類別選擇依賴的機會！**

---

## ?? 回顧三個核心觀念

### 什麼是介面？
- 定義契約的工具
- 說「你必須實現這些方法」
- Mock 基於介面建立

### 什麼是依賴注入？
- 不在類別內部建立依賴
- 改成透過建構函式傳入
- 讓調用者決定傳什麼

### 什麼是 Mock？
- 假的物件
- 實現介面
- 用來測試

**三者結合 = 可 Mock 的設計！**

