using Microsoft.AspNetCore.OpenApi;
using CardValidation.Core.Services;
using CardValidation.Core.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICardValidationService, CardValidationService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/api/creditcards/validate", (ICardValidationService svc, RequestDto req) =>
{
    var errors = new List<string>();

    if (string.IsNullOrWhiteSpace(req.Owner)) errors.Add("Owner is required.");
    if (string.IsNullOrWhiteSpace(req.Number)) errors.Add("Number is required.");
    if (string.IsNullOrWhiteSpace(req.Date)) errors.Add("Date is required.");
    if (string.IsNullOrWhiteSpace(req.Cvc)) errors.Add("CVC is required.");

    if (!errors.Any())
    {
        if (!svc.ValidateOwner(req.Owner)) errors.Add("Owner format is invalid (only letters, 1–3 words).");
        if (!svc.ValidateIssueDate(req.Date)) errors.Add("Card is expired or issue date is invalid.");
        if (!svc.ValidateNumber(req.Number)) errors.Add("Number is invalid (BIN/length/Luhn).");
        else
        {
            try
            {
                var type = svc.GetCardType(req.Number);
                if (!svc.ValidateNumberAndCvcForType(req.Number, req.Cvc))
                    errors.Add($"CVC is invalid for {type}.");
            }
            catch
            {
                errors.Add("Unsupported card type or invalid number.");
            }
        }
        if (!svc.OwnerHasNoCardData(req.Owner, req.Number, req.Cvc))
            errors.Add("Owner must not contain card number fragments or CVC.");
    }

    if (errors.Any()) return Results.BadRequest(new { errors });
    return Results.Ok(new { cardType = svc.GetCardType(req.Number).ToString() });
})
.WithOpenApi()    // ← see töötab nüüd
.WithName("ValidateCreditCard")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest);

app.Run();

public record RequestDto(string Owner, string Number, string Date, string Cvc);
public partial class Program { }
