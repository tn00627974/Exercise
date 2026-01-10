using TestDojo.Fundamentals;

namespace TestDojoTests;

public class FibonacciTests
{
    private Fibonacci _fibonacci;
    [SetUp]
    public void Setup()
    {
        _fibonacci = new Fibonacci();
    }

    [Test]
    public void GetFibonacci_WithNegativeNumber_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _fibonacci.GetFibonacci(-1));
    }
    [Test]
    public void GetFibonacci_WithZero_ReturnsZero()
    {
        var result = _fibonacci.GetFibonacci(0);
        Assert.That(result, Is.EqualTo(0));
    }
    [Test]
    public void GetFibonacci_WithOne_ReturnsOne()
    {
        var result = _fibonacci.GetFibonacci(1);
        Assert.That(result, Is.EqualTo(1));
    }

    [TestCase(2, 1)]
    [TestCase(3, 2)]
    [TestCase(4, 3)]
    [TestCase(5, 5)]
    [TestCase(6, 8)]
    [TestCase(10, 55)]
    public void GetFibonacci_WithValidNumber_ReturnsCorrectFibonacciValue(int n, int expected)
    {
        int result = _fibonacci.GetFibonacci(n);
        Assert.That(result, Is.EqualTo(expected));
    }
}
