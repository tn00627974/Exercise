# Arg.Any vs 具體物件 - 視覺化圖解

## 記憶體中的物件實例

```
Arg.Any<Order>() 的情況：
=================================
設定：_mockSender.Send(Arg.Any<Order>()).Returns(success)

執行 1：_mockSender.Send(new Order { OrderNo = "ORD001" })
        ↓
        我是一個新的 Order 物件
        記憶體位址：0x1000
        內容：{ OrderNo = "ORD001" }
        
        Mock 檢查：「是 Order 型別嗎？」 
                  → 是！
                  → 返回 success ?

執行 2：_mockSender.Send(new Order { OrderNo = "ORD002" })
        ↓
        我是一個新的 Order 物件
        記憶體位址：0x2000
        內容：{ OrderNo = "ORD002" }
        
        Mock 檢查：「是 Order 型別嗎？」
                  → 是！
                  → 返回 success ?

執行 3：_mockSender.Send(new Order { OrderNo = "ORD001" })
        ↓
        我是一個新的 Order 物件
        記憶體位址：0x3000
        內容：{ OrderNo = "ORD001" }（和執行 1 內容相同）
        
        Mock 檢查：「是 Order 型別嗎？」
                  → 是！
                  → 返回 success ?

結論：Arg.Any 不關心內容和位址，只看型別
所以全部都匹配！
```

---

```
具體物件 order 的情況：
=================================
設定：var order = new Order { OrderNo = "ORD001" }
     _mockSender.Send(order).Returns(success)

     記憶體位址：0x1000
     ┌─────────────────┐
     │ order 物件      │
     │ OrderNo="ORD001"│
     │ 位址：0x1000    │
     └─────────────────┘

執行 1：_mockSender.Send(order)
        ↓
        傳入的物件位址：0x1000
        設定的物件位址：0x1000
        
        Mock 檢查：「位址相同嗎？」
                  → 是！(0x1000 == 0x1000)
                  → 返回 success ?

執行 2：_mockSender.Send(new Order { OrderNo = "ORD001" })
        ↓
        我是一個新的 Order 物件
        記憶體位址：0x2000
        內容：{ OrderNo = "ORD001" }（內容和 order 相同！）
        
        Mock 檢查：「位址相同嗎？」
                  → 不是！(0x2000 != 0x1000)
                  → 返回 NOT_SET ?
        
        注意：即使內容完全相同，也不符合，因為位址不同

執行 3：_mockSender.Send(new Order { OrderNo = "ORD002" })
        ↓
        我是一個新的 Order 物件
        記憶體位址：0x3000
        內容：{ OrderNo = "ORD002" }（內容不同）
        
        Mock 檢查：「位址相同嗎？」
                  → 不是！(0x3000 != 0x1000)
                  → 返回 NOT_SET ?

結論：具體物件只匹配記憶體位址相同的物件
所以只有執行 1 符合！
```

---

## 你的測試代碼發生了什麼

```
原始代碼：
=================================

var order = new Order { OrderNo = "ORD001" };
記憶體位址：0x1000

Step 1: _mockSender.Send(Arg.Any<Order>()).Returns(expectedResult);
        ┌────────────────────────────┐
        │ Mock 設定 1：               │
        │ 任何 Order 都返回           │
        │ expectedResult             │
        └────────────────────────────┘

Step 2: _mockSender.Send(order).Returns(expectedResult);
        ┌────────────────────────────┐
        │ Mock 設定 2：               │
        │ 只有位址 0x1000 的 Order   │
        │ 才返回 expectedResult      │
        │（覆蓋設定 1）               │
        └────────────────────────────┘
        
        注意：設定 2 把設定 1 覆蓋了！

Step 3: 測試執行：_mockSender.Send(order);
        傳入的物件位址：0x1000
        
        Mock 檢查最新的設定（設定 2）：
        「位址是 0x1000 嗎？」
        → 是！
        → 返回 expectedResult ?
```

---

## 修正後的代碼

```
修正代碼：
=================================

var order = new Order { OrderNo = "ORD001" };
記憶體位址：0x1000

Step 1: _mockSender.Send(Arg.Any<Order>()).Returns(expectedResult);
        ┌────────────────────────────┐
        │ Mock 設定（唯一的設定）：    │
        │ 任何 Order 都返回           │
        │ expectedResult             │
        └────────────────────────────┘

Step 2: 測試執行：_mockSender.Send(order);
        傳入的物件位址：0x1000
        
        Mock 檢查設定：
        「是 Order 型別嗎？」
        → 是！
        → 返回 expectedResult ?

結論：現在只有一個設定，沒有覆蓋的問題
```

---

## 記憶體位址的視覺化

```
Arg.Any<Order>()：
=================

Mock 設定：「接受一切 Order」

傳入：Order @ 0x1000 → ? 符合
傳入：Order @ 0x2000 → ? 符合
傳入：Order @ 0x3000 → ? 符合
傳入：Order @ 0x4000 → ? 符合
(任何位址的 Order 都符合)


具體物件 order：
================

var order = new Order() @ 0x1000
Mock 設定：「只接受位址 0x1000 的 Order」

傳入：同一個 order @ 0x1000          → ? 符合
傳入：new Order() @ 0x2000（內容相同） → ? 不符合
傳入：new Order() @ 0x3000（內容不同） → ? 不符合
(只有同一個物件實例才符合)
```

---

## 類比幫助理解

```
Arg.Any<Order>() = 公車票
======================
規則：「任何人都可以上車」
人 A 上車（Arg.Any 匹配）?
人 B 上車（Arg.Any 匹配）?
人 C 上車（Arg.Any 匹配）?

具體物件 = VIP 停車位鑰匙
======================
規則：「只有拿著這把鑰匙的人才能進」
你拿著鑰匙進入（位址相同）?
別人拿著一樣的鑰匙進入（位址不同）?
（即使是一模一樣的鑰匙，也必須是同一把）
```

---

## 現在你明白了嗎？

? `Arg.Any<Order>()` = 看型別
? `order` = 看記憶體位址（同一個實例）
? 內容相同 ≠ 同一個實例

所以在測試中：
- **99% 的時候用 `Arg.Any<T>()`**
- **不要混合兩種寫法**
