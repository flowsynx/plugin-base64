using FlowSynx.PluginCore;

namespace FlowSynx.Plugins.Base64.Services;

internal interface IBase64OperationHandler
{
    object Handle(PluginContext context);
}