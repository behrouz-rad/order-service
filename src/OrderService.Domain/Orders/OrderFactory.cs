// © 2025 Behrouz Rad. All rights reserved.

using FluentResults;
using OrderService.Domain.ValueObjects;

namespace OrderService.Domain.Orders;

public static class OrderFactory
{
    public static Result<Order> Create(
        string orderNumber,
        string invoiceAddress,
        string invoiceEmailAddress,
        string invoiceCreditCardNumber,
        IEnumerable<(string ProductId, string ProductName, int ProductAmount, decimal ProductPrice)> orderItems)
    {
        var invoiceAddressResult = InvoiceAddress.Create(invoiceAddress);
        var invoiceEmailAddressResult = InvoiceEmailAddress.Create(invoiceEmailAddress);
        var invoiceCreditCardNumberResult = InvoiceCreditCardNumber.Create(invoiceCreditCardNumber);

        var errors = new List<IError>();

        if (invoiceAddressResult.IsFailed)
        {
            errors.AddRange(invoiceAddressResult.Errors);
        }

        if (invoiceEmailAddressResult.IsFailed)
        {
            errors.AddRange(invoiceEmailAddressResult.Errors);
        }

        if (invoiceCreditCardNumberResult.IsFailed)
        {
            errors.AddRange(invoiceCreditCardNumberResult.Errors);
        }

        var domainOrderItems = new List<OrderItem>();
        foreach (var (productId, productName, productAmount, productPrice) in orderItems)
        {
            var itemResult = OrderItem.Create(productId, productName, productAmount, productPrice);

            if (itemResult.IsFailed)
            {
                errors.AddRange(itemResult.Errors);
            }
            else
            {
                domainOrderItems.Add(itemResult.Value);
            }
        }

        if (errors.Count > 0)
        {
            return Result.Fail<Order>(errors);
        }

        return Order.Create(
            orderNumber,
            invoiceAddressResult.Value,
            invoiceEmailAddressResult.Value,
            invoiceCreditCardNumberResult.Value,
            domainOrderItems);
    }
}
