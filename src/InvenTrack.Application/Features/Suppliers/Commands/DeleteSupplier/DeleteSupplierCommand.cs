namespace InvenTrack.Application.Features.Suppliers.Commands.DeleteSupplier;

using MediatR;

public record DeleteSupplierCommand(Guid Id) : IRequest<Unit>;
