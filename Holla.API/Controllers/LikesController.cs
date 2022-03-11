using Holla.API.Extensions;
using Holla.BLL.DTOs;
using Holla.BLL.Features;
using Holla.BLL.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Holla.API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly LikesManager _likesManager;

        public LikesController(LikesManager likesManager)
        {
            _likesManager = likesManager;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            if(await _likesManager.AddLike(sourceUserId, username))
            {
                return Ok();
            }
            return BadRequest("Failed to like user");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
        {
            var users = await _likesManager.GetUserLikes(User.GetUserId() ,likesParams);
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(users);
        }
    }
}