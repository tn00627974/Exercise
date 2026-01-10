using TestDojo.Fundamentals;

namespace TestDojoTests.Fundamentals
{
    [TestFixture]
    public class NumbersTests
    {
        public Numbers _numbers;

        [SetUp]
        public void Setup()
        {
            // 3A
            // Arrange
            _numbers = new Numbers();
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        [Test]
        public void Add_When1And2_Return3()
        {
            // Act
            var result = _numbers.Add(1, 2);

            // Assert
            Assert.AreEqual(3,result);
        }

        [Test]
        public async Task DividAsync_WithValidDenominator_ReturnCorrectResult()
        {
            var result = await _numbers.DivideAsync(10, 2);

            Assert.That(result, Is.EqualTo(5)); // New Nunit 3
            Assert.AreEqual(result, 5); // Classic
        }

        [Test]
        public async Task DividAsync_DenominatorIsZero_ThrowsExceptoin()
        {
            var exception = Assert.ThrowsAsync<InvalidOperationException>(async () => await _numbers.DivideAsync(10, 0));
        }
    }
}