namespace InvenTrack.Application.UnitTests.Features.Suppliers.Commands.DeleteSupplier;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Suppliers.Commands.DeleteSupplier;
using InvenTrack.Domain.Entities;
using Moq;
using Xunit;

public class DeleteSupplierCommandHandlerTests
{
    private readonly Mock<ISupplierRepository> _supplierRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private DeleteSupplierCommandHandler CreateHandler() => new(_supplierRepository.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_Should_SoftDeleteSupplier_WhenSupplierExists()
    {
        var supplierId = Guid.NewGuid();
        var existing = new Supplier { Id = supplierId, CompanyName = "Acme", IsActive = true };

        _supplierRepository
            .Setup(r => r.GetByIdAsync(supplierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var handler = CreateHandler();

        await handler.Handle(new DeleteSupplierCommand(supplierId), CancellationToken.None);

        _supplierRepository.Verify(r => r.UpdateAsync(It.Is<Supplier>(s =>
            s.Id == supplierId && s.IsActive == false), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenSupplierDoesNotExist()
    {
        var supplierId = Guid.NewGuid();
        _supplierRepository
            .Setup(r => r.GetByIdAsync(supplierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Supplier?)null);

        var handler = CreateHandler();

        await FluentActions.Invoking(() => handler.Handle(new DeleteSupplierCommand(supplierId), CancellationToken.None))
            .Should().ThrowAsync<Exception>()
            .WithMessage("*Supplier not found.*");

        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
