using Entities.Order;
using Serilog;

namespace Repositories
{
    public class Orderdb
    {
        public static IEnumerable<Order> getAllByUserId(int userId, string Status = "all")
        {
            const string query = "";

            try
            {
                Log.Information($"Запрос на получение всех заказов пользователя с id: {userId} со статусом: {Status}");

                List<Order> orders = new List<Order>();


                return orders;
            }

            catch (Exception ex)
            {
                Log.Fatal($"Ошибка при обращении к базе данных\n{ex}");
                return null;
            }
        }

        public static Order getbyId(int id)
        {
            const string query = "";

            try
            {
                Log.Information($"Запрос на получение заказа по id: {id}");

                var Order = new Order()
                {
                    Name = "",
                    gds = {}
                };

                return Order;
            }

            catch (Exception ex)
            {
                Log.Fatal($"Ошибка при обращении к базе данных\n{ex}");
                return null;
            }
        }

        public static int create()
        {
            const string query = "";

            try
            {
                Log.Information("Запрос на создание заказа");


                return -1;

            }

            catch (Exception ex)
            {
                Log.Fatal($"Ошибка при обращении к базе данных\n{ex}");
                return -1;
            }
        }

        public static bool delete(int id)
        {
            const string query = "";

            try
            {
                Log.Information($"Запрос на удаление заказа по id: {id}");

                return false;
            }

            catch (Exception ex)
            {
                Log.Fatal($"Ошибка при обращении к базе данных\n{ex}");
                return false;
            }
        }
    }
}