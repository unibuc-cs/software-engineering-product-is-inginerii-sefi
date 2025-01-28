using Microsoft.AspNetCore.Mvc.Testing;

namespace FreeMusicInstantlyTesting
{
    public class UnitTest1 : IClassFixture<WebApplicationFactory<Program>>
    {

        private readonly WebApplicationFactory<Program> _factory;

        public UnitTest1()
        {
            var factory = new WebApplicationFactory<Program>();
            _factory = factory;
        }

        [Fact(Skip = "Test moved in theory test `TestPagesLoad`")]
        public async void LoadHomePageTest() 
        {
            // This is an exmaple of an integration test
            // We are checking that the app responds with a successful status code

            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/home");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/home")]
        [InlineData("/Identity/Account/Login")]
        public async void TestPagesLoad(string URL)
        {

            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(URL);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }
}