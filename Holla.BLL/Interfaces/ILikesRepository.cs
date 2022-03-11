using Holla.BLL.DTOs;
using Holla.BLL.Helpers;
using Holla.DAL.Entities;
using System.Threading.Tasks;

namespace Holla.BLL.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int likedUserId);

        Task<AppUser> GetUserWithLikes(int userId);

        Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
    }
}