using DeliveryProductAPI.Server.Dtos;
using DeliveryProductAPI.Server.Models;
using DeliveryProductAPI.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace DeliveryProductAPI.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IWebHostEnvironment _env;

        public ProductController(
            IUserRepository userRepository,
            ICategoryRepository categoryRepository,
            IProductRepository productRepository,
            IWebHostEnvironment webHostEnvironment)
        {
            _userRepository = userRepository;
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _env = webHostEnvironment;
        }

        [HttpGet("get")]
        public async Task<IActionResult> Get(int id)
        {
            Product? product = await _productRepository.FirstAsync(id, "Category");
            if (product == null)
                return BadRequest();

            return Ok(new
            {
                id = product.Id,
                title = product.Title,
                description = product.Description,
                price = product.Price,
                imagePath = product.ImagePath,
                categoryId = product.CategoryId,
                categoryName = product.Category.Name,
                weight = product.Weight,
                weightUnit = product.WeightUnit,
                count = product.Count
            });
        }

        [HttpPatch("update_category/{id}/{categoryId}")]
        public async Task<IActionResult> UpdateCategory(int id, int categoryId)
        {
            if (id < 1 || categoryId < 1)
                return BadRequest();

            Product? product = await _productRepository.FirstAsync(id, "Category");
            if (product == null)
                return BadRequest();

            product.CategoryId = categoryId;
            await _productRepository.SaveChangesAsync();

            return Ok();
        }

        [HttpPatch("update_title/{id}/{title}")]
        public async Task<IActionResult> UpdateTitle(int id, string title)
        {
            if (id < 1 || title.Length < 2)
                return BadRequest();

            Product? product = await _productRepository.FirstAsync(id);
            if (product == null)
                return BadRequest();

            product.Title = title;
            await _productRepository.SaveChangesAsync();

            return Ok();
        }

        [HttpPatch("update_description/{id}/{description}")]
        public async Task<IActionResult> UpdateDescription(int id, string description)
        {
            if (id < 1 || description.Length < 2)
                return BadRequest();

            Product? product = await _productRepository.FirstAsync(id);
            if (product == null)
                return BadRequest();

            product.Description = description;
            await _productRepository.SaveChangesAsync();

            return Ok();
        }

        [HttpPatch("update_price/{id}/{price}")]
        public async Task<IActionResult> UpdatePrice(int id, double price)
        {
            if (id < 1 || price <= 0)
                return BadRequest();

            Product? product = await _productRepository.FirstAsync(id);
            if (product == null)
                return BadRequest();

            product.Price = price;
            await _productRepository.SaveChangesAsync();

            return Ok();
        }

        [HttpPatch("update_count/{id}/{count}")]
        public async Task<IActionResult> UpdateCount(int id, int count)
        {
            if (id < 1 || count < 0)
                return BadRequest();

            Product? product = await _productRepository.FirstAsync(id);
            if (product == null)
                return BadRequest();

            product.Count = count;
            await _productRepository.SaveChangesAsync();

            return Ok();
        }

        [HttpPatch("update_image")]
        public async Task<IActionResult> UpdateImage(ProductUpdateImageDto dto)
        {
            if (dto.Id < 1 || dto.ImageFile.Length < 1)
                return BadRequest();

            Product? product = await _productRepository.FirstAsync(dto.Id);
            if (product == null)
                return BadRequest();

            if (System.IO.File.Exists($"{_env.WebRootPath}{product.ImagePath}"))
                System.IO.File.Delete($"{_env.WebRootPath}{product.ImagePath}");

            string path = $"/images/{Guid.NewGuid()}_{Path.GetExtension(dto.ImageFile.FileName)}";
            await System.IO.File.WriteAllBytesAsync(Path.Combine(_env.WebRootPath, path.Substring(1)), await Helpers.ResizeImageAsync(dto.ImageFile));

            product.ImagePath = path;
            await _productRepository.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id < 1)
                return BadRequest();

            await _productRepository.Delete(await _productRepository.FirstAsync(id));

            return Ok();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string query)
        {
            if (query.Length < 2)
                return BadRequest();

            List<dynamic> result = new List<dynamic>();

            foreach (var product in await _productRepository.WhereAsync(p => p.Title.Contains(query)))
            {
                result.Add(new
                {
                    Id = product.Id,
                    Price = product.Price,
                    ImagePath = product.ImagePath,
                    Title = product.Title,
                    Description = product.Description,
                    CategoryId = product.CategoryId,
                    Weight = product.Weight,
                    WeightUnit = product.WeightUnit
                });
            }
            return Ok(result);
        }
    }
}
