using AutoMapper;
using Holla.BLL.DTOs;
using Holla.BLL.Exceptions;
using Holla.BLL.Helpers;
using Holla.BLL.Interfaces;
using Holla.DAL.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Holla.BLL.Features
{
    public class UserManager
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UserManager(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _photoService = photoService;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<PagedList<MemberDto>> GetUsers(string userName, UserParams userParams)
        {
            var user = await _userRepository.GetUserByUsernameAsync(userName);
            userParams.CurrentUsername = user.UserName;
            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = user.Gender == "male" ? "female" : "male";
            }
            return await _userRepository.GetMembersAsync(userParams);
        }

        public async Task<MemberDto> GetUser(string username)
        {
            return await _userRepository.GetMemberAsync(username);
        }

        public async Task<bool> UpdateUser(string userName, MemberUpdateDto memberUpdateDto)
        {
            var user = await _userRepository.GetUserByUsernameAsync(userName);
            _mapper.Map(memberUpdateDto, user);
            _userRepository.Update(user);
            return await _userRepository.SaveAllAsync();
        }

        public async Task<PhotoDto> AddPhoto(string userName, Stream fileStream, string fileName)
        {
            var user = await _userRepository.GetUserByUsernameAsync(userName);
            var result = await _photoService.AddPhotoAsync(fileStream, fileName);
            if (result.Error != null)
            {
                throw new ApiException(result.Error.Message);
            }
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            if (user.Photos.Count() == 0)
            {
                photo.IsMain = true;
            }
            user.Photos.Add(photo);
            if (await _userRepository.SaveAllAsync())
            {
                return _mapper.Map<PhotoDto>(photo);
            }
            return null;
        }

        public async Task<bool> SetMainPhoto(string userName,int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(userName);
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo == null)
            {
                throw new ApiException("Photo not recognized");
            }
            if (photo.IsMain) {
                throw new ApiException("This is already your main photo");
            } 
            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;
            return await _userRepository.SaveAllAsync();
        }

        public async Task<bool> DeletePhoto(string userName, int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(userName);
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) {
                throw new KeyNotFoundException();
            }
            if (photo.IsMain)
            {
                throw new ApiException("You cannot delete your main photo");
            } 
            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) {
                    throw new ApiException(result.Error.Message);
                }
            }
            user.Photos.Remove(photo);
            return await _userRepository.SaveAllAsync();
        }      
    }
}
