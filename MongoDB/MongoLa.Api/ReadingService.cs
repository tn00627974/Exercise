using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Linq;

namespace MongoLa.Api;

public class ReadingService
{
    private readonly IMongoCollection<Reading> _readings;

    public ReadingService(IOptions<DatabaseSettings> settings, IMongoClient client)
    {
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _readings = database.GetCollection<Reading>(settings.Value.CollectionName);
    }
    // 每小時平均發電量 —— 對應 Compass $match -> $group -> $sort
    public async Task<List<HourlyAverage>> GetHourlyAverageAsync(string deviceId)
    {
        return await _readings.Aggregate()
            .Match(r => r.DeviceId == deviceId)
            .Group(
                r => r.TimeStamp.Truncate(DateTimeUnit.Hour),
                g => new HourlyAverage
                {
                    Hour       = g.Key,
                    AvgPowerKW = g.Average(r => r.PowerKW),
                    Count      = g.Count()
                })
            .SortBy(h => h.Hour)
            .ToListAsync();
    }

    public async Task<List<Reading>> GetLatestAsync(string deviceId, int limit = 10)
    {
        var result = _readings.Find(r => r.DeviceId == deviceId)
                                .SortByDescending(r => r.TimeStamp)
                                .Limit(limit);

        return await result.ToListAsync();
    }

    public async Task<List<Reading>> GetLatestAsync1(string deviceId, int limit = 10)
    {
        var filter = Builders<Reading>.Filter.Eq(r => r.DeviceId, deviceId);
        var result = _readings.Find(filter)
                                .SortByDescending(r => r.TimeStamp)
                                .Limit(limit);

        return await result.ToListAsync();
    }

}
