using Holla.BLL.DTOs;
using Holla.BLL.Exceptions;
using Holla.BLL.Helpers;
using Holla.BLL.Interfaces;
using Holla.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Holla.BLL.Features
{
    public class LikesManager
    {
        private readonly IUserRepository _userRepository;
        private readonly ILikesRepository _likesRepository;
        public LikesManager(IUserRepository userRepository, ILikesRepository likesRepository)
        {
            _likesRepository = likesRepository;
            _userRepository = userRepository;
        }

        public async Task<bool> AddLike(int userId ,string targetUsername)
        {
            var likedUser = await _userRepository.GetUserByUsernameAsync(targetUsername);
            var sourceUser = await _likesRepository.GetUserWithLikes(userId);
            if (likedUser == null)
            {
                throw new KeyNotFoundException();
            }
            if (sourceUser.UserName == targetUsername)
            {
                throw new ApiException("You cannot like yourself");
            } 
            var userLike = await _likesRepository.GetUserLike(userId, likedUser.Id);
            if (userLike != null)
            {
                throw new ApiException("You already like this user");
            }
            userLike = new UserLike
            {
                SourceUserId = userId,
                LikedUserId = likedUser.Id
            };
            sourceUser.LikedUsers.Add(userLike);
            return await _userRepository.SaveAllAsync();
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(int userId, LikesParams likesParams)
        {
            likesParams.UserId = userId;
            return await _likesRepository.GetUserLikes(likesParams);
        }
    }
}
