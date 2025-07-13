using FlowSynx.PluginCore;
using FlowSynx.Plugins.Base64;
using FlowSynx.Plugins.Base64.Services;
using Moq;
using System.Text;

namespace FlowSynx.Plugin.Base64.UnitTests.Services;

public class EncodeOperationHandlerTests
{
    private readonly Mock<IGuidProvider> _guidProviderMock;
    private readonly EncodeOperationHandler _handler;

    public EncodeOperationHandlerTests()
    {
        _guidProviderMock = new Mock<IGuidProvider>();
        _handler = new EncodeOperationHandler(_guidProviderMock.Object);
    }

    [Fact]
    public void Handle_ShouldEncodeRawData_WhenRawDataIsProvided()
    {
        // Arrange
        var expectedGuid = Guid.NewGuid();
        _guidProviderMock.Setup(g => g.NewGuid()).Returns(expectedGuid);

        var rawData = Encoding.UTF8.GetBytes("Test Data");
        var context = new PluginContext("file", "type") { RawData = rawData };

        // Act
        var result = _handler.Handle(context) as PluginContext;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedGuid.ToString(), result.Id);
        Assert.Equal("Data", result.SourceType);
        Assert.Equal("Encoding", result.Format);
        var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(result.Content));
        Assert.Equal("Test Data", decoded);
    }

    [Fact]
    public void Handle_ShouldEncodeContent_WhenContentIsProvided()
    {
        // Arrange
        var expectedGuid = Guid.NewGuid();
        _guidProviderMock.Setup(g => g.NewGuid()).Returns(expectedGuid);

        var content = "Sample String";
        var context = new PluginContext("file", "type") { Content = content };

        // Act
        var result = _handler.Handle(context) as PluginContext;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedGuid.ToString(), result.Id);
        Assert.Equal("Data", result.SourceType);
        Assert.Equal("Encoding", result.Format);
        var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(result.Content));
        Assert.Equal("Sample String", decoded);
    }

    [Fact]
    public void Handle_ShouldThrowInvalidDataException_WhenNoDataProvided()
    {
        // Arrange
        var context = new PluginContext("file", "type");

        // Act & Assert
        var exception = Assert.Throws<InvalidDataException>(() => _handler.Handle(context));
        Assert.Equal(Resources.TheEnteredDataIsInvalid, exception.Message);
    }
}