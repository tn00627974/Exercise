using TestDojo.Mocking;
using NSubstitute;

namespace TestDojoTests.Mocking;

/// <summary>
/// 這個文件展示了為什麼介面讓測試變得更好
/// </summary>
public class WhyInterfacesBetterForTesting
{
    // ? 測試沒有介面的類別 - 困難！
    [Test]
    public void TestBadApplication_DIFFICULT()
    {
        // ? 問題 1: 無法使用 Mock
        // var app = new BadApplication();
        // 無法傳入 Mock 的 IInstaller
        // BadApplication 硬編碼了 new Installer()
        
        // ? 問題 2: 無法控制 Installer 的行為
        // Installer 會真的驗證 URL（使用正則表達式）
        // Installer 會真的執行安裝邏輯
        // 測試變得慢且不穩定
        
        // ? 問題 3: 難以測試邊界情況
        // 如果 Install() 會拋出異常怎辦？
        // 無法讓 Installer 拋出異常來測試異常處理
        
        Assert.Pass("這個測試無法寫");
    }

    // ? 測試有介面的類別 - 簡單！
    [Test]
    public void TestGoodApplication_EASY()
    {
        // ? 優點 1: 可以使用 Mock
        var installer = Substitute.For<IInstaller>();
        var app = new GoodApplication(installer);
        
        // ? 優點 2: 完全控制行為
        installer.ValidateUrl("https://www.example.com").Returns(true);
        installer.Install("https://www.example.com").Returns(true);
        
        // ? 優點 3: 測試任何場景
        var result = app.InstallApplication("https://www.example.com");
        Assert.IsTrue(result);
    }

    // ? 額外優點：可以測試異常情況
    [Test]
    public void TestGoodApplication_CanTestExceptions()
    {
        var installer = Substitute.For<IInstaller>();
        installer.ValidateUrl("test").Returns(true);
        
        // ? 可以讓 Mock 拋出異常
        installer
            .When(x => x.Install("test"))
            .Do(x => { throw new NetworkException("網路連接失敗"); });
        
        var app = new GoodApplication(installer);
        
        // ? 驗證異常被正確拋出
        Assert.That(
            () => app.InstallApplication("test"),
            Throws.TypeOf<NetworkException>()
        );
    }

    // ? 額外優點：可以測試多種情況
    [TestCase("test", false)]
    [TestCase("https://www.google.com", true)]
    public void TestGoodApplication_VariousCases(string url, bool expected)
    {
        var installer = Substitute.For<IInstaller>();
        
        // ? 根據 URL 動態返回不同的結果
        installer.ValidateUrl(url).Returns(url.StartsWith("https://"));
        installer.Install(url).Returns(true);
        
        var app = new GoodApplication(installer);
        var result = app.InstallApplication(url);
        
        Assert.That(result, Is.EqualTo(expected));
    }
}

public class NetworkException : Exception
{
    public NetworkException(string message) : base(message) { }
}
