using CardValidation.Core.Enums;

namespace CardValidation.Core.Services.Interfaces;

public interface ICardValidationService
{
    bool ValidateOwner(string owner);
    bool ValidateIssueDate(string date);
    bool ValidateNumber(string number);
    bool ValidateNumberAndCvcForType(string number, string cvc);
    bool OwnerHasNoCardData(string owner, string number, string cvc);
    CardType GetCardType(string number);
}
