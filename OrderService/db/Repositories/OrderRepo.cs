using Entities.Order;
using Npgsql;
using Serilog;
using SQLDataContext;

namespace Repositories
{
    public class Orderdb
    {
        public static async Task<IEnumerable<Order>> getAllByUserId(int userId, int offset = 0, int limit = 20)
        {
            const string query = @"SELECT o.id, o.user_id, 
                                    COALESCE(array_agg(op.product_id) FILTER (WHERE op.product_id IS NOT NULL), '{}') AS product_ids
                                    FROM orders o
                                    LEFT JOIN order_products op ON o.id = op.order_id
                                    WHERE o.user_id = @user_id
                                    GROUP BY o.id, o.user_id
                                    ORDER BY o.id
                                    LIMIT @limit OFFSET @offset";

            try
            {
                Log.Information($"Запрос на получение всех заказов пользователя с id={userId}, offset={offset}, limit={limit}");

                await using var conn = await DataContext.GetConnectionAsync();
                await using var cmd = new NpgsqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@user_id", userId);
                cmd.Parameters.AddWithValue("@limit", limit);
                cmd.Parameters.AddWithValue("@offset", offset);

                await using var reader = await cmd.ExecuteReaderAsync();

                List<Order> orders = new List<Order>();

                while (await reader.ReadAsync())
                {
                    orders.Add(new Order()
                    {
                        Id = reader.GetInt32(0),
                        UserId = reader.GetInt32(1),
                        ProductIds = reader.IsDBNull(2) ? new List<int>() : ((int[])reader.GetValue(2)).Where(id => id != null).ToList()});
                }

                if (orders.Count == 0)
                {
                    Log.Warning($"Заказы для пользователя с id={userId} не найдены");
                }

                else
                {
                    Log.Information($"Найдено {orders.Count} заказов для пользователя с id={userId}");
                }

                return orders;
            }

            catch (Exception ex)
            {
                Log.Fatal($"Ошибка при обращении к базе данных\n{ex}");
                return null;
            }
        }


        public static async Task<Order> GetById(int id)
        {
            const string query = @"
                    SELECT o.id, o.user_id, 
                    COALESCE(array_agg(op.product_id) FILTER (WHERE op.product_id IS NOT NULL), '{}') AS product_ids
                    FROM orders o
                    LEFT JOIN order_products op ON o.id = op.order_id
                    WHERE o.id = @id
                    GROUP BY o.id, o.user_id;";

            try
            {
                Log.Information($"Запрос на получение заказа по id={id}");

                await using var conn = await DataContext.GetConnectionAsync();
                await using var cmd = new NpgsqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id", id);

                await using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var order = new Order()
                    {
                        Id = reader.GetInt32(0),
                        UserId = reader.GetInt32(1),
                        ProductIds = reader.IsDBNull(2) ? new List<int>() : reader.GetFieldValue<int[]>(2).ToList()
                    };

                    Log.Information($"Заказ с id={id} успешно получен");
                    return order;
                }

                Log.Warning($"Заказ с id={id} не найден");
                return null;
            }

            catch (Exception ex)
            {
                Log.Fatal($"Ошибка при обращении к базе данных: {ex}");
                return null;
            }
        }


        public static async Task<int> Create(int userId, int[] productIds)
        {
            Log.Information($"Параметры запроса: user_id={userId}, product_ids={string.Join(",", productIds)}");

            const string query = @"
                            BEGIN;
                            WITH new_order AS (
                                INSERT INTO orders (user_id)
                                VALUES (@user_id)
                                RETURNING id
                            )
                            SELECT id FROM new_order;
                            COMMIT;";

            try
            {
                Log.Information($"Запрос на создание заказа для user_id={userId} с товарами={string.Join(",", productIds)}");

                await using var conn = await DataContext.GetConnectionAsync();
                await using var cmd = new NpgsqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@user_id", userId);

                var orderId = await cmd.ExecuteScalarAsync();

                if (orderId == null || orderId == DBNull.Value)
                {
                    Log.Error($"Не удалось создать заказ для user_id={userId}");
                    return -1;
                }

                int newOrderId = (int)orderId;

                // Теперь добавляем товары в заказ
                const string insertProductsQuery = @"
        INSERT INTO order_products (order_id, product_id)
        SELECT @order_id, unnest(@product_ids);";

                await using var cmdProducts = new NpgsqlCommand(insertProductsQuery, conn);
                cmdProducts.Parameters.AddWithValue("@order_id", newOrderId);
                cmdProducts.Parameters.AddWithValue("@product_ids", productIds);

                await cmdProducts.ExecuteNonQueryAsync();

                Log.Information($"Успешно создан заказ для user_id={userId} с id={newOrderId}");
                return newOrderId;
            }
            catch (Exception ex)
            {
                Log.Fatal($"Ошибка при обращении к базе данных: {ex}");
                return -1;
            }
        }


        public static async Task<bool> delete(int id)
        {
            const string query = @"DELETE FROM order_products WHERE order_id = @id; DELETE FROM orders WHERE id = @id;";

            try
            {
                Log.Information($"Запрос на удаление заказа по id={id}");

                await using var conn = await DataContext.GetConnectionAsync();
                await using var cmd = new NpgsqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id", id);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    Log.Error($"Не удалось удалить заказ с id={id}, возможно, его нет");
                    return false;
                }

                Log.Information($"Заказ с id={id} успешно удалён");
                return true;
            }

            catch (Exception ex)
            {
                Log.Fatal($"Ошибка при обращении к базе данных\n{ex}");
                return false;
            }
        }
    }
}