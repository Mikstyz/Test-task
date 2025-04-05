using DTOs.Product;
using Entities.Product;
using Microsoft.AspNetCore.Mvc;
using Repositories.Product;
using Serilog;
using Service;
using gRCP;

namespace Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductManager _productService;

        public ProductsController(ProductManager productService)
        {
            _productService = productService;
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetProducts([FromQuery] int offset = 0, [FromQuery] int limit = 20)
        {
            Log.Information($"GET запрос на получение продуктов: offset={offset}, limit={limit}");

            var products = await _productService.GetProducts(offset, limit);
            if (products == null)
            {
                Log.Warning("Не удалось получить список продуктов");
                return BadRequest("Каталог пуст");
            }

            var product = new List<product>();

            foreach (var p in product)
            {
                product.Add(new product
                {
                    Seller = p.Seller,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Quantity = p.Quantity
                });
            }

            return Ok(product);
        }


        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            Log.Information($"GET запрос на получение продукта по id={id}");

            var products = await _productService.GetProductById(id);
            if (products == null)
            {
                Log.Warning($"Продукт с id={id} не найден");
                return NotFound("Продукт не найден");
            }

            var product = new List<product>();

            foreach (var p in product)
            {
                product.Add(new product
                {
                    Seller = p.Seller,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Quantity = p.Quantity
                });
            }

            return Ok(product);
        }

        #region gRCP
        [HttpPost("check-availability")]
        public async Task<IActionResult> CheckAvailability([FromBody] CheckAvailabilityDto request)
        {
            if (request == null || request.Quantity <= 0)
            {
                return BadRequest("Некорректные данные запроса.");
            }

            bool isAvailable = await grspManager.CheckProductCount(request.productId, request.Quantity);

            if (isAvailable)
            {
                return Ok(new { available = true });
            }

            return Ok(new { available = false });
        }


        [HttpPost("Create-order")]
        public async Task<IActionResult> CreateOrderInProduct([FromBody] CreateOrderDto request)
        {
            if (request == null || request.ProductsIds == null || request.ProductsIds.Count <= 0)
            {
                return BadRequest("Некорректные данные запроса.");
            }

            bool status = await grspManager.CreateOrderInProduct(request.ProductsIds);

            if (status)
            {
                return Ok(new { Create = true });
            }

            return Ok(new { Create = false });
        }

        #endregion

        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] SearchProductDto searchDto)
        {
            Log.Information($"GET запрос на поиск продуктов: offset={searchDto.Offset}, limit={20}, seller={searchDto.Seller}, name={searchDto.Name}, price={searchDto.Price}, quantity={searchDto.Quantity}");

            var products = await _productService.SearchProducts(searchDto.Offset, 20, searchDto.Seller, searchDto.Name, searchDto.Price, searchDto.Quantity);
            if (products == null)
            {
                Log.Warning("Не удалось найти продукты по заданным параметрам");
                return BadRequest("Ничего не нашлось");
            }

            var product = new List<product>();

            foreach (var p in products)
            {
                product.Add(new product
                {
                    Seller = p.Seller,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Quantity = p.Quantity
                });
            }

            return Ok(product);
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createDto)
        {
            Log.Information($"POST запрос на создание продукта: {createDto.Name}");

            var newProduct = new product
            {
                Seller = createDto.Seller,
                Name = createDto.Name,
                Description = createDto.Description,
                Price = createDto.Price,
                Quantity = createDto.Quantity
            };

            int productId = await _productService.CreateProduct(newProduct);
            if (productId <= 0)
            {
                Log.Warning($"Не удалось создать продукт: {createDto.Name}");
                return BadRequest("Не получилось создать продукт");
            }

            return CreatedAtAction(nameof(GetProductById), new { id = productId }, new { id = productId });
        }


        [HttpPut("update")]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductDto updateDto)
        {
            Log.Information($"PUT запрос на обновление продукта с id={updateDto.Id}");

            var updatedProduct = new updateProduct
            {
                id = updateDto.Id,
                NewName = updateDto.NewName,
                NewDescription = updateDto.NewDescription,
                NewPrice = updateDto.NewPrice,
                NewQuantity = updateDto.NewQuantity
            };

            bool isUpdated = await _productService.UpdateProduct(updatedProduct);
            if (!isUpdated)
            {
                Log.Warning($"Не удалось обновить продукт с id={updateDto.Id}");
                return BadRequest("Не удалось обновить продукт");
            }

            return Ok("Продукт обновлен");
        }


        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            Log.Information($"DELETE запрос на удаление продукта с id={id}");

            bool isDeleted = await _productService.DeleteProduct(id);
            if (!isDeleted)
            {
                Log.Warning($"Не удалось удалить продукт с id={id}");
                return NotFound("Ошибка при удалении продукта");
            }

            return Ok("Продукт удален");
        }
    }
}