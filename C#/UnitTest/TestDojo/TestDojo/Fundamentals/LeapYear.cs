namespace TestDojo.Fundamentals
{
    public class LeapYear
    {
        // google: leap year formula 
        // There's a bug in this code. Can you find it?
        

        public bool IsLeapYear(int year) {

            // 手寫判斷
            if (year % 100 == 0 && year % 400 != 0) return false;
            if (year % 4 == 0) return true;
            return false;

            // 系統內建判斷
            //bool isLear = DateTime.IsLeapYear(year);
            //return isLear;
        }
    }
}
