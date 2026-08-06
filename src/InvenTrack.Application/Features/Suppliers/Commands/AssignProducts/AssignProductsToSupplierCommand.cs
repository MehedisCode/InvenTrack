namespace InvenTrack.Application.Features.Suppliers.Commands.AssignProducts;

using MediatR;

/// <summary>
/// Assigns a list of products to a supplier (many-to-many).
/// Existing assignments are preserved â€” this adds to them.
/// </summary>
public record AssignProductsToSupplierCommand(Guid SupplierId, List<Guid> ProductIds) : IRequest<Unit>;
