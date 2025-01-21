Key Features

(1) Data Annotation and FluentValidation Integration:
      Data Annotations for basic validations like "Required."
      FluentValidation for advanced rules (e.g., password requirements, product name validation).
      
(2) Validation Behavior:
      => Fields like Password and ProductName use FluentValidation to enforce rules such as:
          Password must include one uppercase letter, one lowercase letter.
          Product name must contain only letters.
      => Fields like ProductPrice and OrderNumber etc.. default to 0 but do not allow 0 as a valid value.

(3) Validation Flow:
      => If a user clicks the "Add" button without entering any value:
          Data Annotation: Triggers first to show required field messages.
          FluentValidation: Applies rules for specific fields after basic validation.
      => For fields with default values (0), Data Annotations ensure 0 is treated as invalid and trigger appropriate error messages.

For more information about using FluentValidation in MVC architecture, visit this website
https://dotnettutorials.net/lesson/fluent-api-in-asp-net-core-mvc/
