namespace andis2_api_cuentas.Services;

public struct StatusValues
{
    public const string Processing = "Processing";
    public const string Complete = "Complete";
    public const string Failed = "Failed";
}

public interface IStatusService
{
    public string GetStatus(Guid guid);
    public void SetStatus(Guid guid, string status);
}

public class StatusService : IStatusService
{
    private readonly Dictionary<Guid, string> _status = new();

    public string GetStatus(Guid guid)
    {
        if (_status.TryGetValue(guid, out var status))
        {
            return status;
        }

        return StatusValues.Failed;
    }

    public void SetStatus(Guid guid, string status)
    {
        _status[guid] = status;
    }
}