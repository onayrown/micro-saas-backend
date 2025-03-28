using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace MicroSaaS.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SwaggerDebugController : ControllerBase
    {
        private readonly ISwaggerProvider _swaggerProvider;
        
        public SwaggerDebugController(ISwaggerProvider swaggerProvider)
        {
            _swaggerProvider = swaggerProvider;
        }
        
        [HttpGet("test")]
        public IActionResult TestSwaggerGeneration()
        {
            try
            {
                // Tentar gerar o documento Swagger
                var document = _swaggerProvider.GetSwagger("v1");
                
                // Se chegar aqui, a geração foi bem-sucedida
                return Ok(new { 
                    message = "Swagger document generated successfully", 
                    paths = document.Paths.Count,
                    schemas = document.Components.Schemas.Count
                });
            }
            catch (Exception ex)
            {
                // Se houver um erro, capturar e retornar detalhes
                return StatusCode(500, new { 
                    error = "Failed to generate Swagger document", 
                    message = ex.Message,
                    stackTrace = ex.StackTrace,
                    innerException = ex.InnerException?.Message
                });
            }
        }
        
        [HttpGet("manual")]
        public IActionResult GetManualSwaggerJson()
        {
            // Criar um documento OpenAPI mínimo
            var openApiDoc = new OpenApiDocument
            {
                Info = new OpenApiInfo
                {
                    Title = "MicroSaaS API",
                    Version = "v1",
                    Description = "API para gestão de conteúdo para criadores"
                },
                Paths = new OpenApiPaths(),
                Components = new OpenApiComponents()
            };
            
            // Adicionar um caminho /api/auth/login
            var authLoginPath = new OpenApiPathItem();
            var postOperation = new OpenApiOperation
            {
                Summary = "Login de usuário",
                OperationId = "login",
                Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Auth" } },
                Responses = new OpenApiResponses
                {
                    ["200"] = new OpenApiResponse
                    {
                        Description = "Login bem-sucedido"
                    }
                }
            };
            
            authLoginPath.Operations = new Dictionary<OperationType, OpenApiOperation>
            {
                { OperationType.Post, postOperation }
            };
            
            openApiDoc.Paths.Add("/api/auth/login", authLoginPath);
            
            // Serializar para JSON
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            var json = System.Text.Json.JsonSerializer.Serialize(openApiDoc, jsonOptions);
            
            return Content(json, "application/json");
        }
    }
} 