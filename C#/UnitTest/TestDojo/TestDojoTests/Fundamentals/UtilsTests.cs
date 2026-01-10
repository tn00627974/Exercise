using TestDojo.Fundamentals;

public class UtilsTests
{
    private UtilsTests _utils;

    [SetUp]
    public void Setup()
    {
        _utils = new UtilsTests();
    }

    [Test]
    public void FistDayOfNextMonth_InputDate_IsCorrectNextMonthFirstDay()
    {
        var date = new DateTime(2024, 1, 15);
        var result = Utils.FirstDayOfNextMonth(date);
        Assert.That(result, Is.EqualTo(new DateTime(2024, 2, 1)));
    }

    [TestCase("2023-05-31","2023-06-01")]
    [TestCase("2024-01-01","2024-02-01")]
    [TestCase("2025-12-31","2026-01-01")]
    public void FistDayOfNextMonth_InputDate_ResultCorrectValue(DateTime date , DateTime expectedDate)
    {
        var result = Utils.FirstDayOfNextMonth(date);
        Assert.That(result,Is.EqualTo(expectedDate));
    }

}
