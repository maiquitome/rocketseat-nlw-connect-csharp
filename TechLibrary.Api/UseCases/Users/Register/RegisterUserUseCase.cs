using TechLibrary.Api.Domain.Entities;
using TechLibrary.Api.infrastructure.DataAccess;
using TechLibrary.Communication.Requests;
using TechLibrary.Communication.Responses;
using TechLibrary.Exception;

namespace TechLibrary.Api.UseCases.Users.Register;

public class RegisterUserUseCase
{
    public ResponseRegisteredUserJson Execute(RequestUserJson request)
    {
        Validate(request);

        var entity = new User
        {
            Name = request.Name,
            Email = request.Email,
            Password = request.Password
        };

        var dbContext = new TechLibraryDbContext();

        dbContext.Users.Add(entity);
        dbContext.SaveChanges();

        return new ResponseRegisteredUserJson
        {
            Name = entity.Name,
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

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
