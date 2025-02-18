using Microsoft.AspNetCore.Mvc;
using TechLibrary.Api.UseCases.Users.Register;
using TechLibrary.Communication.Requests;
using TechLibrary.Communication.Responses;
using TechLibrary.Exception;

namespace TechLibrary.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ResponseRegisteredUserJson), StatusCodes.Status201Created)]
    public IActionResult Create(RequestUserJson request)
    {
        try
        {
            var useCase = new RegisterUserUseCase();

            ResponseRegisteredUserJson response = useCase.Execute(request);

            return Created(string.Empty, response);
        }
        catch (TechLibraryException ex)
        {
            return BadRequest(ex.GetErrorMessages());
        }

    }
}

