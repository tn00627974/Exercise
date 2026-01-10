namespace TestDojo.Fundamentals
{
    public class Fibonacci
    {
        public int GetFibonacci(int n)
        {
            if (n < 0) throw new ArgumentOutOfRangeException("n can't be negative!");

            if (n == 0) return 0;
            if (n == 1) return 1;

            return GetFibonacci(n - 1) + GetFibonacci(n - 2); // 0 , 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89
        }
    }
}
