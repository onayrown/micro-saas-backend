using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using MicroSaaS.IntegrationTests.Utils;

namespace MicroSaaS.IntegrationTests
{
    /// <summary>
    /// Provedor personalizado de controlers que substitui certos controllers originais por nossas versões de teste
    /// </summary>
    public class TestControllerReplacementProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            // Remove o AuthController original
            var authControllerType = feature.Controllers.FirstOrDefault(t => 
                t.Name.Equals("AuthController", StringComparison.OrdinalIgnoreCase));
                
            if (authControllerType != null)
            {
                feature.Controllers.Remove(authControllerType);
            }
            
            // Adiciona o nosso TestAuthController
            feature.Controllers.Add(typeof(TestAuthController).GetTypeInfo());
        }
    }
} 