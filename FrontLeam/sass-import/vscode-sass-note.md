# VS Code 啟用 Sass 筆記

## 1. 安裝 VS Code 擴充套件
- 推薦安裝 **Live Sass Compiler** 或 **Sass** 擴充套件。
- 在 Extensions 市集搜尋 `Live Sass Compiler` 並安裝。

## 2. 專案結構建議
- 建議將 `.scss` 檔案放在 `scss/` 資料夾，編譯輸出到 `css/` 資料夾。

## 3. 編譯 Sass
- 點擊 VS Code 右下角的 `Watch Sass` 按鈕，或在命令面板執行 `Live Sass: Watch Sass`.
- 編譯後會自動產生對應的 `.css` 檔案。

## 4. 使用命令列編譯（可選）
- 安裝 Dart Sass:  
  ```
  npm install -g sass
  ```
- 編譯指令範例：  
  ```
  sass scss/all.scss css/all.css
  ```

## 5. 注意事項
- 優先使用 `@use`/`@forward`，避免使用過時的 `@import`。
- 變數、mixin、function 請集中管理於 `_variable.scss`、`_mixin.scss` 等檔案。
- 檔案命名慣例：底線開頭（如 `_main.scss`）代表僅供匯入，不會單獨編譯成 CSS。

## 6. 參考資源
- [Sass 官方文件](https://sass-lang.com/documentation/)
- [Live Sass Compiler](https://marketplace.visualstudio.com/items?itemName=glenn2223.live-sass)

---

## Dart Sass 與 Dart 是什麼？

### Dart Sass
- **Dart Sass** 是 Sass 官方唯一持續維護的實作版本。
- 支援最新的 Sass 語法（如 `@use`、`@forward`）。
- 可用於命令列、Node.js 或各種建構工具。

### Dart
- **Dart** 是 Google 開發的程式語言，主要用於前端（如 Flutter）。
- Dart Sass 是用 Dart 語言寫成的，但你不需要會 Dart 就能用 Dart Sass。

---

## Flutter 與原生 Android/iOS 開發的差異

- **Flutter** 是 Google 推出的跨平台 UI 框架，使用 Dart 語言，一套程式碼可同時產生 Android 和 iOS App。
- **原生 Android 開發** 使用 Java 或 Kotlin，專為 Android 系統設計。
- **原生 iOS 開發** 使用 Swift 或 Objective-C，專為蘋果 iOS 系統設計。

### 差異比較
| 項目         | Flutter                | 原生 Android         | 原生 iOS           |
|--------------|------------------------|----------------------|--------------------|
| 語言         | Dart                   | Java/Kotlin          | Swift/Objective-C  |
| 跨平台       | 支援（一次開發多平台）  | 僅 Android           | 僅 iOS             |
| UI 樣式      | 自繪（高度自訂）       | 系統原生             | 系統原生           |
| 效能         | 接近原生               | 原生最佳             | 原生最佳           |
| 生態系/套件  | 較新，持續成長         | 成熟                 | 成熟               |

- Flutter 適合快速開發多平台 App，節省人力與維護成本。
- 原生開發適合需要極致效能或深度整合系統功能的專案。

---

## 添加 Sass Compiler 的方式

### 1. 使用 VS Code 擴充套件
- 安裝 **Live Sass Compiler**。
- 點擊右下角 `Watch Sass`，自動編譯 `.scss` 檔案。

### 2. 使用命令列（Dart Sass）
- 安裝 Dart Sass：
  ```
  npm install -g sass
  ```
- 編譯指令：
  ```
  sass scss/all.scss css/all.css
  ```
- 可加 `--watch` 參數自動監控：
  ```
  sass --watch scss:css
  ```

### 3. 其他建構工具
- 可整合到 Webpack、Gulp、Grunt 等前端建構流程。

---
