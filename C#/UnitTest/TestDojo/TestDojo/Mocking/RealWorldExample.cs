/*
 * 真實世界例子：餐廳訂單系統
 * 
 * 假設你要測試一個餐廳訂單系統
 * 系統需要調用支付服務和配送服務
 */

namespace TestDojo.Mocking.RealWorldExample;

// ? 沒有介面的寫法
public class BadOrderService
{
    private PaymentProcessor paymentProcessor;
    private ShippingService shippingService;
    
    public BadOrderService()
    {
        // ? 硬編碼依賴！
        paymentProcessor = new PaymentProcessor();
        shippingService = new ShippingService();
    }
    
    public bool PlaceOrder(string orderId, decimal amount)
    {
        // 步驟 1: 向銀行扣款
        if (!paymentProcessor.Charge(orderId, amount))
            return false;
        
        // 步驟 2: 安排配送
        return shippingService.Schedule(orderId);
    }
}

// PaymentProcessor 實現
public class PaymentProcessor
{
    public bool Charge(string orderId, decimal amount)
    {
        // 真的連接到銀行系統
        // ?? 問題：測試時會真的扣款！
        Console.WriteLine($"正在向銀行扣款 {amount} 元");
        Thread.Sleep(5000);  // 模擬網路延遲
        return true;
    }
}

// ShippingService 實現
public class ShippingService
{
    public bool Schedule(string orderId)
    {
        // 真的連接到物流系統
        // ?? 問題：測試時會真的寄送訂單！
        Console.WriteLine($"正在安排配送 {orderId}");
        Thread.Sleep(3000);  // 模擬網路延遲
        return true;
    }
}

// 測試 BadOrderService 的問題：
/*
[Test]
public void PlaceOrder_Success()
{
    var service = new BadOrderService();
    
    // ? 問題 1: 每次測試都會真的扣款！
    // 你的銀行帳戶會被扣款！
    
    // ? 問題 2: 每次測試都會真的寄送訂單
    // 你會收到真的快遞！
    
    // ? 問題 3: 測試需要 8 秒鐘（5秒+3秒）
    // 如果有 100 個測試，就需要 800 秒！
    
    // ? 問題 4: 銀行系統掛掉，測試就失敗
    // 無法控制測試環境
    
    var result = service.PlaceOrder("ORDER001", 100m);
    Assert.IsTrue(result);
}
*/


// ? 使用介面的寫法
public interface IPaymentProcessor
{
    bool Charge(string orderId, decimal amount);
}

public interface IShippingService
{
    bool Schedule(string orderId);
}

public class GoodOrderService
{
    private readonly IPaymentProcessor _paymentProcessor;
    private readonly IShippingService _shippingService;
    
    // ? 通過建構函式注入介面
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

// 測試 GoodOrderService 的優點：
/*
using NSubstitute;

[Test]
public void PlaceOrder_PaymentFails()
{
    // ? 創建 Mock 對象
    var paymentMock = Substitute.For<IPaymentProcessor>();
    var shippingMock = Substitute.For<IShippingService>();
    
    // ? 模擬支付失敗
    paymentMock.Charge(Arg.Any<string>(), Arg.Any<decimal>()).Returns(false);
    
    var service = new GoodOrderService(paymentMock, shippingMock);
    
    // ? 測試支付失敗時的行為
    var result = service.PlaceOrder("ORDER001", 100m);
    Assert.IsFalse(result);
    
    // ? 驗證配送沒有被調用（因為支付已經失敗）
    shippingMock.DidNotReceive().Schedule(Arg.Any<string>());
    
    // ? 優點：
    // - 沒有真的扣款 ?
    // - 沒有真的寄送 ?
    // - 測試在毫秒內完成 ?
    // - 完全可控 ?
}

[Test]
public void PlaceOrder_ShippingFails()
{
    var paymentMock = Substitute.For<IPaymentProcessor>();
    var shippingMock = Substitute.For<IShippingService>();
    
    // ? 模擬支付成功但配送失敗
    paymentMock.Charge(Arg.Any<string>(), Arg.Any<decimal>()).Returns(true);
    shippingMock.Schedule(Arg.Any<string>()).Returns(false);
    
    var service = new GoodOrderService(paymentMock, shippingMock);
    
    var result = service.PlaceOrder("ORDER001", 100m);
    Assert.IsFalse(result);
    
    // ? 驗證支付被調用了
    paymentMock.Received(1).Charge("ORDER001", 100m);
    
    // ? 驗證配送也被調用了
    shippingMock.Received(1).Schedule("ORDER001");
}

[Test]
public void PlaceOrder_Success()
{
    var paymentMock = Substitute.For<IPaymentProcessor>();
    var shippingMock = Substitute.For<IShippingService>();
    
    // ? 模擬兩個服務都成功
    paymentMock.Charge(Arg.Any<string>(), Arg.Any<decimal>()).Returns(true);
    shippingMock.Schedule(Arg.Any<string>()).Returns(true);
    
    var service = new GoodOrderService(paymentMock, shippingMock);
    
    var result = service.PlaceOrder("ORDER001", 100m);
    Assert.IsTrue(result);
}
*/

/*
 * 總結對比
 * 
 * BadOrderService（沒有介面）：
 * ? 真的扣款
 * ? 真的寄送
 * ? 很慢（8秒）
 * ? 不穩定（依賴外部服務）
 * ? 無法測試異常情況
 * 
 * GoodOrderService（使用介面）：
 * ? Mock 不真的扣款
 * ? Mock 不真的寄送
 * ? 很快（毫秒）
 * ? 穩定（完全隔離）
 * ? 容易測試異常情況
 */
