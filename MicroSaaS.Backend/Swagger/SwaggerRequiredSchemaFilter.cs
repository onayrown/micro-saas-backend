using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace MicroSaaS.Backend.Swagger
{
    /// <summary>
    /// Filtro personalizado para o Swagger que corrige problemas com propriedades requeridas
    /// que podem causar erros de serialização.
    /// </summary>
    public class SwaggerRequiredSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema.Properties == null || schema.Properties.Count == 0)
            {
                return;
            }

            // Limpa a lista de propriedades requeridas para evitar problemas de serialização
            // com tipos complexos como TimeSpan e estruturas circulares
            if (schema.Required != null && schema.Required.Count > 0 && 
                (context.Type.Namespace?.StartsWith("MicroSaaS.Shared") == true ||
                 context.Type.Namespace?.StartsWith("MicroSaaS.Domain") == true))
            {
                // Remove requisitos para tipos que possam causar problemas
                var problematicTypes = new[] { typeof(TimeSpan), typeof(Guid) };
                
                var properties = context.Type.GetProperties();
                foreach (var property in properties)
                {
                    if (problematicTypes.Contains(property.PropertyType) && 
                        schema.Required.Contains(property.Name))
                    {
                        schema.Required.Remove(property.Name);
                    }
                }
            }
        }
    }
} 