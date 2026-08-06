namespace InvenTrack.Application.Features.Suppliers.Commands.CreateSupplier;

using InvenTrack.Application.Features.Suppliers.DTOs;
using MediatR;

public record CreateSupplierCommand(
    string CompanyName,
    string ContactPerson,
    string Phone,
    string Email) : IRequest<SupplierDto>;
