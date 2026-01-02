using TestDojo.Fundamentals;

namespace TestDojoTests;

[TestFixture]
public class FizzBuzzTests
{

    [Test]
    public void GetFizzBuzzTest()
    {
        var fizzBuzz = new FizzBuzz();
        var result = fizzBuzz.GetFizzBuzz(15);
        int count = fizzBuzz.GetFizzBuzz(15).Count;
        Assert.AreEqual(15, count);
        Assert.That(result[3 - 1], Is.EqualTo("Fizz"));
        Assert.That(result[5 - 1], Is.EqualTo("Buzz"));
        Assert.That(result[15 - 1], Is.EqualTo("FizzBuzz"));
        Assert.That(result[2-1], Is.EqualTo("2"));


    }

}
