using DeliveryProductAPI.Server.Dtos;
using DeliveryProductAPI.Server.Models;
using DeliveryProductAPI.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DeliveryProductAPI.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IWebHostEnvironment _env;
        private readonly IOrderRepository _orderRepository;

        public ChatController(IUserRepository userRepository, IChatRepository chatRepository, IOrderRepository orderRepository, IWebHostEnvironment env)
        {
            _userRepository = userRepository;
            _chatRepository = chatRepository;
            _orderRepository = orderRepository;
            _env = env;
        }

        private async Task<User?> GetSender(params string[] includes)
        {
            if (User?.Identity?.Name == null)
                return null;

            User? result = await _userRepository.FirstAsync(User.Identity.Name, includes);
            if (result == null)
                throw new BadHttpRequestException("");
            return result;
        }

        [HttpGet("get_id")]
        public async Task<IActionResult> GetId(int orderId)
        {
            var res = await _chatRepository.FirstWhereAsync(c => c.OrderId == orderId);
            if (res == null)
                return BadRequest();

            return Ok(res.Id);
        }

        [HttpPost("add/{orderId}")]
        public async Task<IActionResult> Add(int orderId)
        {
            var res = await _chatRepository.FirstWhereAsync(c => c.OrderId == orderId);
            if (res != null)
                return BadRequest();

            var admin = await _userRepository.FirstWhereAsync(u => u.Role.Name == "ADMIN");
            var order = await _orderRepository.FirstAsync(orderId, "User");
            var client = order?.User;

            if (client == null || admin == null || order == null)
                return BadRequest();

            Chat? chat = await _chatRepository.AddAsync(new Chat(client, admin, order));
            if (chat == null)
                return StatusCode(500, "Не удалось создать чат");

            return Ok();
        }

        [HttpGet("get_all")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAll()
        {
            List<dynamic> result = new();
            foreach (var item in await _chatRepository.WhereAsync(c => c.Messages.Count > 0, "Client", "Messages"))
            {
                result.Add(new
                {
                    Id = item.Id,
                    ClientId = item.ClientId,
                    ClientLogin = item.Client.Login,
                    AdminId = item.AdminId,
                    OrderId = item.OrderId,
                    LastMessage = item.Messages[^1].Content,
                    LastMessageDate = item.Messages[^1].CreatedAt
                });
            }


            return Ok(result.OrderByDescending(r => r.LastMessageDate));
        }

        [HttpPost("upload_image")]
        [Authorize]
        public async Task<IActionResult> UploadImage(UploadImageDto dto)
        {
            var file = dto.File;
            User? sender = await GetSender();
            Chat? chat = await _chatRepository.FirstAsync(dto.ChatId);
            if (file == null || file.Length < 1 || file.FileName.Length < 1 || chat == null || sender == null)
                return BadRequest();


            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            byte[] jpgBytes = Helpers.ConvertToJpg(memoryStream.ToArray(), 85);

            string path = $"/images/{Guid.NewGuid()}.jpg";
            await System.IO.File.WriteAllBytesAsync(Path.Combine(_env.WebRootPath, path.Substring(1)), jpgBytes);

            return Ok(path);
        }
    }
}
