using Microsoft.AspNetCore.Mvc.Testing;
using AngleSharp.Html.Parser;

namespace FreeMusicInstantlyTesting
{
    public class IntegrationTest1 : IClassFixture<WebApplicationFactory<Program>>
    {

        private readonly WebApplicationFactory<Program> _factory;

        public IntegrationTest1()
        {
            var factory = new WebApplicationFactory<Program>();
            _factory = factory;
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
        [Theory]
        [InlineData("/Songs ")]
        [InlineData("/Playlists")]
        [InlineData("/ApplicationUsers/MyProfile")]
        public async void TestLoginRequiredPagesLoad(string URL)
        {

            // Integration test
            // Log in as a default test user
            // Make the desired request

            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = true,
                HandleCookies = true
            });

            // Act

            // Step 1: Get the login page to extract the anti-forgery token
            var loginPageResponse = await client.GetAsync("/Identity/Account/Login");
            loginPageResponse.EnsureSuccessStatusCode();

            var loginPageContent = await loginPageResponse.Content.ReadAsStringAsync();
            var parser = new HtmlParser();
            var document = parser.ParseDocument(loginPageContent);
            var antiForgeryToken = document.QuerySelector("input[name='__RequestVerificationToken']")?.GetAttribute("value");

            if (string.IsNullOrEmpty(antiForgeryToken))
            {
                throw new Exception("Anti-forgery token not found.");
            }

            // Step 2: Prepare login form data
            var loginData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Input.Email", "user@test.com"),
                new KeyValuePair<string, string>("Input.Password", "User1!"),
                new KeyValuePair<string, string>("__RequestVerificationToken", antiForgeryToken),
                new KeyValuePair<string, string>("Input.RememberMe", "false")
            });

            // Step 3: Send the login request
            var loginResponse = await client.PostAsync("/Identity/Account/Login", loginData);

            // Log the response for debugging
            var responseContent = await loginResponse.Content.ReadAsStringAsync();

            // Ensure the login was successful
            loginResponse.EnsureSuccessStatusCode();

            // Step 4: Navigate to /Songs while authenticated
            var response = await client.GetAsync(URL);
            response.EnsureSuccessStatusCode();

            // Assert
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("artist@test.com", true)]
        [InlineData("Tzanca@test.com", false)]
        public async void ViewArtistTest(string expected_result, bool should_contain)
        {
            // Integration test
            // Log in as a default test user
            // Aaccess /ApplicationUsers/ViewArtists?search={search}

            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = true,
                HandleCookies = true
            });

            // Act

            // Step 1: Get the login page to extract the anti-forgery token
            var loginPageResponse = await client.GetAsync("/Identity/Account/Login");
            loginPageResponse.EnsureSuccessStatusCode();

            var loginPageContent = await loginPageResponse.Content.ReadAsStringAsync();
            var parser = new HtmlParser();
            var document = parser.ParseDocument(loginPageContent);
            var antiForgeryToken = document.QuerySelector("input[name='__RequestVerificationToken']")?.GetAttribute("value");

            if (string.IsNullOrEmpty(antiForgeryToken))
            {
                throw new Exception("Anti-forgery token not found.");
            }

            // Step 2: Prepare login form data
            var loginData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Input.Email", "user@test.com"),
                new KeyValuePair<string, string>("Input.Password", "User1!"),
                new KeyValuePair<string, string>("__RequestVerificationToken", antiForgeryToken),
                new KeyValuePair<string, string>("Input.RememberMe", "false")
            });

            // Step 3: Send the login request
            var loginResponse = await client.PostAsync("/Identity/Account/Login", loginData);

            // Log the response for debugging
            var responseContent = await loginResponse.Content.ReadAsStringAsync();

            // Ensure the login was successful
            loginResponse.EnsureSuccessStatusCode();

            // Step 4: Send the ViewArtists request with specifiend search temr
            var response = await client.GetAsync($"/ApplicationUsers/ViewArtists");

            responseContent = await response.Content.ReadAsStringAsync();

            // Assert based on the condition
            if (should_contain)
            {
                Assert.Contains(expected_result, responseContent);
            }
            else
            {
                Assert.DoesNotContain(expected_result, responseContent);
            }
        }

    }
}