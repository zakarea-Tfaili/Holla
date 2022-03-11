using Holla.API.Extensions;
using Holla.BLL.DTOs;
using Holla.BLL.Features;
using Holla.BLL.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Holla.API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly UserManager _userManager;

        public UsersController(UserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
        {
            var users = await _userManager.GetUsers(User.GetUsername(), userParams);
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(users);
        }

        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _userManager.GetUser(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            if (await _userManager.UpdateUser(User.GetUsername(), memberUpdateDto))
            {
                return NoContent();
            }
            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var result = await _userManager.AddPhoto(User.GetUsername(), file.OpenReadStream(), file.FileName);
            if (result != null)
            {
                return CreatedAtRoute("GetUser", new { username = User.GetUsername() }, result);
            }
            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            if (await _userManager.SetMainPhoto(User.GetUsername(), photoId))
            {
                return NoContent();
            }
            return BadRequest("Failed to set main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            if (await _userManager.DeletePhoto(User.GetUsername(), photoId))
            {
                return Ok();
            }
            return BadRequest("Failed to delete the photo");
        }

    }
}