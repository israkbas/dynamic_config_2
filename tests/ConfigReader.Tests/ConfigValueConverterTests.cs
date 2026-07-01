namespace ConfigReader.Tests;

public class ConfigValueConverterTests
{
    [Fact]
    public void Convert_StringValue_ReturnsSameString()
    {
        var result = ConfigValueConverter.Convert<string>("soty.io");

        Assert.Equal("soty.io", result);
    }

    [Fact]
    public void Convert_IntValue_ReturnsInt()
    {
        var result = ConfigValueConverter.Convert<int>("50");

        Assert.Equal(50, result);
    }

    [Fact]
    public void Convert_DoubleValue_ReturnsDouble()
    {
        var result = ConfigValueConverter.Convert<double>("3.14");

        Assert.Equal(3.14, result);
    }

    [Theory]
    [InlineData("1", true)]
    [InlineData("0", false)]
    [InlineData("true", true)]
    [InlineData("false", false)]
    public void Convert_BoolValue_ParsesCorrectly(string rawValue, bool expected)
    {
        var result = ConfigValueConverter.Convert<bool>(rawValue);

        Assert.Equal(expected, result);
    }
}
