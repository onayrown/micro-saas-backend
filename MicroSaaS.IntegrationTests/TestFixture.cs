using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Net.Http;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace MicroSaaS.IntegrationTests
{
    public class TestFixture : IDisposable
    {
        private readonly SharedTestFactory _factory;

        public HttpClient Client { get; }

        public TestFixture()
        {
            // Usar a SharedTestFactory para evitar duplicação
            _factory = new SharedTestFactory();
            Client = _factory.CreateClient();
        }

        public string GetValidToken()
        {
            // Retornar um token válido fixo para os testes
            return "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZXN0ZUBtaWNyb3NhYXMuY29tIiwianRpIjoiYWJjMTIzIiwiaWF0IjoxNTE2MjM5MDIyLCJuYmYiOjE1MTYyMzkwMjIsImV4cCI6MjUxNjIzOTAyMiwiYXVkIjoibWljcm9zYWFzLmNvbSIsImlzcyI6Im1pY3Jvc2Fhcy5jb20ifQ.VY6JUOK9gH3AQJl0KEhHYQ5MURKVc18WA5qmVxpWHaE";
        }

        public void Dispose()
        {
            Client.Dispose();
            _factory.Dispose();
        }
    }
} 