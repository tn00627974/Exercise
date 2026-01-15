using NSubstitute;
using TestDojo.Mocking;

namespace TestDojoTests.Mocking;

public class PhotoApiCallTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task GetPhotoUrlAsync_ValidId_ReturnsUrl()
    {
        // Arrange
        var mockHttpClient = Substitute.For<IPhotoHttpClient>();
        var expectedUrl = "https://via.placeholder.com/600/92c952";
        mockHttpClient.GetAsync(Arg.Any<string>())!.Returns(Task.FromResult(new Photo 
        { 
            Id = 1,
            AlbumId = 1,
            Title = "accusamus beatae ad facilis cum similique qui sunt",
            Url = expectedUrl,
            ThumbnailUrl = "https://via.placeholder.com/150/92c952"
        }));
        var photoApiCall = new PhotoApiCall(mockHttpClient);

        // Act
        var result = await photoApiCall.GetPhotoUrlAsync(1);

        // Assert
        Assert.That(result, Is.EqualTo(expectedUrl));
    }

    [Test]
    public async Task GetPhotoUrlAsync_NullPhoto_ReturnsEmptyString()
    {
        // Arrange
        var mockHttpClient = Substitute.For<IPhotoHttpClient>();
        mockHttpClient.GetAsync(Arg.Any<string>())!.Returns(Task.FromResult<Photo?>(null));
        var photoApiCall = new PhotoApiCall(mockHttpClient);

        // Act
        var result = await photoApiCall.GetPhotoUrlAsync(1);

        // Assert
        Assert.That(result, Is.EqualTo(string.Empty));
    }

    [Test]
    public async Task GetPhotoUrlAsync_CallsClientWithCorrectUrl()
    {
        // Arrange
        var mockHttpClient = Substitute.For<IPhotoHttpClient>();
        mockHttpClient.GetAsync(Arg.Any<string>())!.Returns(Task.FromResult(new Photo 
        { 
            Url = "https://via.placeholder.com/600/92c952"
        }));
        var photoApiCall = new PhotoApiCall(mockHttpClient);
        var photoId = 5;

        // Act
        await photoApiCall.GetPhotoUrlAsync(photoId);

        // Assert
        await mockHttpClient.Received(1).GetAsync($"https://jsonplaceholder.typicode.com/photos/{photoId}");
    }
}
