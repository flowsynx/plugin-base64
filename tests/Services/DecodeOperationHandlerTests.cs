using FlowSynx.PluginCore;
using Moq;
using FlowSynx.Plugins.Base64.Services;
using System.Text;

namespace FlowSynx.Plugin.Base64.UnitTests.Services;

public class DecodeOperationHandlerTests
{
    private readonly Mock<IGuidProvider> _guidProviderMock;
    private readonly DecodeOperationHandler _handler;

    public DecodeOperationHandlerTests()
    {
        _guidProviderMock = new Mock<IGuidProvider>();
        _handler = new DecodeOperationHandler(_guidProviderMock.Object);
    }

    [Fact]
    public void Handle_ShouldDecodeBase64Content_WhenValidBase64IsProvided()
    {
        // Arrange
        var expectedGuid = Guid.NewGuid();
        _guidProviderMock.Setup(g => g.NewGuid()).Returns(expectedGuid);

        var originalText = "Hello, World!";
        var base64Content = Convert.ToBase64String(Encoding.UTF8.GetBytes(originalText));
        var context = new PluginContext("file", "type") { Content = base64Content };

        // Act
        var result = _handler.Handle(context) as PluginContext;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedGuid.ToString(), result.Id);
        Assert.Equal("Data", result.SourceType);
        Assert.Equal("Decoding", result.Format);
        Assert.Equal(originalText, result.Content);
        Assert.Equal(Encoding.UTF8.GetBytes(originalText), result.RawData);
    }

    [Fact]
    public void Handle_ShouldThrowArgumentException_WhenContentIsNull()
    {
        // Arrange
        var context = new PluginContext("file", "type") { Content = null };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _handler.Handle(context));
        Assert.Equal("Data containing Base64 string is required for decoding.", exception.Message);
    }

    [Fact]
    public void Handle_ShouldThrowFormatException_WhenContentIsNotValidBase64()
    {
        // Arrange
        var invalidBase64 = "NotBase64@@";
        var context = new PluginContext("file", "type") { Content = invalidBase64 };

        // Act & Assert
        var exception = Assert.Throws<FormatException>(() => _handler.Handle(context));
        Assert.Contains("The input is not a valid Base-64", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}