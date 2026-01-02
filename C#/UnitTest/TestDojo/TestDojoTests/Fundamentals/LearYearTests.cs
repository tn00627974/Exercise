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
        [TestCase(400, true)]
        [TestCase(1900,false)]
        [TestCase(2000,true)]
        [TestCase(2100,false)]
        [TestCase(2200,false)]
        [TestCase(2300,false)]
        [TestCase(2400,true)]
        [TestCase(3333,false)]
        public void IsLeapYear_When1900_ReturnsFalse(int year , bool expected)
        {
            var leapYear = new LeapYear();
            var result = leapYear.IsLeapYear(year);
            Assert.That(result,Is.EqualTo(expected));
        }
    }
}
