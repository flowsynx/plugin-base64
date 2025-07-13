using FlowSynx.PluginCore;
using FlowSynx.PluginCore.Extensions;
using FlowSynx.Plugins.Base64.Models;
using FlowSynx.Plugins.Base64.Services;

namespace FlowSynx.Plugins.Base64;

public class Base64Plugin : IPlugin
{
    private readonly IGuidProvider _guidProvider;
    private readonly IReflectionGuard _reflectionGuard;
    private IPluginLogger? _logger;
    private bool _isInitialized;

    public Base64Plugin() : this(new GuidProvider(), new DefaultReflectionGuard()) { }

    internal Base64Plugin(IGuidProvider guidProvider, IReflectionGuard reflectionGuard)
    {
        _guidProvider = guidProvider ?? throw new ArgumentNullException(nameof(guidProvider));
        _reflectionGuard = reflectionGuard ?? throw new ArgumentNullException(nameof(reflectionGuard));
    }

    public PluginMetadata Metadata => new()
    {
        Id = Guid.Parse("8f3e97d1-f1a4-40df-9241-644097a56382"),
        Name = "Base64",
        CompanyName = "FlowSynx",
        Description = Resources.PluginDescription,
        Version = new Version(1, 0, 0),
        Category = PluginCategory.Data,
        Authors = new List<string> { "FlowSynx" },
        Copyright = "© FlowSynx. All rights reserved.",
        Icon = "flowsynx.png",
        ReadMe = "README.md",
        RepositoryUrl = "https://github.com/flowsynx/plugin-base64",
        ProjectUrl = "https://flowsynx.io",
        Tags = new List<string>() { "flowSynx", "base64", "data", "data-platform", "encoding", "decoding" },
        MinimumFlowSynxVersion = new Version(1, 1, 1),
    };

    public PluginSpecifications? Specifications { get; set; }

    public Type SpecificationsType => typeof(Base64PluginSpecifications);

    private Dictionary<string, IBase64OperationHandler> OperationMap => new(StringComparer.OrdinalIgnoreCase)
    {
        ["encode"] = new EncodeOperationHandler(_guidProvider),
        ["decode"] = new DecodeOperationHandler(_guidProvider),
        ["valid"] = new ValidOperationHandler(_guidProvider)
    };

    public IReadOnlyCollection<string> SupportedOperations => OperationMap.Keys;

    public Task Initialize(IPluginLogger logger)
    {
        if (_reflectionGuard.IsCalledViaReflection())
            throw new InvalidOperationException(Resources.ReflectionBasedAccessIsNotAllowed);

        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;
        _isInitialized = true;
        return Task.CompletedTask;
    }

    public Task<object?> ExecuteAsync(PluginParameters parameters, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_reflectionGuard.IsCalledViaReflection())
            throw new InvalidOperationException(Resources.ReflectionBasedAccessIsNotAllowed);

        if (!_isInitialized)
            throw new InvalidOperationException($"Plugin '{Metadata.Name}' v{Metadata.Version} is not initialized.");

        var inputParameter = parameters.ToObject<InputParameter>();
        if (!OperationMap.TryGetValue(inputParameter.Operation, out var handler))
            throw new NotSupportedException($"Operation '{inputParameter.Operation}' is not supported.");
        
        var context = ParseDataToContext(inputParameter.Data);
        return Task.FromResult(handler.Handle(context));
    }

    private PluginContext ParseDataToContext(object? data)
    {
        if (data is null)
            throw new ArgumentNullException(nameof(data), "Input data cannot be null.");

        return data switch
        {
            PluginContext singleContext => singleContext,
            IEnumerable<PluginContext> => throw new NotSupportedException("List of PluginContext is not supported."),
            string strData => new PluginContext(_guidProvider.NewGuid().ToString(), "Data") { Content = strData },
            _ => throw new NotSupportedException("Unsupported input data format.")
        };
    }
}