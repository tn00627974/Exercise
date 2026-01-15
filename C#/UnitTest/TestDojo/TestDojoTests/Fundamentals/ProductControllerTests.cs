using TestDojo.Fundamentals;
namespace TestDojoTests.Fundamentals;

//[TestFixture]
public class ProductControllerTests
{

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void GetProductBy_WhenIdZero_ReturnNotFound()
    {
        // Arrange
        var controller = new ProductController();

        // Act
        var result = controller.GetProduct(0);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFound>());
        Assert.That(result.GetStatusCode(), Is.EqualTo(404));
    }

    [Test]
    public void GetProductBy_WhenIdOne_ReturnOk()
    {
        // Arrange
        var controller = new ProductController();

        // Act
        var result = controller.GetProduct(1);

        // Assert
        Assert.That(result, Is.InstanceOf<Ok>());
        Assert.That(result.GetStatusCode(), Is.EqualTo(200));
    }

    [Test]
    public void GetProductBy_WhenIdNegative_ReturnBadRequest()
    {
        // Arrange
        var controller = new ProductController();

        // Act
        var result = controller.GetProduct(-1);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequest>());
        Assert.That(result.GetStatusCode(), Is.EqualTo(500));
    }
}
