using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace RestaurantAPI.IntegrationTests
{
    public class RestaurantControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient _client;

        public RestaurantControllerTests(WebApplicationFactory<Startup> factory)
        {
            _client = factory
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var dbContextOptions = services
                            .SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));

                        services.Remove(dbContextOptions);

                        services
                         .AddDbContext<RestaurantDbContext>(options => options.UseInMemoryDatabase("RestaurantDb"));

                    });
                })
                .CreateClient();
        }

        [Theory]
        [InlineData("pageSize=5&pageNumber=1")]
        [InlineData("pageSize=15&pageNumber=2")]
        [InlineData("pageSize=10&pageNumber=3")]
        public async Task GetAll_WithQueryParameters_ReturnsOkResult(string queryParams)
        {
            // act

            var response = await _client.GetAsync("/api/restaurant?" + queryParams);
            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("pageSize=100&pageNumber=3")]
        [InlineData("pageSize=11&pageNumber=1")]
        [InlineData(null)]
        [InlineData("")]
        public async Task GetAll_WithInvalidQueryParams_ReturnsBadRequest(string queryParams)
        {


            // act

            var response = await _client.GetAsync("/api/restaurant?" + queryParams);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }
}
