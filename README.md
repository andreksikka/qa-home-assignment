# Credit Card Validation Microservice

A small .NET 8 microservice that validates credit card data and returns either:

- A **successful response** with detected card type  
  *(Visa, MasterCard, American Express)*  
- Or a **400 Bad Request** with all validation errors

This project includes:
- Full domain logic (`CardValidation.Core`)
- Minimal API host (`CardValidation.Web`)
- **80%+ unit test coverage**
- **Integration tests using Reqnroll (Cucumber-style BDD)**
- A Docker-based pipeline (work in progress)

---

## Features

### Credit Card Validation Rules
The API validates:

- ✔ Owner name format  
- ✔ Card number format (digits only)  
- ✔ Card type detection (prefix rules)  
- ✔ Number length per card type  
- ✔ Luhn algorithm correctness  
- ✔ CVC length per card type  
- ✔ Expiry date (not expired)  

---

## Test Coverage

### Unit Tests
Located in:
tests/CardValidation.UnitTests

Covers:
- Card owner validation  
- Card number rules  
- Luhn algorithm  
- CVC validation  
- Aggregated validator behavior  
(Passes: **34/34 tests**)

### Integration Tests (BDD)
Located in:
tests/CardValidation.IntegrationTests
Using:
- **Reqnroll** (SpecFlow-compatible)
- **WebApplicationFactory<Program>**
- Real HTTP calls against in-memory test server

Includes scenarios:
- Valid Visa card  
- Invalid card with multiple validation errors  
- Service returns all expected error details  
(Passes: **4/4 scenarios**)

### Running the Application Locally
dotnet run --project src/CardValidation.Web

### API available at:
POST http://localhost:5000/api/creditcards/validate
Example body:
{
  "cardOwner": "John Doe",
  "cardNumber": "4111111111111111",
  "expiry": "11/2025",
  "cvc": "123"
}

