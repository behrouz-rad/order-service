// © 2025 Behrouz Rad. All rights reserved.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OrderService.Domain.Orders;

namespace OrderService.Infrastructure.Interceptors;

public class CreatedAtInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        SetCreatedAtForNewOrders(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, 
        InterceptionResult<int> result, 
        CancellationToken cancellationToken = default)
    {
        SetCreatedAtForNewOrders(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void SetCreatedAtForNewOrders(DbContext? context)
    {
        if (context is null) return;

        var newOrders = context.ChangeTracker
            .Entries<Order>()
            .Where(e => e.State == EntityState.Added)
            .Select(e => e.Entity);

        var now = DateTimeOffset.UtcNow;
        foreach (var order in newOrders)
        {
            order.SetCreatedAt(now);
        }
    }
}
