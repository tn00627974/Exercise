/*
 * 你的代碼：為什麼使用介面是好的設計？
 * ════════════════════════════════════════════════════════════════════
 */

// 你的設計：? 使用介面 + 依賴注入
// ════════════════════════════════════════════════════════════════════

/*
public class NewApplication
{
    private readonly IInstaller _installer;  // ← 依賴介面，不是具體類別

    public NewApplication(IInstaller installer)  // ← 通過建構函式注入
    {
        _installer = installer;
    }

    public bool InstallApplication(string url)
    {
        if (!_installer.ValidateUrl(url)) return false;
        try
        {
            var result = _installer.Install(url);
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}

public interface IInstaller
{
    bool Install(string url);
    bool ValidateUrl(string url);
}

public class Installer : IInstaller
{
    public bool Install(string url)
    {
        return true;
    }

    public bool ValidateUrl(string url)
    {
        var regex = new Regex(
            @"^https?://(?:www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b(?:[-a-zA-Z0-9()@:%_\+.~#?&/=]*)$");
        return regex.IsMatch(url);
    }
}
*/

// ════════════════════════════════════════════════════════════════════
// 你的測試代碼：展現了介面的強大之處
// ════════════════════════════════════════════════════════════════════

/*
[TestCase("test", false)]
[TestCase("https://www.google.com", true)]
public void InstallApplication_WithUrlValidation_ReturnsCorrectResult(
    string url, 
    bool expected)
{
    // Arrange - 創建 Mock
    var installer = Substitute.For<IInstaller>();  // ← 關鍵！
    
    // 配置 Mock 的行為
    installer.ValidateUrl(url).Returns(url == "https://www.google.com");
    installer.Install(url).Returns(true);
    
    var app = new NewApplication(installer);

    // Act
    var result = app.InstallApplication(url);

    // Assert
    Assert.That(result, Is.EqualTo(expected));
}

[Test]
public void InstallApplication_WhenInstallExecption_CatchExecption()
{
    var installer = Substitute.For<IInstaller>();

    // ← 模擬異常情況
    installer.ValidateUrl("test").Returns(true);
    installer
        .When(x => x.Install("test"))
        .Do(x => { throw new Exception(); });

    var app = new NewApplication(installer);

    // ← 驗證異常被正確拋出
    Assert.That(
        () => app.InstallApplication("test"), 
        Throws.Exception
    );
}
*/

// ════════════════════════════════════════════════════════════════════
// 為什麼這個設計好？詳細說明
// ════════════════════════════════════════════════════════════════════

