using CardValidation.Core.Enums;
namespace CardValidation.Core;

public record ValidationResult(
    bool IsValid,
    CardType? CardType,
    List<string> Errors
);

