using System;
using Microsoft.AspNetCore.Hosting;
using Xunit;

// Definir a visibilidade de internals para o framework de teste Mvc.Testing
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.AspNetCore.Mvc.Testing")]

// Configuração para assegurar que esta é uma biblioteca de testes
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("xunit")] 