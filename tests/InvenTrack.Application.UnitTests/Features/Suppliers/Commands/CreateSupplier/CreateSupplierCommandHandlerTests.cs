namespace InvenTrack.Application.UnitTests.Features.Suppliers.Commands.CreateSupplier;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Suppliers.Commands.CreateSupplier;
using InvenTrack.Domain.Entities;
using Moq;
using Xunit;

public class CreateSupplierCommandHandlerTests
{
    private readonly Mock<ISupplierRepository> _supplierRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private CreateSupplierCommandHandler CreateHandler() => new(_supplierRepository.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_Should_CreateSupplier_WithActiveStatus()
    {
        _supplierRepository
            .Setup(r => r.AddAsync(It.IsAny<Supplier>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Supplier s, CancellationToken _) => s);

        var handler = CreateHandler();

        var result = await handler.Handle(new CreateSupplierCommand("Acme Corp", "John", "555-0100", "john@acme.com"), CancellationToken.None);

        result.Should().NotBeNull();
        result.CompanyName.Should().Be("Acme Corp");
        _supplierRepository.Verify(r => r.AddAsync(It.Is<Supplier>(s =>
            s.CompanyName == "Acme Corp" && s.ContactPerson == "John" && s.IsActive), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
