namespace OCS.TeamAssistant.Appraiser.Domain.Exceptions;

public sealed class AppraiserException : ApplicationException
{
    public AppraiserException(string message)
        : base(message)
    {
    }
}