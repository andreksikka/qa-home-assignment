using System.Net.Http.Json;
using FluentAssertions;
using Reqnroll;
using Microsoft.AspNetCore.Mvc.Testing;
using CardValidation.Core; 

namespace CardValidation.IntegrationTests.StepDefinitions;

[Binding]
public class CreditCardValidationSteps
{
    private readonly WebApplicationFactory<Program> _factory;
    private HttpClient _client;
    private HttpResponseMessage _response = default!;

    private Dictionary<string, string> _request;

    public CreditCardValidationSteps(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("http://localhost")
        });
    }

    [Given(@"a credit card request:")]
    public void GivenACreditCardRequest(Table table)
    {
        var row = table.Rows[0];

      _request = new Dictionary<string, string>
{
    { "owner", row["owner"] },
    { "number", row["number"] },
    { "date", row["date"] },
    { "cvc", row["cvc"] }
};
    }

    [When(@"^I POST the request to /api/creditcards/validate$")]
    public async Task WhenIPOST()
    {
        _response = await _client.PostAsJsonAsync("/api/creditcards/validate", _request);
    }

    [Then(@"the response status code should be (.*)")]
    public void ThenStatusCodeShouldBe(int expected)
    {
        ((int)_response.StatusCode).Should().Be(expected);
    }

    [Then(@"the response should contain ""(.*)""")]
    public async Task ThenResponseShouldContain(string expected)
    {
        var json = await _response.Content.ReadAsStringAsync();
        json.Should().Contain(expected);
    }
}
