using TestDojo.Fundamentals;


namespace TestDojoTests.Fundamentals
{
    [TestFixture]
    public class FormatterTests
    {
        [Test]
        public void FormatBold_WrapsInputInBoldTags()
        {
            var formatter = new Formatter();
            string boldText = formatter.FormatBold("test");
            Assert.That(boldText, Is.EqualTo("<b>test</b>"));
        }
    }
}
