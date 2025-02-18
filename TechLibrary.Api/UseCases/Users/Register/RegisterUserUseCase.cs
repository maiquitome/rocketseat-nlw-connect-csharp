using TechLibrary.Communication.Requests;
using TechLibrary.Communication.Responses;

namespace TechLibrary.Api.UseCases.Users.Register;

public class RegisterUserUseCase
{
    public ResponseRegisteredUserJson Execute(RequestUserJson request)
    {
        Validate(request);

        return new ResponseRegisteredUserJson
        {

        };
    }

    private static void Validate(RequestUserJson request)
    {
        var validator = new RegisterUserValidator();

        FluentValidation.Results.ValidationResult result = validator.Validate(request);

        bool isInvalid = result.IsValid == false;

        if (isInvalid)
        {
            var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();
        }
    }
}
