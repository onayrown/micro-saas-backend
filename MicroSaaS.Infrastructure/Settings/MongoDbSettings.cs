namespace MicroSaaS.Infrastructure.Settings;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
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