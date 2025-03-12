using DeliveryProductAPI.Server.Dtos;
using DeliveryProductAPI.Server.Models;
using DeliveryProductAPI.Server.Repositories.Interfaces;
using DeliveryProductAPI.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text.RegularExpressions;

namespace DeliveryProductAPI.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IOrderRepository _orderRepository;

        private readonly JwtService _jwtService;

        public UserController(IUserRepository userRepository, IRoleRepository roleRepository, JwtService jwtService, IOrderRepository orderRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _jwtService = jwtService;
            _orderRepository = orderRepository;
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

        [DisableCors]
        [HttpPost("signin")]
        public async Task<IActionResult> SignIn(SignInDto dto)
        {
            User? user = null;
            if (dto.Login != null)
                user = await _userRepository.FirstAsync(dto.Login, "Role");
            else if (dto.EmailAddress != null)
                user = await _userRepository.FirstWhereAsync(u => u.EmailAddress != null && u.EmailAddress.Equals(dto.EmailAddress.ToLower()), "Role");

            if (user == null)
                return Unauthorized("Пользователь не найден");

            if (!user.VerifyPassword(dto.Password))
                return Unauthorized("Неправильный пароль");


            string token = _jwtService.GenerateToken(user.Login, user.Role.Name);

            return Ok(new
            {
                token
            });
        }

        [HttpPost("signinadmin")]
        public async Task<IActionResult> SignInAdmin(SignInDto dto)
        {
            User? user = null;
            if (dto.Login != null)
                user = await _userRepository.FirstWhereAsync(u => u.Login == dto.Login.ToLower() && u.Role.Name.Contains("ADMIN"), "Role");
            else if (dto.EmailAddress != null)
                user = await _userRepository.FirstWhereAsync(u => u.EmailAddress != null && u.EmailAddress.Equals(dto.EmailAddress.ToLower()) && u.Role.Name.Contains("ADMIN"), "Role");

            if (user == null)
                return Unauthorized("Пользователь не найден");

            if (!user.VerifyPassword(dto.Password))
                return Unauthorized("Неправильный пароль");


            string token = _jwtService.GenerateToken(user.Login, user.Role.Name);

            return Ok(new
            {
                token
            });
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(SignUpDto dto)
        {
            if (await _userRepository.FirstWhereAsync(u => u.EmailAddress.ToLower() == dto.EmailAddress.ToLower()) != null)
                return Unauthorized("Почта занята");
            if (await _userRepository.FirstAsync(dto.Login) != null)
                return Unauthorized("Логин занят");

            Role? role = await _roleRepository.FirstAsync("CLIENT") ?? throw new ArgumentNullException("Dont found a client role in database table");
            User? user = await _userRepository.AddAsync(new User(dto.EmailAddress, dto.Login, dto.Password, role));

            if (user == null)
                return StatusCode(500, "Ошибка сервера");

            return Ok(new
            {
                token = _jwtService.GenerateToken(user.Login, role.Name)
            });
        }

        [HttpGet("auth")]
        [Authorize]
        public async Task<IActionResult> Auth()
        {
            User? user = await GetSender("Role");
            if (user == null)
                return Unauthorized();

            return Ok(new
            {
                Id = user.Id,
                Email = user.EmailAddress,
                Login = user.Login,
                Role = user.Role.Name,
                CreatedAt = user.CreatedAt
            });
        }

        [HttpGet("my_active_orders")]
        [Authorize]
        public async Task<IActionResult> MyActiveOrders()
        {
            List<dynamic> result = new();

            foreach (var item in (await GetSender("Orders")).Orders.Where(o => o.StatusId < 4))
            {
                var order = await _orderRepository.FirstAsync(item.Id, "Status", "Products.Product");
                if (order == null)
                    continue;
                List<dynamic> products = new();

                foreach (var product in order.Products)
                    products.Add(new
                    {
                        Product = new
                        {
                            Id = product.Id,
                            Title = product.Product.Title,
                            Price = product.Product.Price
                        },
                        Count = product.Count
                    });

                result.Add(new
                {
                    Id = item.Id,
                    DeliveryPrice = item.DeliveryPrice,
                    DeliveryTime = item.DeliveryTime,
                    StatusId = order.StatusId,
                    IsCompleted = order.Status.Id == 4,
                    Status = order.Status.Name,
                    TotalPrice = order.TotalPrice,
                    Products = products,
                    CreatedAt = item.CreatedAt,
                    Address = item.Address
                });

            }

            return Ok(result);
        }

        [HttpGet("my_completed_orders")]
        [Authorize]
        public async Task<IActionResult> MyCompletedOrders()
        {
            List<dynamic> result = new();

            foreach (var item in (await GetSender("Orders")).Orders.Where(o => o.StatusId == 4))
            {
                var order = await _orderRepository.FirstAsync(item.Id, "Status", "Products.Product");
                if (order == null)
                    continue;
                List<dynamic> products = new();

                foreach (var product in order.Products)
                    products.Add(new
                    {
                        Product = new
                        {
                            Id = product.Id,
                            Title = product.Product.Title,
                            Price = product.Product.Price
                        },
                        Count = product.Count
                    });

                result.Add(new
                {
                    Id = item.Id,
                    DeliveryPrice = item.DeliveryPrice,
                    DeliveryTime = item.DeliveryTime,
                    StatusId = order.StatusId,
                    IsCompleted = order.Status.Id == 4,
                    Status = order.Status.Name,
                    TotalPrice = order.TotalPrice,
                    Products = products,
                    CreatedAt = item.CreatedAt,
                    Address = item.Address
                });

            }

            return Ok(result);
        }

        [HttpPatch("change_login/{login}")]
        [Authorize]
        public async Task<IActionResult> ChangeLogin(string login)
        {
            User? sender = await GetSender();
            if (sender == null)
                return Unauthorized();
            if (login.Length < 3 || login.Length > 64)
                return BadRequest();

            if (await _userRepository.FirstAsync(login) != null)
                return BadRequest("Логин занят");

            sender.Login = login.ToLower();
            await _userRepository.SaveChangesAsync();

            return Ok();
        }

        [HttpPatch("change_password/{password}")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(string password)
        {
            User? sender = await GetSender();
            if (sender == null)
                return Unauthorized();
            if (password.Length < 7 || password.Length > 255)
                return BadRequest();

            sender.SetPassword(password);
            await _userRepository.SaveChangesAsync();

            return Ok();
        }

        [HttpPatch("change_email/{email}")]
        [Authorize]
        public async Task<IActionResult> ChangeEmail(string email)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, pattern))
                return BadRequest("Недействительная почта");

            User? sender = await GetSender();
            if (sender == null)
                return Unauthorized();

            if (await _userRepository.FirstWhereAsync(u => u.EmailAddress.ToLower() == email.ToLower()) != null)
                return BadRequest("Почта занята");

            sender.EmailAddress = email.ToLower();
            await _userRepository.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("delete")]
        [Authorize]
        public async Task<IActionResult> Delete()
        {
            User? sender = await GetSender("Orders");
            if (sender == null)
                return Unauthorized();

            sender.Status = false;
            sender.Login = $"DELETED_{Guid.NewGuid()}__{sender.Login}";
            sender.EmailAddress = $"DELETED_{Guid.NewGuid()}__{sender.EmailAddress}";

            sender.Orders.ForEach(o => o.StatusId = 4);

            await _userRepository.SaveChangesAsync();

            return Ok();
        }
    }
}
