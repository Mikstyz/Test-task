using Entities.User;
using Npgsql;
using Serilog;
using SQLDataContext;

namespace Repositories.User
{
    public class Userdb
    {
        public static async Task<int> Register(string name, string user_login, string user_password)
        {
            const string query = "INSERT INTO users (name, user_login, user_password) VALUES (@name, @user_login, @user_password) RETURNING id";

            try
            {
                Log.Information("Запрос в бд на создание пользователя");

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user_password);

                await using var conn = await DataContext.GetConnectionAsync();
                await using var cmd = new NpgsqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@user_login", user_login);
                cmd.Parameters.AddWithValue("@user_password", hashedPassword);

                var result = await cmd.ExecuteScalarAsync();

                if (result == null || result == DBNull.Value)
                {
                    Log.Error("Не удалось зарегистрировать пользователя");
                    return -1;
                }

                return (int)result;
            }
            catch (Exception ex)
            {
                Log.Fatal("Ошибка при обращении к базе данных: {ErrorMessage}", ex.Message);
                return -1;
            }
        }

        public static async Task<user> Authorization(string user_login, string user_password)
        {
            const string query = "SELECT name, user_login, user_password FROM users WHERE user_login = @user_login AND user_password = @user_password";

            try
            {
                Log.Information("Запрос в бд на авторизацию пользователя");

                await using var conn = await DataContext.GetConnectionAsync();
                await using var cmd = new NpgsqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@user_login", user_login);
                cmd.Parameters.AddWithValue("@user_password", user_password);

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    string storedHash = reader.GetString(2);

                    if (BCrypt.Net.BCrypt.Verify(user_password, storedHash))
                    {
                        return new user()
                    {
                        Name = reader.GetString(0),
                        Login = reader.GetString(1)
                    };
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Log.Fatal("Ошибка при обращении к базе данных: {ErrorMessage}", ex.Message);
                return null;
            }
        }

        public static async Task<bool> Delete(int id)
        {
            const string query = "DELETE FROM users WHERE id = @id";

            try
            {
                Log.Information("Запрос в бд на удаление пользователя");

                await using var conn = await DataContext.GetConnectionAsync();
                await using var cmd = new NpgsqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id", id);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    Log.Error("Не удалось удалить пользователя");
                    return false;
                }

                Log.Information("Успешное удаление пользователя");
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