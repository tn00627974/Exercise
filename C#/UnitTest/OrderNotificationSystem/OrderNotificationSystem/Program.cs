global using OrderNotificationSystem.Models;
global using OrderNotificationSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// See https://aka.ms/new-console-template for more information
Console.WriteLine("=== 訂單通知系統調試 ===\n");

// 測試 1：沒有配置發送器的情況
Console.WriteLine("測試 1：沒有配置發送器");
var composites = new CompositeNotificationSender();
var order1 = new Order { OrderNo = "ORD001", Amount = 1000 };
var result1 = composites.Send(order1);
Console.WriteLine($"結果：Success={result1.Success}, Message={result1.Message}\n");

// 測試 2：配置 Email 和 Line 發送器
Console.WriteLine("測試 2：配置 Email 和 Line 發送器");
var emailSender = new EmailNotificationSender();
var lineSender = new LineNotificationSender();
var composites2 = new CompositeNotificationSender(emailSender, lineSender);
var order2 = new Order { OrderNo = "ORD002", Amount = 2000 };
var result2 = composites2.Send(order2);
Console.WriteLine($"結果：Success={result2.Success}, Message={result2.Message}\n");

// 測試 3：通過 OrderService 使用
Console.WriteLine("測試 3：通過 OrderService 使用");
var validator = new DefaultOrderValidator();
var sender = new CompositeNotificationSender(emailSender, lineSender);
var orderService = new OrderService(validator, sender);
var order3 = new Order { OrderNo = "ORD003", Amount = 500 };
var result3 = orderService.PlaceOrder(order3);
Console.WriteLine($"結果：Success={result3.Success}, Message={result3.Message}\n");

Console.WriteLine("=== 調試完成 ===");