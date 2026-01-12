using TestDojo.Mocking;
using NSubstitute;

namespace TestDojoTests.Fundamentals;

public class NewApplicationTests
{
    [SetUp]
    public void Setup()
    {
    }

    [TestCase("test", false)]  // 無效 URL → 返回 False
    [TestCase("https://www.google.com", true)]  // 有效 URL → 返回 True
    public void InstallApplication_WithUrlValidation_ReturnsCorrectResult(string url, bool expected)
    {
        // Arrange
        var installer = Substitute.For<IInstaller>();
        installer.ValidateUrl(url).Returns(url == "https://www.google.com");
        installer.Install(url).Returns(true);  // 模擬安裝成功
        var app = new NewApplication(installer);

        // Act 
        var result = app.InstallApplication(url);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void InstallApplication_WhenInstallExecption_CatchExecption()
    {
        // Arrange
        var installer = Substitute.For<IInstaller>();

        // Act 
        installer.ValidateUrl("test").Returns(true);
        installer
            .When(x => x.Install("test"))
            .Do(x => { throw new Exception(); });

        var app = new NewApplication(installer);

        // Assert
        Assert.That(() => app.InstallApplication("test"), Throws.Exception);
    }
}
