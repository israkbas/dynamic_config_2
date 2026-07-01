using ConfigReader;

var connectionString = "mongodb://localhost:27018";
using var reader = new ConfigurationReader("SERVICE-A", connectionString, 5000);

while (true)
{
    var siteName = reader.GetValue<string>("SiteName");
    var maxItemCount = reader.GetValue<int>("MaxItemCount");

    Console.WriteLine($"[SERVICE-A] SiteName={siteName} MaxItemCount={maxItemCount}");

    await Task.Delay(2000);
}
