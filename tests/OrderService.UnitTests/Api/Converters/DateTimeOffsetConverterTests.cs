using System.Text.Json;
using FluentAssertions;
using OrderService.Api.Converters;

namespace OrderService.UnitTests.Api.Converters;

public sealed class DateTimeOffsetConverterTests
{
    private readonly JsonSerializerOptions _options;

    public DateTimeOffsetConverterTests()
    {
        _options = new JsonSerializerOptions();
        _options.Converters.Add(new DateTimeOffsetConverter());
    }

    [Fact]
    public void Write_ShouldFormatDateTimeOffsetAsUtcString()
    {
        // Arrange
        var testObject = new { CreatedAt = new DateTimeOffset(2025, 3, 7, 12, 0, 0, TimeSpan.Zero) };

        // Act
        var json = JsonSerializer.Serialize(testObject, _options);

        // Assert
        json.Should().Contain("\"CreatedAt\":\"2025-03-07T12:00:00Z\"");
    }

    [Fact]
    public void Write_ShouldConvertNonUtcToUtc()
    {
        // Arrange
        var testObject = new { CreatedAt = new DateTimeOffset(2025, 3, 7, 12, 0, 0, TimeSpan.FromHours(2)) };

        // Act
        var json = JsonSerializer.Serialize(testObject, _options);

        // Assert
        json.Should().Contain("\"CreatedAt\":\"2025-03-07T10:00:00Z\"");
    }

    [Fact]
    public void Read_ShouldParseUtcStringToDateTimeOffset()
    {
        // Arrange
        var json = "{\"CreatedAt\":\"2025-03-07T12:00:00Z\"}";

        // Act
        var result = JsonSerializer.Deserialize<TestDto>(json, _options);

        // Assert
        result!.CreatedAt.Should().Be(new DateTimeOffset(2025, 3, 7, 12, 0, 0, TimeSpan.Zero));
    }

    [Fact]
    public void Read_ShouldParseOffsetStringToUtcDateTimeOffset()
    {
        // Arrange
        var json = "{\"CreatedAt\":\"2025-03-07T12:00:00+02:00\"}";

        // Act
        var result = JsonSerializer.Deserialize<TestDto>(json, _options);

        // Assert
        result!.CreatedAt.Should().Be(new DateTimeOffset(2025, 3, 7, 10, 0, 0, TimeSpan.Zero));
    }

    private sealed class TestDto
    {
        public DateTimeOffset CreatedAt { get; set; }
    }
}
