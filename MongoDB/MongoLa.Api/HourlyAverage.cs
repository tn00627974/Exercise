namespace MongoLa.Api;

/// <summary>
/// $group 之後的形狀 —— 不再是 Reading，沒有 Id / DeviceId。
/// </summary>
public class HourlyAverage
{
    public DateTime Hour { get; set; }
    public double AvgPowerKW { get; set; }
    public int Count { get; set; }
}
