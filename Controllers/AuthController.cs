
using DotNetRPG.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotNetRPG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;

        public AuthController(IAuthRepository  authRepo)
        {
            _authRepo = authRepo;
        }
        [HttpPost("Register")]
        public async Task<ActionResult<ServiceResponse<int>>>Register(UserRegisterDTO request)
        {
            var response = await _authRepo.Register(
                new User
                {
                    Name = request.UserName,
                },
                request.Password
                );
            if (!response.Success)
            {
                return BadRequest(response);
            }
            else
            {
                return Ok(response);
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login(UserLoginDTO request)
        {
            var response = await _authRepo.Login(request.UserName, request.Password);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            else
            {
                return Ok(response);
            }
        }


    }
}
