using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using System;

namespace MicroSaaS.Infrastructure.MongoDB
{
    public static class MongoDbInitializer
    {
        private static bool _initialized = false;
        private static readonly object _lock = new object();

        public static void Initialize()
        {
            if (_initialized)
                return;

            lock (_lock)
            {
                if (_initialized)
                    return;

                try
                {
                    // Configuração de serialização global para tipos específicos
                    BsonSerializer.TryRegisterSerializer(typeof(Guid), new GuidSerializer(BsonType.String));
                    BsonSerializer.TryRegisterSerializer(typeof(DateTime), new DateTimeSerializer(DateTimeKind.Utc));

                    // Define convenções globais para o MongoDB
                    var pack = new ConventionPack
                    {
                        // Usar camel case para nomes de propriedades
                        new CamelCaseElementNameConvention(),
                        
                        // Convenção para ignorar propriedades nulas
                        new IgnoreIfNullConvention(true),
                        
                        // Convenção para ignorar elementos extras
                        new IgnoreExtraElementsConvention(true)
                    };

                    // Registre o pacote de convenções para todas as classes da aplicação
                    ConventionRegistry.Register("MicroSaaS Conventions", pack, t => true);

                    Console.WriteLine("MongoDB: Inicialização de serializadores e convenções concluída com sucesso.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"MongoDB: Erro na inicialização: {ex.Message}");
                }

                _initialized = true;
            }
        }
    }
} 