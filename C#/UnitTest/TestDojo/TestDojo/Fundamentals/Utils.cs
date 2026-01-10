using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDojo.Fundamentals
{
    public static class Utils
    {
        public static DateTime FirstDayOfNextMonth(DateTime date)
        {
            // There's a bug here, can you find it?
            
            return new DateTime(date.Year, date.Month, 1).AddMonths(1);
        }
    }
}
