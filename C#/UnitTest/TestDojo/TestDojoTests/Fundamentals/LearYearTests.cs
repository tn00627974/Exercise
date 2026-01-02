using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDojo.Fundamentals;

namespace TestDojoTests.Fundamentals
{
    [TestFixture]
    internal class LearYearTests
    {
        [Test]
        public void IsLeapYear_When2000_Return()
        {
            var leapYear = new LeapYear();
            var result = leapYear.IsLeapYear(1900);
            Assert.AreEqual(false, result);
        }
    }
}
