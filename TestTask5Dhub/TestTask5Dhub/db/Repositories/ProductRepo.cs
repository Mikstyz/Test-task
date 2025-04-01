using Entities.Product;
using Npgsql;
using Serilog;
using SQLDataContext;

namespace Repositories.Product
{
    public class Productdb
    {
        public static async Task<IEnumerable<product>> getOffsset(int Offset, int limit = 20)
        {
            const string query = @"SELECT seller, name, description, price, quantity FROM products ORDER BY name LIMIT @limit OFFSET @offset";
            
            try
            {
                Log.Information("Запрос на получение всех продуктов");

                await using var conn = await DataContext.GetConnectionAsync();
                await using var cmd = new NpgsqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@limit", limit);
                cmd.Parameters.AddWithValue("@Offset", Offset);

                using var reader = await cmd.ExecuteReaderAsync();
                {
                    List<product> products = new List<product>();

                    while (await reader.ReadAsync())
                    {
                        products.Add(new product()
                        {
                            Seller = reader.GetString(0),
                            Name = reader.GetString(1),
                            Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Price = reader.GetDouble(3),
                            Quantity = reader.GetInt32(4),
                        });
                    }

                    return products.ToArray();
                }
            }

            catch (Exception ex)
            {
                Log.Fatal($"Ошибка при обращении к базе данных\n{ex}");
                return null;
            }
        }

        public static async Task<product> getId(int id)
        {
            const string query = "SELECT seller, name, description, price, quantity FROM products WHERE id = @id";

            try
            {
                Log.Information("Запрос на получение продукта по id");

                await using var conn = await DataContext.GetConnectionAsync();
                await using var cmd = new NpgsqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id", id);

                using var reader = await cmd.ExecuteReaderAsync();
                {
                    var product = new product()
                    {
                        Seller = reader.GetString(0),
                        Name = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Price = reader.GetDouble(3),
                        Quantity = reader.GetInt32(4),
                    };

                    return product;
                }
            }

            catch (Exception ex)
            {
                Log.Fatal($"Ошибка при обращении к базе данных\n{ex}");
                return null;
            }
        }

        public static async Task<IEnumerable<product>> search(int offset, int limit = 20, string seller = null,string name = null, double? price = 0, int? quantity = 0)
        {
            #region filter
            List<string> conditions = new List<string>();
            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();
            int paramIndex = 0;

            if (!string.IsNullOrEmpty(seller))
            {
                conditions.Add($"seller ILIKE @p{paramIndex}");
                parameters.Add(new NpgsqlParameter($"@p{paramIndex++}", $"%{seller}%"));
            }

            if (!string.IsNullOrEmpty(name))
            {
                conditions.Add($"name ILIKE @p{paramIndex}");
                parameters.Add(new NpgsqlParameter($"@p{paramIndex++}", $"%{name}%"));
            }

            if (price.HasValue)
            {
                conditions.Add($"price = @p{paramIndex}");
                parameters.Add(new NpgsqlParameter($"@p{paramIndex++}", price.Value));
            }
            if (quantity.HasValue)
            {
                conditions.Add($"quantity = @p{paramIndex}");
                parameters.Add(new NpgsqlParameter($"@p{paramIndex++}", quantity.Value));
            }
            #endregion

            string query = conditions.Any()
                ? $"SELECT seller, name, description, price, quantity FROM products WHERE {string.Join(" AND ", conditions)} ORDER BY name LIMIT @limit OFFSET @offset"
                : $"SELECT seller, name, description, price, quantity FROM products ORDER BY name LIMIT @limit OFFSET @offset";

            try
            {
                Log.Information("Запрос на поиск продуктов");
                await using var conn = await DataContext.GetConnectionAsync();
                await using var cmd = new NpgsqlCommand(query, conn);

                if (parameters.Any())
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                }

                cmd.Parameters.AddWithValue("@limit", limit);
                cmd.Parameters.AddWithValue("@Offset", offset);

                var products = new List<product>();

                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    products.Add(new product
                    {
                        Seller = reader.GetString(0),
                        Name = reader.GetString(1),
                        Description = reader.GetString(2),
                        Price = reader.GetDouble(3),
                        Quantity = reader.GetInt32(4)
                    });
                }

                return products.ToArray();
            }

            catch (Exception ex)
            {
                Log.Fatal($"Ошибка при обращении к базе данных \n{ex}");
                return null;
            }
        }

        public static async Task<int> create(string seller,string name, string Description, double price, int quantity)
        {
            const string query = "INSERT INTO products (seller, name, description, price, quantity) VALUES (@seller, @name, @description, @price, @quantity) RETURNING id";

            if (string.IsNullOrEmpty(name) || price < 0 || quantity < 0)
            {
                Log.Error("Некорректные данные для создания продукта");
                return -1;
            }

            try
            {
                Log.Information("Запрос на создание продукта");

                await using var conn = await DataContext.GetConnectionAsync();
                await using var cmd = new NpgsqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@seller", seller);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@description", Description);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.Parameters.AddWithValue("@quantity", quantity);

                var result = await cmd.ExecuteScalarAsync();

                if (result == null || result == DBNull.Value)
                {
                    Log.Error("Не удалось создать товар");
                    return -1;
                }

                Log.Information("Успешное обновление товара");
                return (int)result;
            }

            catch (Exception ex)
            {
                Log.Fatal($"Ошибка при обращении к базе данных\n{ex}");
                return -1;
            }
        }

        public static async Task<bool> Update(int id, string newName, string newDescription, double newPrice, int newQuantity)
        {
            const string query = "UPDATE products SET name = @newName, description = @newDescription, price = @newPrice, quantity = @newQuantity WHERE id = @id";

            if (id <= 0 || string.IsNullOrEmpty(newName) || newPrice < 0 || newQuantity < 0)
            {
                Log.Error($"Некорректные данные для обновления продукта: id={id}, name={newName}, price={newPrice}, quantity={newQuantity}");
                return false;
            }

            try
            {
                Log.Information($"Запрос на обновление продукта по id: {id}");

                await using var conn = await DataContext.GetConnectionAsync();
                await using var cmd = new NpgsqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@newName", newName);
                cmd.Parameters.AddWithValue("@newDescription", string.IsNullOrEmpty(newDescription) ? DBNull.Value : newDescription);
                cmd.Parameters.AddWithValue("@newPrice", newPrice);
                cmd.Parameters.AddWithValue("@newQuantity", newQuantity);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                {
                    Log.Information($"Продукт с id {id} успешно обновлён");
                    return true;
                }

                Log.Warning($"Продукт с id {id} не найден для обновления");
                return false;
            }
            catch (Exception ex)
            {
                Log.Fatal($"Ошибка при обращении к базе данных \n{ex}");
                return false;
            }
        }

        public static async Task<bool> Delete(int id)
        {
            const string query = "DELETE FROM products WHERE id = @id";

            if (id <= 0)
            {
                Log.Error($"Некорректный id для удаления: {id}");
                return false;
            }

            try
            {
                Log.Information($"Запрос на удаление продукта по id: {id}");

                await using var conn = await DataContext.GetConnectionAsync();
                await using var cmd = new NpgsqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id", id);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                {
                    Log.Information($"Продукт с id {id} успешно удалён");
                    return true;
                }

                Log.Warning($"Продукт с id {id} не найден для удаления");
                return false;
            }

            catch (Exception ex)
            {
                Log.Fatal($"Ошибка при обращении к базе данных \n{ex}");
                return false;
            }
        }
    }
}