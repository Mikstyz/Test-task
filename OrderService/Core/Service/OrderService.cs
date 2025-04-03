using Serilog;
using Repositories;
using Entities.Order;

namespace Service
{
    public class OrderService
    {
        public async Task<IEnumerable<Order>> getAllbyUserId(int userId, int offset = 0)
        {

            var orders = await Orderdb.getAllByUserId(userId, offset, 20);

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


        public async Task<int> create(int userId, int[] ProductIds)
        {
            Log.Information("Создание товара");


            var OrderId = await Orderdb.Create(userId, ProductIds);

            if (OrderId <= 0)
            {
                Log.Information("Не удалось создать товар");
                return -1;
            }

            Log.Information("Успешное создание товара");
            return OrderId;
        }


        public async Task<bool> delete(int id)
        {
            Log.Information("Удаление товара");

            var status = await Orderdb.delete(id);

            if (!status)
            {
                Log.Information("Не удалось удалить товар");
                return false;
            }

            Log.Information("Успешное удаление товара");
            return true;
        }
    }
}