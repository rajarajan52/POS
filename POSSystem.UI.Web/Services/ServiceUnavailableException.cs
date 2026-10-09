namespace POSSystem.UI.Web.Services;

/// <summary>
/// Exception thrown when a microservice is unavailable.
/// </summary>
public class ServiceUnavailableException : Exception
{
    public ServiceUnavailableException(string serviceName, Exception innerException)
        : base($"{serviceName} is currently unavailable", innerException)
    {
        ServiceName = serviceName;
    }

    public string ServiceName { get; }
}


