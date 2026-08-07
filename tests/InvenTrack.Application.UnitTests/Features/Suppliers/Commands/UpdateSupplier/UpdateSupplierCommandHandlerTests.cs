namespace InvenTrack.Application.UnitTests.Features.Suppliers.Commands.UpdateSupplier;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Suppliers.Commands.UpdateSupplier;
using InvenTrack.Domain.Entities;
using Moq;
using Xunit;

public class UpdateSupplierCommandHandlerTests
{
    private readonly Mock<ISupplierRepository> _supplierRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private UpdateSupplierCommandHandler CreateHandler() => new(_supplierRepository.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_Should_UpdateSupplier_WhenSupplierExists()
    {
        var supplierId = Guid.NewGuid();
        var existing = new Supplier { Id = supplierId, CompanyName = "Old" };

        _supplierRepository
            .Setup(r => r.GetByIdAsync(supplierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var handler = CreateHandler();

        await handler.Handle(new UpdateSupplierCommand(supplierId, "New Co", "Jane", "555", "jane@new.co"), CancellationToken.None);

        _supplierRepository.Verify(r => r.UpdateAsync(It.Is<Supplier>(s =>
            s.Id == supplierId && s.CompanyName == "New Co" && s.ContactPerson == "Jane"), It.IsAny<CancellationToken>()), Times.Once);
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

        await FluentActions.Invoking(() => handler.Handle(
            new UpdateSupplierCommand(supplierId, "New Co", "Jane", "555", "jane@new.co"), CancellationToken.None))
            .Should().ThrowAsync<Exception>()
            .WithMessage("*Supplier not found.*");

        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
