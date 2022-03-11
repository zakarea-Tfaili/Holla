using Holla.API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Holla.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {

    }
}