using TestDojo.Mocking;
using System.IO;

namespace TestDojoTests;

public class JournalTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void FileProcess_When_FileNotExists_ReturnWriteText()
    {
        // Arrange
        var journal = new Journal();
        // Act
        journal.FileProcess("test.txt");
        // Assert
        Assert.That(File.Exists("log.txt"), Is.True);
        var logContent = File.ReadAllText("log.txt");
        Assert.That(logContent, Does.Contain("File not found"));
    }
}
