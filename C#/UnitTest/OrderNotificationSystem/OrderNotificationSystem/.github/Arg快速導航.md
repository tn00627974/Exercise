# Arg.Any vs 具體物件 - 快速導航

## ?? 五分鐘快速答案

### 你的問題
```csharp
_mockSender.Send(Arg.Any<Order>()).Returns(expectedResult);
_mockSender.Send(order).Returns(expectedResult);
```

### 答案

| 寫法 | 含義 | 結果 |
|------|------|------|
| `Arg.Any<Order>()` | 任何Order都符合 | ? 推薦 |
| `order` | 只有這個Order物件 | ? 不推薦 |

### 你的問題
第二行覆蓋了第一行！只需要第一行。

```csharp
? 正確
_mockSender.Send(Arg.Any<Order>()).Returns(expectedResult);
```

---

## ?? 深入學習（選一個閱讀）

### ?? 最推薦：一張圖勝千言.md
- 時間：5分鐘
- 內容：視覺化圖表，一目了然
- 適合：快速理解

### ?? 詳細解釋.md
- 時間：10分鐘
- 內容：詳細的類比和解釋
- 適合：透徹理解

### ?? 視覺化圖解.md
- 時間：15分鐘
- 內容：記憶體位址圖解
- 適合：完全搞懂

### ?? 可執行範例.cs
- 時間：20分鐘
- 內容：可以實際執行的代碼
- 適合：自己驗證

---

## ? 核心區別

```
Arg.Any<Order>()
↓
【不檢查內容，只看型別】
↓
任何Order都符合 ?

order 具體物件
↓
【檢查記憶體位址】
↓
只有同一個實例符合 ?
```

---

## ?? 記住這句話

> **「99% 的測試都用 Arg.Any<T>()，別想太多」**

---

## ? 你已經修正的代碼

```csharp
// ? 原始（有問題）
_mockSender.Send(Arg.Any<Order>()).Returns(expectedResult);
_mockSender.Send(order).Returns(expectedResult);

// ? 修正（正確）
_mockSender.Send(Arg.Any<Order>()).Returns(expectedResult);
```

---

## 下一步

- [ ] 選一個文檔快速閱讀
- [ ] 理解為什麼有差別
- [ ] 確認你的測試代碼已修正
- [ ] 繼續完成第 6 關！

---

**完成！** 現在你可以繼續寫測試代碼了。??
