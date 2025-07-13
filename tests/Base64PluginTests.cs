using FlowSynx.PluginCore;
using FlowSynx.Plugins.Base64.Services;
using FlowSynx.Plugins.Base64;
using Moq;
using System.Text;

namespace FlowSynx.Plugin.Base64.UnitTests;

public class Base64PluginTests
{
    private readonly Mock<IGuidProvider> _guidProviderMock;
    private readonly Mock<IReflectionGuard> _reflectionGuardMock;
    private readonly Mock<IPluginLogger> _loggerMock;
    private readonly Base64Plugin _plugin;

    public Base64PluginTests()
    {
        _guidProviderMock = new Mock<IGuidProvider>();
        _reflectionGuardMock = new Mock<IReflectionGuard>();
        _loggerMock = new Mock<IPluginLogger>();

        _guidProviderMock.Setup(g => g.NewGuid()).Returns(Guid.Parse("11111111-1111-1111-1111-111111111111"));
        _reflectionGuardMock.Setup(r => r.IsCalledViaReflection()).Returns(false);

        _plugin = new Base64Plugin(_guidProviderMock.Object, _reflectionGuardMock.Object);
    }

    [Fact]
    public async Task Initialize_ShouldSetIsInitialized_WhenCalledWithLogger()
    {
        // Act
        await _plugin.Initialize(_loggerMock.Object);

        // No exceptions means initialized
    }

    [Fact]
    public async Task Initialize_ShouldThrow_WhenCalledViaReflection()
    {
        _reflectionGuardMock.Setup(r => r.IsCalledViaReflection()).Returns(true);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _plugin.Initialize(_loggerMock.Object));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldEncodeData_WhenOperationIsEncode()
    {
        // Arrange
        await _plugin.Initialize(_loggerMock.Object);

        string testData = "FlowSynx";
        string expectedEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(testData));

        var parameters = new PluginParameters
        {
            { "Data", testData },
            { "Operation", "encode" }
        };

        // Act
        var result = await _plugin.ExecuteAsync(parameters, CancellationToken.None);
        var context = Assert.IsType<PluginContext>(result);

        // Assert
        Assert.Equal(expectedEncoded, context.Content);
        Assert.Equal("Encoding", context.Format);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldDecodeData_WhenOperationIsDecode()
    {
        // Arrange
        await _plugin.Initialize(_loggerMock.Object);

        string original = "FlowSynx";
        string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(original));

        var parameters = new PluginParameters
        {
            { "Data", encoded },
            { "Operation", "decode" }
        };

        // Act
        var result = await _plugin.ExecuteAsync(parameters, CancellationToken.None);
        var context = Assert.IsType<PluginContext>(result);

        // Assert
        Assert.Equal(original, context.Content);
        Assert.Equal("Decoding", context.Format);
        Assert.Equal(Encoding.UTF8.GetBytes(original), context.RawData);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldValidateBase64_WhenOperationIsValid()
    {
        // Arrange
        await _plugin.Initialize(_loggerMock.Object);

        string validBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("FlowSynx"));
        var parameters = new PluginParameters
        {
            { "Data", validBase64 },
            { "Operation", "valid" }
        };

        // Act
        var result = await _plugin.ExecuteAsync(parameters, CancellationToken.None);
        var context = Assert.IsType<PluginContext>(result);

        // Assert
        Assert.Equal("True", context.Content);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenOperationIsNotSupported()
    {
        // Arrange
        await _plugin.Initialize(_loggerMock.Object);

        var parameters = new PluginParameters
        {
            { "Data", "FlowSynx" },
            { "Operation", "compress" }
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<NotSupportedException>(() =>
            _plugin.ExecuteAsync(parameters, CancellationToken.None));
        Assert.Contains("Operation 'compress' is not supported", ex.Message);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenCalledViaReflection()
    {
        // Arrange
        await _plugin.Initialize(_loggerMock.Object);
        _reflectionGuardMock.Setup(r => r.IsCalledViaReflection()).Returns(true);

        var parameters = new PluginParameters
        {
            { "Data", "FlowSynx" },
            { "Operation", "encode" }
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _plugin.ExecuteAsync(parameters, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenPluginIsNotInitialized()
    {
        var parameters = new PluginParameters
        {
            { "Data", "FlowSynx" },
            { "Operation", "encode" }
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _plugin.ExecuteAsync(parameters, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenDataIsNull()
    {
        await _plugin.Initialize(_loggerMock.Object);

        var parameters = new PluginParameters
        {
            { "Data", null },
            { "Operation", "encode" }
        };

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _plugin.ExecuteAsync(parameters, CancellationToken.None));
        Assert.Contains("Input data cannot be null", ex.Message);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenDataIsListOfPluginContext()
    {
        await _plugin.Initialize(_loggerMock.Object);

        var parameters = new PluginParameters
        {
            { "Data", new List<PluginContext>() },
            { "Operation", "encode" }
        };

        var ex = await Assert.ThrowsAsync<NotSupportedException>(() =>
            _plugin.ExecuteAsync(parameters, CancellationToken.None));
        Assert.Contains("List of PluginContext is not supported", ex.Message);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldSupportCancellation()
    {
        await _plugin.Initialize(_loggerMock.Object);

        var parameters = new PluginParameters
        {
            { "Data", "FlowSynx" },
            { "Operation", "encode" }
        };

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _plugin.ExecuteAsync(parameters, cts.Token));
    }
}