/*
 * 
 * ?? 要點 1：NewApplication 依賴 IInstaller（介面）
 * ────────────────────────────────────────────────────
 * 
 * private readonly IInstaller _installer;
 * 
 * ? 好處：
 *    - NewApplication 不需要知道 Installer 的具體實現
 *    - 可以接受任何實現 IInstaller 的對象
 *    - 包括真實的 Installer，也包括 Mock
 * 
 * 
 * ?? 要點 2：通過建構函式注入依賴
 * ────────────────────────────────────────────────────
 * 
 * public NewApplication(IInstaller installer)
 * {
 *     _installer = installer;
 * }
 * 
 * ? 好處：
 *    - 類別不負責創建 Installer
 *    - 調用者決定傳入什麼實現
 *    - 生產環境傳入真實 Installer
 *    - 測試環境傳入 Mock Installer
 * 
 * 
 * ?? 要點 3：測試時使用 Mock
 * ────────────────────────────────────────────────────
 * 
 * var installer = Substitute.For<IInstaller>();  // ← Mock
 * installer.ValidateUrl(url).Returns(...);
 * installer.Install(url).Returns(...);
 * var app = new NewApplication(installer);
 * 
 * ? 好處：
 *    - 不需要真實的 Installer 實現
 *    - 完全控制 Mock 的行為
 *    - 可以模擬任何情況：成功、失敗、異常等
 *    - 測試快速、穩定、隔離
 * 
 * 
 * ?? 要點 4：對比沒有介面的版本
 * ────────────────────────────────────────────────────
 * 
 * ? 沒有介面的寫法：
 * 
 *     public class BadNewApplication
 *     {
 *         private readonly Installer _installer;
 *         
 *         public BadNewApplication()
 *         {
 *             _installer = new Installer();  // ← 硬編碼
 *         }
 *     }
 * 
 * 問題：
 * ? 無法傳入 Mock
 * ? 測試時會執行真實的 Installer
 * ? ValidateUrl() 會真的檢查 URL 正則表達式
 * ? Install() 會真的執行安裝邏輯
 * ? 測試不穩定、不快速
 * 
 * 
 * ?? 要點 5：實際執行流程
 * ────────────────────────────────────────────────────
 * 
 * 測試時發生什麼：
 * 
 * 1. var installer = Substitute.For<IInstaller>();
 *    → 創建一個 Mock 對象，實現 IInstaller 介面
 * 
 * 2. installer.ValidateUrl(url).Returns(...);
 *    → 配置 Mock：當調用 ValidateUrl("test") 時，返回 false
 *               當調用 ValidateUrl("https://www.google.com") 時，返回 true
 * 
 * 3. var app = new NewApplication(installer);
 *    → 把 Mock 傳給 NewApplication
 *    → NewApplication 將 Mock 保存到 _installer
 * 
 * 4. var result = app.InstallApplication("test");
 *    → NewApplication 調用 _installer.ValidateUrl("test")
 *    → Mock 返回 false（因為我們配置過）
 *    → InstallApplication 直接返回 false，不繼續執行
 * 
 * 5. Assert.That(result, Is.EqualTo(expected));
 *    → 驗證結果是否符合預期
 * 
 * 整個過程：
 * ? 沒有真實的 Installer
 * ? 沒有真實的 URL 驗證
 * ? 沒有真實的安裝邏輯
 * ? 完全被測試代碼控制
 * 
 * 
 * ?? 對比表：有介面 vs 沒介面
 * ════════════════════════════════════════════════════════════════════
 * 
 * | 方面           | 沒介面        | 有介面        |
 * |----------------|----------------|----------------|
 * | 可以 Mock      | ? 否          | ? 是         |
 * | 測試速度       | ?? 可能很慢    | ?? 毫秒級     |
 * | 測試穩定性     | ? 依賴外部    | ? 完全隔離   |
 * | 異常測試       | ? 困難        | ? 容易       |
 * | 代碼耦合度     | ?? 高          | ?? 低         |
 * | 易於修改       | ? 困難        | ? 容易       |
 * | 易於擴展       | ? 困難        | ? 容易       |
 * 
 * 
 * ?? 核心答案：為什麼介面讓測試更好？
 * ════════════════════════════════════════════════════════════════════
 * 
 * 1. ?? 可替換性
 *    - 類別依賴介面（抽象），不依賴具體實現
 *    - 可以輕鬆替換 Mock 或其他實現
 * 
 * 2. ?? 隔離性
 *    - 每個類別可以獨立測試
 *    - 不依賴外部資源或其他類別
 * 
 * 3. ? 效率
 *    - Mock 不執行真實邏輯
 *    - 測試快速
 * 
 * 4. ??? 穩定性
 *    - 完全控制行為
 *    - 測試結果可靠
 * 
 * 5. ?? 測試覆蓋
 *    - 容易測試異常情況
 *    - 容易測試邊界情況
 *    - 容易測試各種組合
 * 
 * 
 * ?? 實踐建議：你已經做對了！
 * ════════════════════════════════════════════════════════════════════
 * 
 * 你的代碼已經正確地使用了介面和依賴注入。
 * 你的測試也正確地使用了 Mock 來隔離 NewApplication。
 * 
 * 這是最佳實踐！?
 */
