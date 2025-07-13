namespace FlowSynx.Plugins.Base64.Services;

public interface IReflectionGuard
{
    bool IsCalledViaReflection();
}