
# LINQ 常用語法

LINQ（Language Integrated Query）是 C# 和 .NET 中用來查詢和操作數據的一個強大工具。這裡是一些常用的 LINQ 查詢語法和操作範例：

### 1. 基本查詢語法

```csharp
var query = from item in collection
            where item.Property == someValue
            select item;
```

### 2. Where 篩選

```csharp
var result = collection.Where(item => item.Property == someValue);
```

### 3. Select 選取

```csharp
var result = collection.Select(item => item.Property);
```

### 4. OrderBy 排序

```csharp
var result = collection.OrderBy(item => item.Property);
```

### 5. OrderByDescending 反向排序

```csharp
var result = collection.OrderByDescending(item => item.Property);
```

### 6. GroupBy 分組

```csharp
var result = collection.GroupBy(item => item.Property);
```

### 7. Join 連接

```csharp
var result = from item1 in collection1
             join item2 in collection2 on item1.Property equals item2.Property
             select new { item1, item2 };
```

### 8. Aggregate 聚合

```csharp
var result = collection.Aggregate((total, next) => total + next);
```

### 9. Any 判斷是否有任何符合條件的項目

```csharp
var exists = collection.Any(item => item.Property == someValue);
```

### 10. All 判斷是否所有項目都符合條件

```csharp
var allMatch = collection.All(item => item.Property == someValue);
```

### 11. Count 計數

```csharp
var count = collection.Count(item => item.Property == someValue);
```

### 12. First / FirstOrDefault 獲取第一個或默認值

```csharp
var first = collection.First(item => item.Property == someValue);
var firstOrDefault = collection.FirstOrDefault(item => item.Property == someValue);
```

### 13. Distinct 去重

```csharp
var uniqueItems = collection.Distinct();
```

### 14. Union 聯合

```csharp
var result = collection1.Union(collection2);
```

### 15. Intersect 交集

```csharp
var result = collection1.Intersect(collection2);
```

### 16. Except 差集

```csharp
var result = collection1.Except(collection2);
```

### 17. ToList 轉換為列表

```csharp
var list = collection.Where(item => item.Property == someValue).ToList();
```

這些只是 LINQ 查詢語法的一部分，LINQ 還有許多其他強大的功能和操作，可以根據你的需求進行組合使用。
