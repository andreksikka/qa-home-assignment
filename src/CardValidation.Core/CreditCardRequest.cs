namespace CardValidation.Core;

public record CreditCardRequest(
    string CardOwner,
    string CardNumber,
    string Expiry,
    string CVC
);
