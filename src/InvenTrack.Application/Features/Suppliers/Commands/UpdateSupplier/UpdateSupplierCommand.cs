namespace InvenTrack.Application.Features.Suppliers.Commands.UpdateSupplier;

using MediatR;

public record UpdateSupplierCommand(
    Guid Id,
    string CompanyName,
    string ContactPerson,
    string Phone,
    string Email) : IRequest<Unit>;
