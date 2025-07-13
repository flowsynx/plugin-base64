using FlowSynx.PluginCore;

namespace FlowSynx.Plugins.Base64.Services;

internal class ValidOperationHandler : IBase64OperationHandler
{
    private readonly IGuidProvider _guidProvider;

    public ValidOperationHandler(IGuidProvider guidProvider)
    {
        _guidProvider = guidProvider;
    }

    public object Handle(PluginContext context)
    {
        string base64ToValidate = context.Content ?? 
            throw new ArgumentException("Data containing Base64 string is required for validation.");

        string filename = $"{_guidProvider.NewGuid()}";
        bool isValid = IsBase64String(base64ToValidate);

        return new PluginContext(filename, "Data")
        {
            Format = "Encoding",
            Content = isValid.ToString()
        };
    }

    private bool IsBase64String(string base64)
    {
        if (string.IsNullOrWhiteSpace(base64))
            return false;

        base64 = base64.Trim();
        if (base64.Length % 4 != 0)
            return false;

        try
        {
            Convert.FromBase64String(base64);
            return true;
        }
        catch
        {
            return false;
        }
    }
}