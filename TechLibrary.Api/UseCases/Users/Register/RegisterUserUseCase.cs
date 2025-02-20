using FluentValidation.Results;
using TechLibrary.Api.Domain.Entities;
using TechLibrary.Api.Infrastructure.DataAccess;
using TechLibrary.Api.Infrastructure.Security.Cryptography;
using TechLibrary.Api.Infrastructure.Security.Tokens.Access;
using TechLibrary.Communication.Requests;
using TechLibrary.Communication.Responses;
using TechLibrary.Exception;

namespace TechLibrary.Api.UseCases.Users.Register;

public class RegisterUserUseCase
{
    public ResponseRegisteredUserJson Execute(RequestUserJson request)
    {
        var dbContext = new TechLibraryDbContext();

        Validate(request, dbContext);

        var cryptography = new BCryptAlgorithm();

        var entity = new User
        {
            Name = request.Name,
            Email = request.Email,
            Password = cryptography.HashPassword(request.Password)
        };

        dbContext.Users.Add(entity);
        dbContext.SaveChanges();

        var tokenGenerator = new JwtTokenGenerator();

        return new ResponseRegisteredUserJson
        {
            Name = entity.Name,
            AccessToken = tokenGenerator.Generate(entity)
        };
    }

    private static void Validate(RequestUserJson request, TechLibraryDbContext dbContext)
    {
        var validator = new RegisterUserValidator();

        ValidationResult result = validator.Validate(request);

        bool isThereUserWithThisEmail = dbContext.Users.Any(user => user.Email == request.Email);

        if (isThereUserWithThisEmail)
        {
            result.Errors.Add(new ValidationFailure("Email", "Email já cadastrado."));
        }

        bool isInvalid = result.IsValid == false;

        if (isInvalid)
        {
            var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
