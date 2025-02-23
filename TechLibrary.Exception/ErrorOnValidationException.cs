using System.Net;

namespace TechLibrary.Exception;

public class ErrorOnValidationException : TechLibraryException
{
    private readonly List<string> _errorMessages;

    public ErrorOnValidationException(List<string> errorMessages) : base(string.Empty)
    {
        _errorMessages = errorMessages;
    }

    public override List<string> GetErrorMessages() => _errorMessages;
    public override HttpStatusCode GetStatusCode() => HttpStatusCode.BadRequest;
}
