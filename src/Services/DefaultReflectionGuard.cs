using FlowSynx.PluginCore.Helpers;

namespace FlowSynx.Plugins.Base64.Services;

internal class DefaultReflectionGuard : IReflectionGuard
{
    public bool IsCalledViaReflection() => ReflectionHelper.IsCalledViaReflection();
}