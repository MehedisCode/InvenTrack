namespace InvenTrack.Application.Features.Products.Commands.DeleteProduct;

using MediatR;

public record DeleteProductCommand(Guid Id) : IRequest<Unit>;
