// © 2025 Behrouz Rad. All rights reserved.

using Mapster;
using OrderService.Application.DTOs;
using OrderService.Domain.Orders;

namespace OrderService.Application.Mappings;

public class OrderMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Order to OrderDto mapping
        config.NewConfig<Order, OrderDto>()
              .Map(dest => dest.OrderNumber, src => src.OrderNumber)
              .Map(dest => dest.InvoiceAddress, src => src.InvoiceAddress.Value)
              .Map(dest => dest.InvoiceEmailAddress, src => src.InvoiceEmailAddress.Value)
              .Map(dest => dest.InvoiceCreditCardNumber, src => src.InvoiceCreditCardNumber.Value)
              .Map(dest => dest.CreatedAt, src => src.CreatedAt)
              .Map(dest => dest.Products, src => src.OrderItems);

        // OrderItem to OrderItemDto mapping
        config.NewConfig<OrderItem, OrderItemDto>()
              .Map(dest => dest.ProductId, src => src.ProductId)
              .Map(dest => dest.ProductName, src => src.ProductName)
              .Map(dest => dest.ProductAmount, src => src.ProductAmount)
              .Map(dest => dest.ProductPrice, src => src.ProductPrice);
    }
}
