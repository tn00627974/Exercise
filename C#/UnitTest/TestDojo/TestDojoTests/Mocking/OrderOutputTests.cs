using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using TestDojo.Mocking;

namespace TestDojoTests;

[TestFixture]
public class OrderOutputTests
{
    [SetUp]
    public void Setup()
    {
    }

    [TestCase("OrderId")]
    [TestCase("CustomerId")]
    [TestCase("ProductId")]
    public void Order_Properties_Has_Required_Attribute(string propertyName)
    {
        Assert.That(typeof(Order)
            .GetProperty(propertyName),
            Has.Attribute(typeof(RequiredAttribute)));
    }
}
