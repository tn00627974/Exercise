# RFID 倉儲/維修管理 APP (RFID Warehouse & Maintenance App)

## 專案簡介 (Overview)
本專案為一款基於 **.NET MAUI** 開發的跨平台行動應用程式，專為倉儲盤點與設備維修管理設計。核心功能整合 **RFID 掃描** 技術，並採用現代化的 **MVVM 架構** 與 **粉+藍+蜂蜜色** 漸層介面風格。

## 技術堆疊 (Tech Stack)
- **Framework**: .NET MAUI (Multi-platform App UI)
- **Language**: C# 10+
- **Architecture**: MVVM (Model-View-ViewModel)
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection
- **UI Styling**: XAML with Custom ControlTemplates & Gradients

## 專案架構 (Architecture)

本專案採用標準的分層架構，確保邏輯與介面分離，易於維護與擴充。

```
RfidApp/
├── Models/          # 資料模型 (DTOs)
│   ├── InventoryItem.cs  # 庫存項目
│   ├── RepairTicket.cs   # 維修單
│   └── User.cs           # 使用者資訊
├── Services/        # 業務邏輯與資料存取層
│   ├── IAuthService.cs   # 身分驗證介面
│   ├── IDataService.cs   # 資料存取介面 (Mock/API)
│   ├── IRfidService.cs   # RFID 硬體整合介面
│   └── (Implementations) # 實作檔 (AuthService, MockDataService, RfidService)
├── ViewModels/      # 視圖模型 (處理 UI 邏輯與狀態)
│   ├── BaseViewModel.cs      # INotifyPropertyChanged 實作
│   ├── LoginViewModel.cs     # 登入邏輯
│   ├── DashboardViewModel.cs # 儀表板數據聚合
│   ├── InventoryViewModel.cs # 庫存列表與掃描邏輯
│   └── RepairViewModel.cs    # 維修單管理邏輯
├── Views/           # 使用者介面 (XAML)
│   ├── LoginPage.xaml        # 登入頁
│   ├── DashboardPage.xaml    # 主儀表板
│   ├── InventoryPage.xaml    # 庫存頁面
│   └── RepairPage.xaml       # 維修頁面
├── App.xaml         # 全域資源 (Styles, Colors, Templates)
├── AppShell.xaml    # 導航結構 (TabBar, Routes)
└── MauiProgram.cs   # 應用程式進入點與 DI 註冊
```

## 核心邏輯說明 (Core Logic)

### 1. 模擬 RFID 服務 (Mock RFID Service)
- **檔案**: `Services/RfidService.cs`
- **邏輯**: 使用 `System.Timers.Timer` 模擬硬體掃描行為。
- **事件**: 當計時器觸發時，隨機從預設清單中選取一個 Tag ID，並觸發 `TagDetected` 事件。
- **整合**: `InventoryViewModel` 訂閱此事件，當收到 Tag 時，自動搜尋並顯示對應商品。

### 2. 資料服務 (Data Service)
- **檔案**: `Services/MockDataService.cs`
- **邏輯**: 目前使用記憶體內的 `List<T>` 作為暫存資料庫。
- **擴充性**: 未來可直接替換為 `SqliteDataService` 或 `ApiDataService`，只需實作 `IDataService` 介面即可，無需修改 ViewModel。

### 3. 介面主題 (UI Theme)
- **檔案**: `App.xaml`
- **配色**: 定義了 `MainGradient` (LinearGradientBrush)，由 Pink (`#FFC3A0`) -> Honey (`#FFD1A9`) -> Blue (`#A2D2FF`) 組成。
- **樣式**: 全域定義了 `CardStyle` (卡片容器) 與 `PrimaryButtonStyle` (漸層按鈕)，確保視覺一致性。

## 如何開始 (Getting Started)
1. 確認已安裝 **Visual Studio 2022** 並包含 **.NET MAUI** 工作負載。
2. 開啟 `RfidApp.sln`。
3. 選擇目標平台 (Android Emulator 或 Windows Machine)。
4. 按下 F5 執行。
5. **登入**: 輸入任意非空帳號密碼即可 (Mock Auth)。

## 待辦事項 (Future Work)
- [ ] 實作「新增/編輯」庫存與維修單的功能。
- [ ] 整合真實 RFID SDK (如 Zebra DataWedge)。
- [ ] 串接後端 API 或 SQLite 資料庫。
