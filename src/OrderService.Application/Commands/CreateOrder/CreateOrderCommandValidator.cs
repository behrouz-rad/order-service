// © 2025 Behrouz Rad. All rights reserved.

using FluentValidation;
using OrderService.Application.DTOs;

namespace OrderService.Application.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.OrderDto)
            .NotNull()
            .WithMessage("Order data is required");

        RuleFor(x => x.OrderDto.InvoiceAddress)
            .NotEmpty()
            .WithMessage("Invoice address is required")
            .MinimumLength(10)
            .WithMessage("Invoice address must be at least 10 characters long");

        RuleFor(x => x.OrderDto.InvoiceEmailAddress)
            .NotEmpty()
            .WithMessage("Invoice email address is required")
            .EmailAddress()
            .WithMessage("Invalid email format");

        RuleFor(x => x.OrderDto.InvoiceCreditCardNumber)
            .NotEmpty()
            .WithMessage("Invoice credit card number is required")
            .Must(BeValidCreditCard)
            .WithMessage("Invalid credit card format");

        RuleFor(x => x.OrderDto.Products)
            .NotNull()
            .WithMessage("Products are required")
            .NotEmpty()
            .WithMessage("At least one product is required");

        RuleForEach(x => x.OrderDto.Products)
            .SetValidator(new CreateOrderItemDtoValidator());
    }

    private static bool BeValidCreditCard(string creditCardNumber)
    {
        if (string.IsNullOrWhiteSpace(creditCardNumber))
            return false;

        var cleanNumber = creditCardNumber.Replace("-", "").Replace(" ", "");
        return cleanNumber.Length >= 13 && cleanNumber.Length <= 19 && cleanNumber.All(char.IsDigit);
    }
}

public class CreateOrderItemDtoValidator : AbstractValidator<CreateOrderItemDto>
{
    public CreateOrderItemDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required");

        RuleFor(x => x.ProductName)
            .NotEmpty()
            .WithMessage("Product name is required");

        RuleFor(x => x.ProductAmount)
            .GreaterThan(0)
            .WithMessage("Product amount must be greater than zero");

        RuleFor(x => x.ProductPrice)
            .GreaterThan(0)
            .WithMessage("Product price must be greater than zero");
    }
}
