using ConfigReader;

var connectionString = "mongodb://localhost:27018";
using var reader = new ConfigurationReader("SERVICE-B", connectionString, 5000);

while (true)
{
    var isBasketEnabled = reader.GetValue<bool>("IsBasketEnabled");

    Console.WriteLine($"[SERVICE-B] IsBasketEnabled={isBasketEnabled}");

    await Task.Delay(2000);
}
