using TestDojo.Fundamentals;

namespace TestDojoTests.Fundamentals
{
    [TestFixture]
    public class NumbersTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        [Test]
        public void Add_When1And2_Return3()
        {
            // 3A
            // Arrange
            var numbers = new Numbers();

            // Act
            var result = numbers.Add(1, 2);

            // Assert
            Assert.AreEqual(3,result);
        }
    }
}