using Serilog;
using Repositories;
using Entities.Order;
using Entities.Product;
using Entities.gRCP;
using DTOs.Order;

namespace Service
{
    public class OrderService
    {
        private readonly HttpClient _client;

        public OrderService(HttpClient httpClient, ILogger<OrderService> logger)
        {
            _client = httpClient;
            _client.BaseAddress = new Uri("http://localhost:5004");
            _client.DefaultRequestVersion = new Version(1, 1); // Только HTTP/1.1
            _client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact; // Никаких других версий
        }



        public async Task<IEnumerable<Order>> getAllbyUserId(int userId, int offset = 0)
        {

            var orders = await Orderdb.GetAllByUserId(userId, offset, 20);

            if (orders == null)
            {
                Log.Error($"Ошибка при получении заказов для user_id={userId}");
                return null;
            }

            Log.Information($"Успешно получено {orders.Count()} заказов для user_id={userId}");
            return orders;
        }


        public async Task<Order> getById(int id)
        {
            var order = await Orderdb.GetById(id);

            if (order == null)
            {
                Log.Information("Не удалось найти информацию о заказе");
                return null;
            }

            Log.Information("Успешное получение информации о заказе");
            return order;
        }


        public async Task<int> Create(int userId, List<productsIds> ProducstIds)
        {
            Log.Information("Создание заказа для пользователя {UserId}. Количество товаров: {ProductCount}", userId, ProducstIds.Count);

            var createOrderRequest = new CreateOrderDto
            {
                ProductsIds = ProducstIds
            };

            try
            {
                var response = await _client.PostAsJsonAsync("/api/products/Create-order", createOrderRequest);

                if (!response.IsSuccessStatusCode)
                {
                    Log.Warning($"Ошибка при создании заказа для пользователя {userId}: запрос не удался.");
                    return -1;
                }

                // Десериализуем ответ
                var result = await response.Content.ReadFromJsonAsync<CreateOrderResponse>();

                Log.Information(result.ToString());

                if (result == null || !result.Create)
                {
                    Log.Information("Некоторые товары недоступны в требуемом количестве");
                    return -1;
                }


                var orderId = await Orderdb.Create(userId, ProducstIds);

                if (orderId <= 0)
                {
                    Log.Warning($"Не удалось создать заказ для пользователя {userId}");
                    return -1;
                }

                Log.Information($"Успешное создание заказа с id {orderId} для пользователя {userId}");
                return orderId;
            }

            catch (Exception ex)
            {
                Log.Error($"Ошибка при создании заказа для пользователя {userId}\n{ex}");
                return -1;
            }
        }



        public async Task<bool> delete(int id)
        {
            Log.Information("Удаление заказа");

            var status = await Orderdb.delete(id);

            if (!status)
            {
                Log.Information("Не удалось удалить заказ");
                return false;
            }

            Log.Information("Успешное удаление заказа");
            return true;
        }
    }
}