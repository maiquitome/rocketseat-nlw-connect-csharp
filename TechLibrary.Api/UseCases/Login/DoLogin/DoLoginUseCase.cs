using TechLibrary.Api.Infrastructure.DataAccess;
using TechLibrary.Api.Infrastructure.Security.Cryptography;
using TechLibrary.Api.Infrastructure.Security.Tokens.Access;
using TechLibrary.Communication.Requests;
using TechLibrary.Communication.Responses;
using TechLibrary.Exception;

namespace TechLibrary.Api.UseCases.Login.DoLogin;

public class DoLoginUseCase
{
    public ResponseRegisteredUserJson Execute(RequestLoginJson request)
    {
        var dbContext = new TechLibraryDbContext();

        Domain.Entities.User? entity = dbContext.Users.FirstOrDefault(
            user => user.Email.Equals(request.Email)
        );

        if (entity is null)
            throw new InvalidLoginException();

        var cryptography = new BCryptAlgorithm();
        bool isPasswordInvalid = !cryptography.Verify(request.Password, entity);

        if (isPasswordInvalid)
            throw new InvalidLoginException();

        var tokenGenerator = new JwtTokenGenerator();

        return new ResponseRegisteredUserJson
        {
            Name = entity.Name,
            AccessToken = tokenGenerator.Generate(entity)
        };
    }
}
