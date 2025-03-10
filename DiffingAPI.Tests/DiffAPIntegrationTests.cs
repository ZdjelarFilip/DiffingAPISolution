using System.Net.Http.Json;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DiffingAPI.Tests
{
    public class DiffAPIIntegrationTests
    {
        private WebApplicationFactory<Program> CreateFactory()
        {
            return new WebApplicationFactory<Program>();
        }

        [Fact]
        public async Task DiffEndpoints_ShouldWorkCorrectly()
        {
            using var factory = CreateFactory();
            using var client = factory.CreateClient();

            string id = "1";
            var leftData = new { Data = Convert.ToBase64String(new byte[] { 1, 2, 3 }) };
            var rightData = new { Data = Convert.ToBase64String(new byte[] { 1, 3, 3 }) };

            // Upload left side
            var leftResponse = await client.PutAsJsonAsync($"/v1/diff/{id}/left", leftData);
            Assert.Equal(HttpStatusCode.Created, leftResponse.StatusCode);

            // Check 404 before right is uploaded
            var earlyResponse = await client.GetAsync($"/v1/diff/{id}");
            Assert.Equal(HttpStatusCode.NotFound, earlyResponse.StatusCode);

            // Upload right side
            var rightResponse = await client.PutAsJsonAsync($"/v1/diff/{id}/right", rightData);
            Assert.Equal(HttpStatusCode.Created, rightResponse.StatusCode);

            // Get diff result
            var response = await client.GetAsync($"/v1/diff/{id}");
            var result = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("ContentDoNotMatch", result);
        }

        [Fact]
        public async Task UploadingNullData_ShouldReturnBadRequest()
        {
            using var factory = CreateFactory();
            using var client = factory.CreateClient();

            string id = "2";
            var invalidData = new { Data = (string?)null };

            var leftResponse = await client.PutAsJsonAsync($"/v1/diff/{id}/left", invalidData);
            var rightResponse = await client.PutAsJsonAsync($"/v1/diff/{id}/right", invalidData);

            Assert.Equal(HttpStatusCode.BadRequest, leftResponse.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, rightResponse.StatusCode);
        }

        [Fact]
        public async Task SameData_ShouldReturnEquals()
        {
            using var factory = CreateFactory();
            using var client = factory.CreateClient();

            string id = "3";
            var data = new { Data = Convert.ToBase64String(new byte[] { 1, 2, 3 }) };

            await client.PutAsJsonAsync($"/v1/diff/{id}/left", data);
            await client.PutAsJsonAsync($"/v1/diff/{id}/right", data);

            var response = await client.GetAsync($"/v1/diff/{id}");
            var result = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("Equals", result);
        }

        [Fact]
        public async Task DifferentSize_ShouldReturnSizeDoNotMatch()
        {
            using var factory = CreateFactory();
            using var client = factory.CreateClient();

            string id = "4";
            var leftData = new { Data = Convert.ToBase64String(new byte[] { 1, 2, 3 }) };
            var rightData = new { Data = Convert.ToBase64String(new byte[] { 1, 2 }) };

            await client.PutAsJsonAsync($"/v1/diff/{id}/left", leftData);
            await client.PutAsJsonAsync($"/v1/diff/{id}/right", rightData);

            var response = await client.GetAsync($"/v1/diff/{id}");
            var result = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("SizeDoNotMatch", result);
        }
    }
}
