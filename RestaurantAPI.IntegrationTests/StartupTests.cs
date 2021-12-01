using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RestaurantAPI.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace RestaurantAPI.IntegrationTests
{
    public class StartupTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly List<Type> _controllerTypes;
        private readonly WebApplicationFactory<Startup> _factory;

        public StartupTests(WebApplicationFactory<Startup> factory)
        {
            _controllerTypes = typeof(Startup)
                .Assembly
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(ControllerBase)))
                .ToList();

            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    _controllerTypes.ForEach(c => services.AddScoped(c));
                });
            });
        }

        [Fact]
        public void ConfigureServices_ForControllers_RegistersAllDependencies()
        {
            //arrange
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();



            // assert
            _controllerTypes.ForEach(t =>
            {
                var controller = scope.ServiceProvider.GetService(t);
                controller.Should().NotBeNull();

            });
        }
    }
}
