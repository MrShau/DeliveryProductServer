using DeliveryProductAPI.Server.Dtos;
using DeliveryProductAPI.Server.Models;
using DeliveryProductAPI.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryProductAPI.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IOrderRepository _orderRepostiory;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IChatRepository _chatRepository;

        public OrderController(IUserRepository userRepository, IOrderRepository orderRepostiory, IProductRepository productRepository, IOrderRepository orderRepository, IChatRepository chatRepository)
        {
            _userRepository = userRepository;
            _orderRepostiory = orderRepostiory;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _chatRepository = chatRepository;
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

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> Add(AddOrderDto dto)
        {
            List<Product> products = new();
            double totalPrice = 0;
            foreach (var item in dto.Products)
            {
                var product = await _productRepository.FirstAsync(item.ProductId);
                if (product != null)
                {
                    product.Count -= item.Count;
                    await _orderRepository.SaveChangesAsync();
                    products.Add(product);
                    totalPrice += product.Price * item.Count;
                }
            }
            var client = await GetSender();

            var order = await _orderRepostiory.AddAsync(new Models.Order(client, totalPrice, await _orderRepostiory.GetFirstStatus(), dto.DeliveryPrice, dto.DeliveryTime, dto.Address));
            if (order == null)
                return BadRequest();

            foreach (var product in products)
                await _orderRepostiory.AddProductAsync(new(order, product, dto.Products.FirstOrDefault(p => p.ProductId == product.Id).Count));

            var admin = await _userRepository.FirstWhereAsync(u => u.Role.Name == "ADMIN");

            if (client == null || admin == null || order == null)
                return Ok();

            await _chatRepository.AddAsync(new Chat(client, admin, order));

            return Ok();
        }

        [HttpGet("get")]
        public async Task<IActionResult> Get(int id)
        {
            Order? order = await _orderRepository.FirstAsync(id, "Products.Product", "Status", "User");
            if (order == null)
                return BadRequest();

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

            return Ok(new
            {
                Id = order.Id,
                DeliveryPrice = order.DeliveryPrice,
                DeliveryTime = order.DeliveryTime,
                StatusId = order.StatusId,
                IsCompleted = order.Status.Id == 4,
                Status = order.Status.Name,
                TotalPrice = order.TotalPrice,
                Products = products,
                CreatedAt = order.CreatedAt,
                ClientId = order.UserId,
                ClientLogin = order.User.Login,
                Address = order.Address
            });
        }

        [HttpGet("get_wait_confirmations")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetConfirmations()
        {
            List<dynamic> result = new();

            foreach (var item in (await _orderRepository.WhereAsync(o => o.StatusId == 1, "Status", "User", "Products.Product")))
            {
                List<dynamic> products = new();

                foreach (var product in item.Products)
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
                    StatusId = item.StatusId,
                    IsCompleted = item.Status.Id == 4,
                    Status = item.Status.Name,
                    TotalPrice = item.TotalPrice,
                    Products = products,
                    CreatedAt = item.CreatedAt,
                    ClientId = item.UserId,
                    ClientLogin = item.User.Login,
                    Address = item.Address
                });

            }

            return Ok(result);
        }

        [HttpGet("get_actives")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetActives()
        {
            List<dynamic> result = new();

            foreach (var item in (await _orderRepository.WhereAsync(o => o.StatusId > 1 && o.StatusId < 4, "Status", "Products.Product")))
            {
                List<dynamic> products = new();

                foreach (var product in item.Products)
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
                    StatusId = item.StatusId,
                    IsCompleted = item.Status.Id == 4,
                    Status = item.Status.Name,
                    TotalPrice = item.TotalPrice,
                    Products = products,
                    CreatedAt = item.CreatedAt,
                    Address = item.Address

                });

            }

            return Ok(result);
        }

        [HttpGet("get_completed")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetCompleted()
        {
            List<dynamic> result = new();

            foreach (var item in (await _orderRepository.WhereAsync(o => o.StatusId == 4, "Status", "Products.Product")))
            {
                List<dynamic> products = new();

                foreach (var product in item.Products)
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
                    StatusId = item.StatusId,
                    IsCompleted = item.Status.Id == 4,
                    Status = item.Status.Name,
                    TotalPrice = item.TotalPrice,
                    Products = products,
                    CreatedAt = item.CreatedAt,
                    Address = item.Address
                });

            }

            return Ok(result);
        }

        [HttpPost("confirm/{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Confirm(int id)
        {
            Order? order = await _orderRepository.FirstAsync(id, "Status");
            if (order == null)
                return BadRequest();

            order.StatusId++;
            await _orderRepository.SaveChangesAsync();

            return Ok();
        }
    }
}
