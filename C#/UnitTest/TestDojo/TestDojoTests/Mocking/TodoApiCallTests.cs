using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using TestDojo.Mocking;

namespace TestDojoTests;

public class TodoApiCallTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task GetTodoTitle_ValidId_ReturnsTitle()
    {
        // Arrange
        var mockHttpClient = Substitute.For<IMyHttpClient>();
        mockHttpClient.GetAsync(Arg.Any<string>())!.Returns(Task.FromResult(new Todo { Title = "Test" }));
        var todoApiCall = new TodoApiCall(mockHttpClient);

        // Act
        var result = await todoApiCall.GetTodoTitleAsync(1);

        // Assert
        Assert.That(result, Is.EqualTo("Test"));
    }
}
