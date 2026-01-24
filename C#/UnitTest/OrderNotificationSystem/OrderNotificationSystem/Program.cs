global using OrderNotificationSystem;
global using OrderNotificationSystem.Models;
using OrderNotificationSystem.Decorators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// See https://aka.ms/new-console-template for more information

Console.WriteLine("=== 訂單通知系統調試 ===\n");

//// 測試 1：沒有配置發送器的情況
//Console.WriteLine("測試 1：沒有配置發送器");
//var composites = new CompositeNotificationSender();
//var order1 = new Order { OrderNo = "ORD001", Amount = 1000 };
//var result1 = composites.Send(order1);
//Console.WriteLine($"結果：Success={result1.Success}, Message={result1.Message}\n");

//// 測試 2：配置 Email 和 Line 發送器
//Console.WriteLine("測試 2：配置 Email 和 Line 發送器");
//var emailSender = new EmailNotificationSender();
//var lineSender = new LineNotificationSender();
//var composites2 = new CompositeNotificationSender(emailSender, lineSender);
//var order2 = new Order { OrderNo = "ORD002", Amount = 2000 };
//var result2 = composites2.Send(order2);
//Console.WriteLine($"結果：Success={result2.Success}, Message={result2.Message}\n");

//// 測試 3：通過 OrderService 使用
//Console.WriteLine("測試 3：通過 OrderService 使用");
//var validator = new DefaultOrderValidator();
//var sender = new CompositeNotificationSender(emailSender, lineSender);
//var orderService = new OrderService(validator, sender);
//var order3 = new Order { OrderNo = "ORD003", Amount = 500 };
//var result3 = orderService.PlaceOrder(order3);
//Console.WriteLine($"結果：Success={result3.Success}, Message={result3.Message}\n");

//Console.WriteLine("=== 調試完成 ===");

Console.WriteLine("=== Arg.Any vs 具體物件 的區別 ===\n");

// 模擬 Mock 框架的行為
Console.WriteLine("假設你有一個 Mock:");
Console.WriteLine("_mockSender.Send(Arg.Any<Order>()).Returns(successResult);");
Console.WriteLine();

// 準備 Order
var order1 = new Order { OrderNo = "ORD001" };
var order2 = new Order { OrderNo = "ORD002" };
var order3 = new Order { OrderNo = "ORD001" };  // 內容和 order1 相同

// 方式 1：Arg.Any - 匹配任何 Order
Console.WriteLine("方式 1：Arg.Any<Order>()");
Console.WriteLine("設定：無論什麼 Order 都返回 success\n");

Console.WriteLine($"order1 = new Order {{ OrderNo = \"ORD001\" }}");
var result1 = CheckMatch_AnyOrder(order1);
Console.WriteLine($"  結果：{result1}\n");

Console.WriteLine($"order2 = new Order {{ OrderNo = \"ORD002\" }}");
var result2 = CheckMatch_AnyOrder(order2);
Console.WriteLine($"  結果：{result2}\n");

Console.WriteLine($"order3 = new Order {{ OrderNo = \"ORD001\" }} (內容和 order1 相同)");
var result3 = CheckMatch_AnyOrder(order3);
Console.WriteLine($"  結果：{result3}\n");

Console.WriteLine("=====================================\n");

// 方式 2：具體物件 - 只匹配特定實例
Console.WriteLine("方式 2：_mockSender.Send(order).Returns(success)");
Console.WriteLine("設定：只有這個特定的 order 物件才返回 success\n");

Console.WriteLine($"order1 = new Order {{ OrderNo = \"ORD001\" }}");
var match1 = CheckMatch_SpecificOrder(order1, order1);  // 同一個
Console.WriteLine($"  呼叫時也用 order1：{match1}\n");

Console.WriteLine($"order2 = new Order {{ OrderNo = \"ORD002\" }}");
var match2 = CheckMatch_SpecificOrder(order1, order2);  // 不同的
Console.WriteLine($"  呼叫時用 order2：{match2}\n");

Console.WriteLine($"order3 = new Order {{ OrderNo = \"ORD001\" }} (內容和 order1 相同)");
var match3 = CheckMatch_SpecificOrder(order1, order3);  // 內容相同但不同實例
Console.WriteLine($"  呼叫時用 order3：{match3}\n");

Console.WriteLine("=====================================\n");

// 物件實例的區別
Console.WriteLine("為什麼會不同？看物件實例：\n");
Console.WriteLine($"order1 == order1          ? {order1 == order1}     (同一個)");
Console.WriteLine($"order1 == order3          ? {order1 == order3}     (內容相同但不同實例)");
Console.WriteLine($"object.ReferenceEquals(order1, order3) ? {object.ReferenceEquals(order1, order3)}");
Console.WriteLine();
Console.WriteLine("Mock 框架使用 == 或 ReferenceEquals 來比較");
Console.WriteLine("所以 order1 和 order3 即使內容相同，也不會被認為是同一個");

    // 模擬 Arg.Any<Order>() 的行為
static string CheckMatch_AnyOrder(Order order)
{
    // Arg.Any 不檢查內容，只檢查型別
    if (order is Order)  // 只要是 Order 型別
    {
    return "✅ 匹配！返回 success";
    }
    return "❌ 不匹配";
}

// 模擬具體物件的行為
static string CheckMatch_SpecificOrder(Order expected, Order actual)
{
    // 具體物件檢查實例是否相同
    if (object.ReferenceEquals(expected, actual))  // 必須是同一個實例
    {
    return "✅ 匹配！返回 success";
    }
    return "❌ 不匹配";
}


//class Order
//{
//    public string OrderNo { get; set; }
//}

/* 預期輸出：

=== Arg.Any vs 具體物件 的區別 ===

方式 1：Arg.Any<Order>()
設定：無論什麼 Order 都返回 success

order1 = new Order { OrderNo = "ORD001" }
  結果：✅ 匹配！返回 success

order2 = new Order { OrderNo = "ORD002" }
  結果：✅ 匹配！返回 success

order3 = new Order { OrderNo = "ORD001" } (內容和 order1 相同)
  結果：✅ 匹配！返回 success

=====================================

方式 2：_mockSender.Send(order).Returns(success)
設定：只有這個特定的 order 物件才返回 success

order1 = new Order { OrderNo = "ORD001" }
  呼叫時也用 order1：✅ 匹配！返回 success

order2 = new Order { OrderNo = "ORD002" }
  呼叫時用 order2：❌ 不匹配

order3 = new Order { OrderNo = "ORD001" } (內容和 order1 相同)
  呼叫時用 order3：❌ 不匹配

=====================================

為什麼會不同？看物件實例：

order1 == order1          ? True     (同一個)
order1 == order3          ? False    (內容相同但不同實例)
object.ReferenceEquals(order1, order3) ? False

Mock 框架使用 == 或 ReferenceEquals 來比較
所以 order1 和 order3 即使內容相同，也不會被認為是同一個

*/
