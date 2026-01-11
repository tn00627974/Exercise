using TestDojo.Mocking;
using NSubstitute;

namespace TestDojoTests.Fundamentals;

public class NewApplicationTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void InstallApplication_ValidUrl_ReturnsFalse()
    {
        // Arrange
        var installer = Substitute.For<IInstaller>();
        var app = new NewApplication(installer);

        // Act 
        var result = app.InstallApplication("test");

        // Assert
        Assert.IsFalse(result);
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
