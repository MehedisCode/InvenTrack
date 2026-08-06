namespace InvenTrack.Application.Features.Products.DTOs;

/// <summary>Request body for updating a product (Id comes from route).</summary>
public record UpdateProductRequest(
    string Name,
    string SKU,
    string? Description,
    decimal PurchasePrice,
    decimal SellingPrice,
    Guid CategoryId,
    bool IsActive);
