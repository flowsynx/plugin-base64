using FlowSynx.PluginCore;
using System.Text;

namespace FlowSynx.Plugins.Base64.Services;

internal class DecodeOperationHandler : IBase64OperationHandler
{
    private readonly IGuidProvider _guidProvider;

    public DecodeOperationHandler(IGuidProvider guidProvider)
    {
        _guidProvider = guidProvider;
    }

    public object Handle(PluginContext context)
    {
        string base64Input = context.Content ?? 
            throw new ArgumentException("Data containing Base64 string is required for decoding.");

        string filename = $"{_guidProvider.NewGuid()}";
        byte[] decodedBytes = Convert.FromBase64String(base64Input);
        string decodedText = Encoding.UTF8.GetString(decodedBytes);

        return new PluginContext(filename, "Data")
        {
            Format = "Decoding",
            Content = decodedText,
            RawData = decodedBytes
        };
    }
}