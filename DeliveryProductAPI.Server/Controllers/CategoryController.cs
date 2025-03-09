using DeliveryProductAPI.Server.Dtos;
using DeliveryProductAPI.Server.Models;
using DeliveryProductAPI.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryProductAPI.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IWebHostEnvironment _env;

        public CategoryController(
            ICategoryRepository categoryRepository,
            IProductRepository productRepository,
            IWebHostEnvironment webHostEnvironment
            )
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _env = webHostEnvironment;
        }

        [HttpPost("add")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Add(AddCategoryDto dto)
        {
            Category? category = await _categoryRepository.AddAsync(
                new Category(dto.Name)
                );

            if (category == null)
                return StatusCode(500, "Ошибка сервера");

            return Ok();
        }

        [HttpGet("get_all")]
        public async Task<IActionResult> GetAll()
        {
            List<dynamic> result = new List<dynamic>();

            foreach (var category in await _categoryRepository.WhereAsync(c => true))
            {
                result.Add(new
                {
                    Id = category.Id,
                    Name = category.Name
                });
            }

            return Ok(result);
        }

        [HttpGet("get")]
        public async Task<IActionResult> Get(int id)
        {
            Category? category = await _categoryRepository.FirstAsync(id);

            return Ok(new
            {
                Id = category?.Id,
                Name = category?.Name
            });
        }

        [HttpGet("get_products")]
        public async Task<IActionResult> GetProducts(int categoryId)
        {
            List<dynamic> result = new List<dynamic>();

            foreach (var product in (await _categoryRepository.FirstAsync(categoryId, "Products")).Products.Where(p => p.Status))
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
                    WeightUnit = product.WeightUnit,
                    Count = product.Count
                });

            }

            return Ok(result);
        }

        [HttpPost("add_product")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> AddProduct(CategoryAddProductDto dto)
        {
            Category? category = await _categoryRepository.FirstAsync(dto.CategoryId, "Products");
            if (category == null)
                return BadRequest("Категория не найдена");

            if (dto.ImageFile.Length < 1)
                return BadRequest();

            string path = $"/images/{Guid.NewGuid()}_{Path.GetExtension(dto.ImageFile.FileName)}";
            await System.IO.File.WriteAllBytesAsync(Path.Combine(_env.WebRootPath, path.Substring(1)), await Helpers.ResizeImageAsync(dto.ImageFile));

            Product? product = await _productRepository.AddAsync(new Product(
                dto.Title,
                dto.Description,
                dto.Price,
                category,
                path,
                dto.Weight,
                dto.WeightUnit,
                dto.Count
                ));

            if (product == null)
                return StatusCode(500, "Не удалось создать продукт");

            return Ok();
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            Category? category = await _categoryRepository.FirstAsync(id, "Products");
            if (category == null)
                return BadRequest();

            foreach (var product in category.Products)
                product.Status = false;

            category.Status = false;
            await _categoryRepository.SaveChangesAsync();

            return Ok();
        }


    }
}
