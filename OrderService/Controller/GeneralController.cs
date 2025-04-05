using DTOs.Order;
using Entities.Order;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Serilog;
using Service;

namespace Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }


        [HttpGet("OrderInUser")]
        public async Task<IEnumerable<Order>> getAllbyUserId(int userId, int offset = 0)
        {
            try
            {
                Log.Information($"Попытка получить все заказы для user_id={userId}, offset={offset}");
                var orders = await Orderdb.GetAllByUserId(userId, offset, 20);
                if (orders == null)
                {
                    Log.Error($"Ошибка при получении заказов для user_id={userId}");
                    return null;
                }
                Log.Information($"Успешно получено {orders.Count()} заказов для user_id={userId}");
                return orders;
            }
            catch (Exception ex)
            {
                Log.Fatal($"Ошибка при получении заказов для user_id={userId}\n{ex}");
                return null;
            }
        }


        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            Log.Information($"GET запрос на получение заказа по id={id}");

            var order = await _orderService.getById(id);
            if (order == null)
            {
                Log.Warning($"Заказ с id={id} не найден");
                return NotFound("Заказ не найден");
            }

            return Ok(order);
        }


        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
        {
            Log.Information($"POST запрос на создание заказа для user_id={dto.UserId} с товарами={string.Join(", ", dto.ProductsIds.Select(p => $"productId={p.productId}, quantity={p.Quantity}"))}");

            int orderId = await _orderService.Create(dto.UserId, dto.ProductsIds);
            if (orderId <= 0)
            {
                Log.Warning($"Не удалось создать заказ для user_id={dto.UserId}");
                return BadRequest("Не удалось создать заказ");
            }

            return CreatedAtAction(nameof(GetById), new { id = orderId }, new { id = orderId });
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Log.Information($"DELETE запрос на удаление заказа с id={id}");

            bool isDeleted = await _orderService.delete(id);
            if (!isDeleted)
            {
                Log.Warning($"Не удалось удалить заказ с id={id}");
                return NotFound("Ошибка удаления заказа");
            }

            return Ok("Заказ удалён");
        }
    }
}