using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using TestDojo.Fundamentals;

namespace TestDojoTests.Fundamentals
{
    [TestFixture]
    public class LogEventTests
    {
        [Test]
        public void LogEvent_When_EventTrigger()
        {
            var logger = new LogEvent(NullLogger.Instance);
            logger.LogError("tesing");

            Guid myGuid = Guid.Empty;
            logger.ErrorLogged += (sender, guid) => { myGuid = guid; };

            Assert.That(myGuid , Is.EqualTo(Guid.Empty));
        }
    }

    
}
