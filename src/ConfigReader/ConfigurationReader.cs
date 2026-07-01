using MongoDB.Driver;

namespace ConfigReader;

public class ConfigurationReader : IDisposable
{
    private readonly string _applicationName;
    private readonly IMongoCollection<ConfigRecord> _collection;
    private readonly Timer _timer;
    private readonly object _lock = new();

    private Dictionary<string, ConfigRecord> _cache = new();

    public ConfigurationReader(string applicationName, string connectionString, int refreshTimerIntervalInMs = 60000)
    {
        _applicationName = applicationName;

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase("DynamicConfigDb");
        _collection = database.GetCollection<ConfigRecord>("ConfigRecords");

        // ilk yukleme hemen bitsin ki new'den sonra GetValue direkt calisabilsin
        LoadConfigurationsAsync().GetAwaiter().GetResult();

        _timer = new Timer(async _ => await LoadConfigurationsAsync(), null, refreshTimerIntervalInMs, refreshTimerIntervalInMs);
    }

    public T GetValue<T>(string key)
    {
        ConfigRecord? record;
        lock (_lock)
        {
            _cache.TryGetValue(key, out record);
        }

        if (record is null)
        {
            throw new KeyNotFoundException($"'{key}' anahtarına ait aktif bir konfigurasyon kaydı bulunamadı ({_applicationName}).");
        }

        return ConvertValue<T>(record.Value);
    }

    private static T ConvertValue<T>(string rawValue)
    {
        var targetType = typeof(T);

        if (targetType == typeof(bool))
        {
            var boolResult = rawValue == "1" || rawValue.Equals("true", StringComparison.OrdinalIgnoreCase);
            return (T)(object)boolResult;
        }

        var converted = Convert.ChangeType(rawValue, targetType);
        return (T)converted;
    }

    private async Task LoadConfigurationsAsync()
    {
        try
        {
            var filter = Builders<ConfigRecord>.Filter.And(
                Builders<ConfigRecord>.Filter.Eq(x => x.ApplicationName, _applicationName),
                Builders<ConfigRecord>.Filter.Eq(x => x.IsActive, true));

            var records = await _collection.Find(filter).ToListAsync();

            var newCache = new Dictionary<string, ConfigRecord>();
            foreach (var record in records)
            {
                newCache[record.Name] = record;
            }

            lock (_lock)
            {
                _cache = newCache;
            }
        }
        catch
        {
            // Mongo'ya ulasilamadiysa son basarili cache ile devam eder
        }
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}
