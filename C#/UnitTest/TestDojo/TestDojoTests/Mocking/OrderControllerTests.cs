using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TestDojo.Mocking;

namespace TestDojoTests.Fundamentals;

[TestFixture]
public class OrderControllerTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Order_Properties_Has_Required_Attribute()
    {
        Assert.That(typeof(OrderController)
            .GetMethod(nameof(OrderController.Post)),
            Has.Attribute(typeof(HttpPostAttribute)));

        Assert.That(typeof(OrderController)
            .GetMethod(nameof(OrderController.Post)),
            Has.Attribute(typeof(RouteAttribute)).Property("Template").Contain("api/order"));
    }
}
