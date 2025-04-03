using Npgsql;

namespace SQLDataContext
{
    public static class DataContext
    {
        private static readonly string ConnectionString;

        static DataContext()
        {
            ConnectionString = $"Host={ConnectString.Host};Port={ConnectString.Port};Database={ConnectString.Database};Username={ConnectString.Username};Password={ConnectString.Password};Encoding=UTF8;Include Error Detail=true";
        }

        public static async Task<NpgsqlConnection> GetConnectionAsync()
        {
            var connection = new NpgsqlConnection(ConnectionString);
            try
            {
                await connection.OpenAsync();
                return connection;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                throw;
            }
        }

        public static NpgsqlConnection GetConnection()
        {
            var connection = new NpgsqlConnection(ConnectionString);
            try
            {
                connection.Open();
                return connection;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                throw;
            }
        }
    }

    public static class ConnectString
    {
        public static string Host { get; } = "localhost";
        public static string Port { get; } = "5432";
        public static string Username { get; } = "postgres";
        public static string Password { get; } = "1234";
        public static string Database { get; } = "dhub5";
    }
}