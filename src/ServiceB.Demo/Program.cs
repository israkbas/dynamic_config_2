using ConfigReader;

var connectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING") ?? "mongodb://localhost:27018";
using var reader = new ConfigurationReader("SERVICE-B", connectionString, 5000);

while (true)
{
    var isBasketEnabled = reader.GetValue<bool>("IsBasketEnabled");

    Console.WriteLine($"[SERVICE-B] IsBasketEnabled={isBasketEnabled}");

    await Task.Delay(2000);
}
