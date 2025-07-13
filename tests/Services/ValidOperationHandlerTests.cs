using FlowSynx.PluginCore;
using Moq;
using FlowSynx.Plugins.Base64.Services;

namespace FlowSynx.Plugin.Base64.UnitTests.Services;

public class ValidOperationHandlerTests
{
    private readonly Mock<IGuidProvider> _guidProviderMock;
    private readonly ValidOperationHandler _handler;

    public ValidOperationHandlerTests()
    {
        _guidProviderMock = new Mock<IGuidProvider>();
        _handler = new ValidOperationHandler(_guidProviderMock.Object);
    }

    [Fact]
    public void Handle_ShouldReturnTrue_WhenValidBase64IsProvided()
    {
        // Arrange
        var expectedGuid = Guid.NewGuid();
        _guidProviderMock.Setup(g => g.NewGuid()).Returns(expectedGuid);

        var validBase64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("Test string"));
        var context = new PluginContext("file", "type") { Content = validBase64 };

        // Act
        var result = _handler.Handle(context) as PluginContext;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedGuid.ToString(), result.Id);
        Assert.Equal("Data", result.SourceType);
        Assert.Equal("Encoding", result.Format);
        Assert.Equal("True", result.Content);
    }

    [Fact]
    public void Handle_ShouldReturnFalse_WhenInvalidBase64IsProvided()
    {
        // Arrange
        var expectedGuid = Guid.NewGuid();
        _guidProviderMock.Setup(g => g.NewGuid()).Returns(expectedGuid);

        var invalidBase64 = "Invalid@@Base64";
        var context = new PluginContext("file", "type") { Content = invalidBase64 };

        // Act
        var result = _handler.Handle(context) as PluginContext;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedGuid.ToString(), result.Id);
        Assert.Equal("Data", result.SourceType);
        Assert.Equal("Encoding", result.Format);
        Assert.Equal("False", result.Content);
    }

    [Fact]
    public void Handle_ShouldReturnFalse_WhenBase64StringIsEmpty()
    {
        // Arrange
        var expectedGuid = Guid.NewGuid();
        _guidProviderMock.Setup(g => g.NewGuid()).Returns(expectedGuid);

        var context = new PluginContext("file", "type") { Content = "" };

        // Act
        var result = _handler.Handle(context) as PluginContext;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedGuid.ToString(), result.Id);
        Assert.Equal("Data", result.SourceType);
        Assert.Equal("Encoding", result.Format);
        Assert.Equal("False", result.Content);
    }

    [Fact]
    public void Handle_ShouldThrowArgumentException_WhenContentIsNull()
    {
        // Arrange
        var context = new PluginContext("file", "type") { Content = null };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _handler.Handle(context));
        Assert.Equal("Data containing Base64 string is required for validation.", exception.Message);
    }
}