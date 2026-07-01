using ConfigReader;

var connectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING") ?? "mongodb://localhost:27018";
using var reader = new ConfigurationReader("SERVICE-A", connectionString, 5000);

while (true)
{
    var siteName = reader.GetValue<string>("SiteName");
    var maxItemCount = reader.GetValue<int>("MaxItemCount");

    Console.WriteLine($"[SERVICE-A] SiteName={siteName} MaxItemCount={maxItemCount}");

    await Task.Delay(2000);
}
