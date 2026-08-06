namespace InvenTrack.Application.Features.Categories.Commands.DeleteCategory;

using MediatR;

public record DeleteCategoryCommand(Guid Id) : IRequest;
