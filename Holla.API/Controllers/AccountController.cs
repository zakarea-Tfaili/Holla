using Holla.BLL.DTOs;
using Holla.BLL.Features;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Holla.API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly AccountManager _accountManager;

        public AccountController(AccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            return await _accountManager.Register(registerDto);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            return await _accountManager.Login(loginDto);
        }
    }
}