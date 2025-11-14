using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using CardValidation.Core;

public class CreditCardIntegrationTests :
    IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CreditCardIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]

public async Task Valid_Visa_Returns_OK()
{
    var request = new CreditCardRequest(
        "Jane Doe",
        "4111111111111111",
        "11/2026",
        "123"
    );

    var payload = new
    {
        Owner = request.CardOwner,
        Number = request.CardNumber,
        Date = request.Expiry,
        Cvc = request.CVC
    };

    var response = await _client.PostAsJsonAsync("/api/creditcards/validate", payload);

    response.EnsureSuccessStatusCode();

    var json = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
    Assert.NotNull(json);
    Assert.Equal("Visa", json!["cardType"]);
}


    [Fact]
   public async Task Invalid_Card_Returns_BadRequest()
{
    var request = new CreditCardRequest(
        "",
        "999",
        "01/2010",
        "1"
    );

    var payload = new
    {
        Owner = request.CardOwner,
        Number = request.CardNumber,
        Date = request.Expiry,
        Cvc = request.CVC
    };

    var response = await _client.PostAsJsonAsync("/api/creditcards/validate", payload);
    Assert.False(response.IsSuccessStatusCode);

    var json = await response.Content.ReadFromJsonAsync<Dictionary<string, string[]>>();
    Assert.NotNull(json);
    Assert.True(json!["errors"].Length > 0);
}
}
