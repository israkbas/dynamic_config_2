using System.Globalization;

namespace ConfigReader;

public static class ConfigValueConverter
{
    public static T Convert<T>(string rawValue)
    {
        var targetType = typeof(T);

        if (targetType == typeof(bool))
        {
            var boolResult = rawValue == "1" || rawValue.Equals("true", StringComparison.OrdinalIgnoreCase);
            return (T)(object)boolResult;
        }

        var converted = System.Convert.ChangeType(rawValue, targetType, CultureInfo.InvariantCulture);
        return (T)converted;
    }
}
