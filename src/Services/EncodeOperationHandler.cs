using FlowSynx.PluginCore;
using System.Text;

namespace FlowSynx.Plugins.Base64.Services;

internal class EncodeOperationHandler : IBase64OperationHandler
{
    private readonly IGuidProvider _guidProvider;

    public EncodeOperationHandler(IGuidProvider guidProvider)
    {
        _guidProvider = guidProvider;
    }

    public object Handle(PluginContext context)
    {
        byte[] dataToEncode;

        if (context.RawData is not null)
            dataToEncode = context.RawData;
        else if (context.Content is not null)
            dataToEncode = Encoding.UTF8.GetBytes(context.Content);
        else
            throw new InvalidDataException(Resources.TheEnteredDataIsInvalid);

        string filename = $"{_guidProvider.NewGuid()}";
        string encodedString = Convert.ToBase64String(dataToEncode);
        return new PluginContext(filename, "Data")
        {
            Format = "Encoding",
            Content = encodedString
        };
    }
}