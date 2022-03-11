using AutoMapper;
using Holla.BLL.DTOs;
using Holla.BLL.Exceptions;
using Holla.BLL.Helpers;
using Holla.BLL.Interfaces;
using Holla.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Holla.BLL.Features
{
    public class MessageManager
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public MessageManager(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
        {
            _mapper = mapper;
            _messageRepository = messageRepository;
            _userRepository = userRepository;
        }

        public async Task<MessageDto> CreateMessage(string userName, CreateMessageDto createMessageDto)
        {
            if (userName.Equals(createMessageDto.RecipientUsername.ToLower()))
            {
                throw new ApiException("You cannot send messages to yourself");
            }
            var sender = await _userRepository.GetUserByUsernameAsync(userName);
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername.ToLower());
            if (recipient == null)
            {
                throw new KeyNotFoundException();
            }
            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };
            _messageRepository.AddMessage(message);
            if (await _messageRepository.SaveAllAsync())
            {
                return _mapper.Map<MessageDto>(message);
            }
            return null;
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(string userName, MessageParams messageParams)
        {
            messageParams.Username = userName;
            return await _messageRepository.GetMessagesForUser(messageParams);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string username, string targetUserName)
        {
            return await _messageRepository.GetMessageThread(username, targetUserName);
        }

        public async Task<bool> DeleteMessage(string userName, int id)
        {
            var message = await _messageRepository.GetMessage(id);
            if (!message.Sender.UserName.Equals(userName) && !message.Recipient.UserName.Equals(userName))
            {
                throw new UnauthorizedAccessException();
            }
            if (message.Sender.UserName == userName)
            { 
                message.SenderDeleted = true;
            }
            if (message.Recipient.UserName == userName)
            { 
                message.RecipientDeleted = true;
            }
            if (message.SenderDeleted && message.RecipientDeleted)
                _messageRepository.DeleteMessage(message);

            return await _messageRepository.SaveAllAsync();
        }

    }
}
