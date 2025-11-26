# RFID App 技術規格文件 (Technical Specification)

## 1. 專案概述 (Project Overview)
本專案旨在開發一款基於 RFID 技術的行動應用程式，用於倉庫管理與維修管理。透過行動裝置與 RFID 讀取器的整合，提升庫存盤點、貨物移動及設備維修流程的效率與準確性。

## 2. 技術堆疊 (Technology Stack)
*   **開發框架**: .NET MAUI (Multi-platform App UI)
*   **程式語言**: C# 10+
*   **目標平台**: Android 8.0+, iOS 14+
*   **開發環境**: Visual Studio 2022 / VS Code
*   **架構模式**: MVVM (Model-View-ViewModel)
*   **依賴注入**: .NET MAUI 內建 Dependency Injection

## 3. 系統架構 (System Architecture)
本專案採用標準的 **MVVM** 架構，以確保程式碼的可維護性與測試性。

### 3.1 目錄結構
```text
RfidApp/
├── Models/          # 資料模型 (Data Models)
├── ViewModels/      # 視圖模型 (Business Logic & State)
├── Views/           # 使用者介面 (UI Pages)
├── Services/        # 服務層 (API, Database, RFID Hardware)
├── App.xaml         # 應用程式入口與全域資源
├── AppShell.xaml    # 導航結構定義
└── MauiProgram.cs   # 應用程式啟動與依賴註冊
```

### 3.2 核心層級
1.  **View (UI)**: 使用 XAML 定義介面，負責資料綁定與視覺呈現。
2.  **ViewModel**: 處理業務邏輯、命令 (Commands) 與狀態通知 (INotifyPropertyChanged)。
3.  **Model**: 定義資料結構 (如 `InventoryItem`, `RepairOrder`)。
4.  **Service**: 負責外部通訊，包括 RFID 硬體通訊、後端 API 呼叫或本地資料庫存取。

## 4. 功能模組 (Functional Modules)

### 4.1 身份驗證 (Authentication)
*   **功能**: 使用者登入/登出。
*   **流程**: 輸入帳號密碼 -> 驗證 -> 取得 Token -> 進入主頁。
*   **擴充性**: 預留 OAuth (Google/Microsoft) 登入介面。

### 4.2 倉庫管理 (Warehouse Management)
*   **RFID 盤點 (Inventory)**: 
    *   透過 RFID 批量讀取標籤。
    *   即時比對系統帳面數量與實際讀取數量。
*   **庫存列表 (Stock List)**: 檢視目前庫存狀態、篩選與搜尋。
*   **貨物移動 (Goods Movement)**: 記錄入庫、出庫與移庫操作。

### 4.3 維修管理 (Maintenance Management)
*   **報修申請 (Repair Requests)**: 填寫報修單，包含設備資訊與故障描述。
*   **維修單管理 (Order Management)**: 追蹤維修進度 (待處理、維修中、已完成)。
*   **RFID 綁定 (RFID Binding)**: 將 RFID 標籤與維修設備或零件進行綁定/解綁。

## 5. 使用者介面設計 (UI/UX Design)
*   **風格**: 現代化、柔和風格 (Soft & Clean)。
*   **配色**: 粉色與藍色系的蜜糖漸層 (Pink-Blue Honey Gradient)。
*   **互動**: 
    *   使用流暢的轉場動畫。
    *   按鈕與卡片採用圓角設計與陰影效果 (Glassmorphism 元素)。
    *   支援深色模式 (Dark Mode) 適配。

## 6. 硬體整合 (Hardware Integration)
*   **RFID 讀取**: 
    *   透過 `IRfidService` 介面抽象化硬體操作。
    *   支援藍牙 (Bluetooth) 連接手持式 RFID 讀取器 (如 Zebra, CSL 等)。
    *   實作標籤過濾 (RSSI Filter) 與去重 (De-duplication) 邏輯。

## 7. 資料與通訊 (Data & Communication)
*   **API 通訊**: 使用 `HttpClient` 呼叫 RESTful API。
*   **本地儲存**: 使用 `SQLite` 或 `Preferences` 儲存使用者設定與離線暫存資料。
*   **錯誤處理**: 統一的例外處理機制與使用者提示 (Toast/Alert)。

## 8. 未來擴充規劃 (Roadmap)
*   [ ] 整合雲端 OAuth 登入。
*   [ ] 離線作業模式 (Offline Sync)。
*   [ ] 推播通知 (Push Notifications)。
*   [ ] 進階報表與圖表分析。
