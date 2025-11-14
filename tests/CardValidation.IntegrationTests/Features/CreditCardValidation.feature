Feature: CreditCardValidationAPI
  Validates credit card data and returns either validation errors or detected card type.

  Scenario: Valid Visa card
    Given a credit card request:
      | owner    | number           | date    | cvc |
      | Jane Doe | 4111111111111111 | 11/2025 | 123 |
    When I POST the request to /api/creditcards/validate
    Then the response status code should be 200
    And the response should contain "Visa"

  Scenario: Invalid card with multiple errors
    Given a credit card request:
      | owner        | number | date    | cvc |
      | John 123 Doe | 1234   | 01/2020 | 99  |
    When I POST the request to /api/creditcards/validate
    Then the response status code should be 400
    And the response should contain "errors"
