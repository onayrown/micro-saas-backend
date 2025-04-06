namespace MicroSaaS.Infrastructure.Settings;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public int ConnectTimeout { get; set; } = 30000; // Timeout em milissegundos (30s)
    public int ServerSelectionTimeout { get; set; } = 30000; // Timeout em milissegundos (30s)

    public MongoDbSettings()
    {
    }

    public MongoDbSettings(string connectionString, string databaseName)
    {
        ConnectionString = connectionString;
        DatabaseName = databaseName;
    }
} 