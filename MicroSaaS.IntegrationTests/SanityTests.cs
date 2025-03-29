using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using Xunit;

namespace MicroSaaS.IntegrationTests;

public class SanityTests
{
    [Fact]
    public void SimpleTest_AlwaysPasses()
    {
        // Arrange
        const bool expected = true;

        // Act
        const bool actual = true;

        // Assert
        Assert.Equal(expected, actual);
    }
} 