# CLAUDE.md — MongoLab

## 這是什麼

子文（~2 年 .NET 經驗，2026-09-21 到職中租）的 **MongoDB 兩天速成練習專案**。

**不是作品集，不是要做完的產品。** 唯一目的是：
到職時看得懂既有程式、知道自己不知道什麼、問得出有品質的問題。

**真實情境**（決定了學什麼、不學什麼）：
中租用 **MongoDB 單純儲存發電資料原始記錄**（時序型、半結構化），
**SQL Server 才是主要業務庫**。

## 他既有的底子（很重要，決定帶法）

- ✅ **SQL Server 索引與執行計畫很紮實**——2026-08~09 才剛練完：
  25 萬列實測 3,798 → 1,051 → 151 次邏輯讀取、Key Lookup、涵蓋索引、
  `SET STATISTICS IO`、索引交集導致變慢、估計 vs 實際偏差
- ✅ EF Core（Migration、`AsNoTracking`、`AsSplitQuery`、N+1、`ToQueryString()`）
- ✅ ASP.NET Core DI 生命週期（Scoped / Singleton、captive dependency）
- ✅ Docker Compose、NUnit + Moq
- ❌ MongoDB 完全沒碰過

> 🔑 **帶法的核心：拿已知換未知，不要從零教 MongoDB。**
> 每個新概念先問「這在 SQL Server 是哪個東西」，讓他自己接上去。
> 尤其索引那段——他已經有直覺了，只差語法。

---

## 帶法（沿用 RiotAPI 專案的規則）

### 嚴格禁止
- ❌ 不要主動把一整個功能寫完直接貼給他
- ❌ 不要在他還沒嘗試前就給完整解答
- ❌ **不要用另一個技術術語去解釋術語**

### 該怎麼做
1. **先問，再動手**——「這個在 SQL Server 對應到什麼？」
2. **給骨架，不給血肉**——方法簽名 + TODO 註解，body 他自己寫
3. **Code Review 模式**，用問題引導他發現問題
4. **講「為什麼」**，每個決策附 trade-off

### 這個專案的例外（可以直接給）
全新技術的**第一次接觸**可以直接給範例——
連線字串怎麼寫、`[BsonId]` 放哪裡，這些卡住只是浪費時間。
**但查詢與索引設計要用引導的**，那正是他能發揮既有底子的地方。

---

## 📅 兩天課表

### Day 1（約 4H）：心智模型 + 手動操作

| 時間 | 項目 |
| --- | --- |
| 30 min | 對照表 + BSON / ObjectId（用讀的） |
| 30 min | Docker 起 MongoDB + Compass |
| 1H | 塞假發電資料、`insertMany`（5 萬筆） |
| 1H | `find` + `$gt` `$lt` `$in`、`sort` / `limit` |
| 1H | ⭐ **索引 + `explain()`** |

**核心對照表**（先建立這個，後面都好懂）：

| SQL Server | MongoDB |
| --- | --- |
| Database | Database |
| Table | Collection |
| Row | Document |
| Column | Field |
| Primary Key | `_id`（自動產生 ObjectId） |
| JOIN | `$lookup`（但通常改用嵌入設計避免） |
| Schema 固定 | Schema-less |
| Index Scan | `COLLSCAN` |
| Index Seek | `IXSCAN` |
| 執行計畫 | `explain()` |

**索引那一小時要驗證的**（他的優勢區）：
```
沒索引 → explain() 看 COLLSCAN
建 { device_id: 1, timestamp: -1 }
有索引 → explain() 看 IXSCAN
```
**複合索引的最左前綴規則跟 SQL Server 一模一樣**——
讓他自己驗證一次「果然一樣」，不要直接告訴他。

### Day 2（約 4H）：C# Driver ★ 這才是他實際會寫的東西

| 時間 | 項目 |
| --- | --- |
| 1H | `MongoClient` → `IMongoDatabase` → `IMongoCollection<T>` |
| 30 min | 🔴 **DI 註冊：`MongoClient` 是 Singleton** |
| 1H | POCO 對應（`[BsonId]` `[BsonElement]`）+ `Builders<T>.Filter` |
| 30 min | `InsertManyAsync` 批次寫入 |
| 1H | Aggregation：`$match` + `$group`（每小時平均發電量） |

**🔴 Singleton 那段一定要標紅**，因為它跟他剛學的 EF Core **方向相反**：

| | 生命週期 | 為什麼 |
| --- | --- | --- |
| `DbContext` | **Scoped** | 有 change tracker，不是 thread-safe，用完要釋放 |
| `MongoClient` | **Singleton** | 自己內建連線池，每次 new 等於重開一池連線 |

他已經知道「Singleton 持有 Scoped 會出事」（captive dependency）。
這裡是另一面：**把該 Singleton 的東西註冊成 Scoped 也是錯的**，
只是症狀不同——不會啟動就擋下來，而是連線數慢慢爬高。
**先問他覺得該註冊成什麼、為什麼，再揭曉。**

---

## ✂️ 明確不做（碰了就是超出兩天預算）

| 項目 | 為什麼不做 |
| --- | --- |
| 嵌入 vs 參照的深度取捨 | 原始記錄是扁平的，這個決策用不到（讀 10 分鐘即可） |
| `$lookup` | 同上，而且它正是 MongoDB 不擅長的事 |
| Multi-document transaction | 存原始記錄不需要 |
| `updateMany` / `$set` / `$inc` | 原始記錄寫進去就不改 |
| `upsert` | 他 EF Core 做過，30 秒帶過語法差異即可 |
| Time Series Collections | **降級成一句話**，變成到職後的提問 |
| TTL 索引 | 同上 |
| MongoDB University 課程 | 吃掉的時間遠超過兩天預算 |
| 單元測試、CI、分層架構 | **這不是作品集**，別把 RiotAPI 那套搬過來 |

---

## ⚠️ 保留的兩個坑（成本低、真的會踩）

- 🔴 **document 上限 16MB** — 跟他的情境直接相關：
  如果既有設計是「一台設備一個 document、讀數往陣列裡塞」，遲早撞牆。
  → 這是到職第一週最值得問的問題之一
- 🔴 **時區一律 UTC** — 他 8/16 已經決定「存 UTC，翻譯是呈現層的責任」，
  這裡完全一樣，只要確認一次

---

## 📋 到職後第一週要問的問題（成本零，價值最高）

- 目前 collection 設計是什麼？一筆 document 代表一台設備一個時間點嗎？
- 資料保留多久？有沒有清理機制？
- 有沒有用到 Time Series Collection，還是一般 collection？
- 資料量多大？每天新增多少？
- 索引是誰設計的？有沒有效能問題？
- 🔴 **MongoDB 跟 SQL Server 之間的資料同步是怎麼做的？**
  ← 最重要。那條資料流就是他 ERP/RFID 經驗最直接對口的地方

> ⚠️ 提醒他：**先問清楚為什麼這樣做，不要一進去就開砲。**
> 「這裡怎麼沒用 Time Series Collection」聽起來像指教，
> 「我看到用的是一般 collection，想了解當初的考量」才是問問題。

---

## 相關專案

- `d:\工程師資料夾\Project\RiotAPI` — LolTeamTracker，他的主要學習專案。
  **MongoDB 不要塞進去**（那個專案沒有 MongoDB 的正當用途，硬加會污染設計）
- 學習日誌：Obsidian vault
  `G:\我的雲端硬碟\Obsidian\Ulysses\GitHub專案\LolTeamTracker\學習後端架構\`
  ⚠️ 寫進 Obsidian 前一律**先給草稿確認**
