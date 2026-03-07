using MySql.Data.MySqlClient;

namespace BetFounders.CentralBackend.Data.Database;

public class DbConnectionFactory(string connectionString)
{
    public MySqlConnection CreateConnection() => new(connectionString);
}