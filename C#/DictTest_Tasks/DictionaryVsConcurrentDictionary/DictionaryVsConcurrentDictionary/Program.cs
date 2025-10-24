using System.Collections.Concurrent;
using System.Diagnostics;

class Program
{
    private static Dictionary<int, string> normalDict = new();
    private static ConcurrentDictionary<int, string> concurrentDict = new();

    static async Task Main()
    {
        Console.WriteLine("測試 1：普通 Dictionary（非執行緒安全）");
        await TestDictionary(normalDict, useConcurrent: false);

        Console.WriteLine("測試 2：ConcurrentDictionary（執行緒安全）");
        await TestDictionary(concurrentDict, useConcurrent: true);
    }

    static async Task TestDictionary(object dict, bool useConcurrent)
    {
        var tasks = new List<Task>();
        var stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < 100; i++)
        {
            int threadId = i;
            tasks.Add(Task.Run(() =>
            {
                for (int j = 0; j < 1000; j++)
                {
                    string value = $"Thread-{threadId}-Value-{j}";
                    int key = j;

                    try
                    {
                        if (useConcurrent)
                        {
                            ((ConcurrentDictionary<int, string>)dict)[key] = value;
                            //Console.WriteLine($"ConcurrentDictionary : {value}");
                        }
                        else
                        {
                            ((Dictionary<int, string>)dict)[key] = value; // 會出錯
                            //Console.WriteLine($"Dictionary : {value}");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }

                }
            }));
    }

        try
        {
            await Task.WhenAll(tasks);
            stopwatch.Stop();
            Console.WriteLine($"完成，耗時: {stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine($"資料筆數: " +
                $"{(useConcurrent ? 
                ((ConcurrentDictionary<int, string>)dict).Count :  // 顯示資料筆數 (執行緒安全的 ConcurrentDictionary 數量正確1000 )
                ((Dictionary<int, string>)dict).Count)}"); // 顯示資料筆數 (非執行緒安全的 Dictionary 數量會不正確 940, 1007 , 1015 等)
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Console.WriteLine($"發生例外: {ex.Message}");
        }
    }
}
