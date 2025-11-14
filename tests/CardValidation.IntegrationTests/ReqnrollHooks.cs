using Reqnroll;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CardValidation.IntegrationTests;

[Binding]
public sealed class ReqnrollHooks
{
    private readonly ScenarioContext _ctx;

    public ReqnrollHooks(ScenarioContext ctx)
    {
        _ctx = ctx;
        _ctx.Set(new WebApplicationFactory<Program>(), "factory");
    }
}
