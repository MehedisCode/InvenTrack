namespace InvenTrack.Application.Features.Categories.DTOs;

/// <summary>Request body for updating a category (Id comes from route).</summary>
public record UpdateCategoryRequest(string Name, string? Description);
