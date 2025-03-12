using DeliveryProductAPI.Server.Models;
using DeliveryProductAPI.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace DeliveryProductAPI.Server.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IWebHostEnvironment _env;

        public ChatHub(AppDbContext context, IUserRepository userRepository, IWebHostEnvironment env)
        {
            _context = context;
            _userRepository = userRepository;
            _env = env;
        }

        // Подключение пользователя и отправка истории сообщений
        public async Task JoinChat(int chatId)
        {
            Chat? chat = await _context.Chats.FirstOrDefaultAsync(c => c.Id == chatId);
            if (chat == null)
                return;

            var messages = await _context.Messages
                .Includes("Sender")
                .Where(m => m.ChatId == chatId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();


            List<dynamic> result = new();
            foreach (var item in messages)
            {
                result.Add(new
                {
                    Id = item.Id,
                    SenderId = item.SenderId,
                    SenderLogin = item.Sender.Login,
                    Content = item.Content,
                    CreatedAt = item.CreatedAt,
                    ChatId = item.ChatId,
                    AttachmentPath = item.AttachmentPath
                });
            }

            await JoinGroup(chatId);

            await Clients.Caller.SendAsync("ReceiveHistory", Newtonsoft.Json.JsonConvert.SerializeObject(result));
            await Clients.Caller.SendAsync("ReceiveHistoryReact", result);
        }

        private async Task<User?> GetSender(params string[] includes)
        {
            if (Context.User?.Identity?.Name == null)
                return null;

            User? result = await _userRepository.FirstAsync(Context.User.Identity.Name, includes);
            if (result == null)
                throw new BadHttpRequestException("");
            return result;
        }

        public async Task SendMessage(int chatId, string message)
        {

            var chat = await _context.Chats
                .Includes("Client", "Admin", "Order")
                .FirstOrDefaultAsync(c => c.Id == chatId);

            if (chat == null)
            {
                throw new HubException("Чат не найден.");
            }

            var sender = await GetSender();
            if (sender == null)
            {
                throw new HubException("Пользователь не найден.");
            }

            var newMessage = new Message(chat, sender, message);
            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                Id = newMessage.Id,
                SenderId = newMessage.SenderId,
                SenderLogin = newMessage.Sender.Login,
                Content = newMessage.Content,
                CreatedAt = newMessage.CreatedAt,
                ChatId = newMessage.ChatId,
                AttachmentPath = newMessage.AttachmentPath
            }));

            await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessageReact", new
            {
                Id = newMessage.Id,
                SenderId = newMessage.SenderId,
                SenderLogin = newMessage.Sender.Login,
                Content = newMessage.Content,
                CreatedAt = newMessage.CreatedAt,
                ChatId = newMessage.ChatId,
                AttachmentPath = newMessage.AttachmentPath
            });
        }

        public async Task SendImage(int chatId, string path)
        {
            if (chatId < 1 || path.Length < 5)
                return;

            var chat = await _context.Chats
                .Includes("Client", "Admin", "Order")
                .FirstOrDefaultAsync(c => c.Id == chatId);

            if (chat == null)
            {
                throw new HubException("Чат не найден.");
            }

            var sender = await GetSender();
            if (sender == null)
            {
                throw new HubException("Пользователь не найден.");
            }

            var newMessage = new Message(chat, sender, "Изображение");
            newMessage.AttachmentPath = path;
            await _context.Messages.AddAsync(newMessage);
            await _context.SaveChangesAsync();

            await Clients.Group(chatId.ToString()).SendAsync("ReceiveImage", Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                Id = newMessage.Id,
                SenderId = newMessage.SenderId,
                SenderLogin = newMessage.Sender.Login,
                Content = newMessage.Content,
                CreatedAt = newMessage.CreatedAt,
                ChatId = newMessage.ChatId,
                AttachmentPath = newMessage.AttachmentPath
            }));

            await Clients.Group(chatId.ToString()).SendAsync("ReceiveImageReact", new
            {
                Id = newMessage.Id,
                SenderId = newMessage.SenderId,
                SenderLogin = newMessage.Sender.Login,
                Content = newMessage.Content,
                CreatedAt = newMessage.CreatedAt,
                ChatId = newMessage.ChatId,
                AttachmentPath = newMessage.AttachmentPath
            });
        }

        // Присоединение к чату (группе)
        public async Task JoinGroup(int chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
        }


        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
