namespace InvenTrack.Infrastructure.UnitTests;

using FluentAssertions;
using InvenTrack.Infrastructure.Persistence;
using InvenTrack.Infrastructure.Persistence.Repositories;
using InvenTrack.Infrastructure.UnitTests.Helpers;
using InvenTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

[Collection("Database")]
public class RepositoryTests
{
    private readonly DatabaseFixture _fixture;

    public RepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    private Repository<Category> CreateRepository()
        => new CategoryRepository(_fixture.Context);

    [Fact]
    public async Task AddAsync_ShouldStageEntity()
    {
        var repository = CreateRepository();
        var category = new Category { Name = "Electronics" };

        var result = await repository.AddAsync(category);

        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
        _fixture.Context.Categories.Local.Should().Contain(category);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntity_WhenExists()
    {
        var repository = CreateRepository();
        var category = new Category { Name = "Books" };
        await _fixture.Context.Categories.AddAsync(category);
        await _fixture.Context.SaveChangesAsync();

        // Detach so the next read hits the database, not the change tracker.
        _fixture.Context.Entry(category).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

        var found = await repository.GetByIdAsync(category.Id);

        found.Should().NotBeNull();
        found!.Name.Should().Be("Books");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenMissing()
    {
        var repository = CreateRepository();

        var found = await repository.GetByIdAsync(Guid.NewGuid());

        found.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        var repository = CreateRepository();
        await _fixture.Context.Categories.AddRangeAsync(
            new Category { Name = "A" },
            new Category { Name = "B" },
            new Category { Name = "C" });
        await _fixture.Context.SaveChangesAsync();

        var all = await repository.GetAllAsync();

        all.Should().HaveCount(3);
    }

    [Fact]
    public void Query_ShouldReturnQueryable()
    {
        var repository = CreateRepository();

        var query = repository.Query();

        query.Should().NotBeNull();
        query.Should().BeAssignableTo<System.Linq.IQueryable<Category>>();
    }

    [Fact]
    public async Task UpdateAsync_ShouldMarkEntityModified()
    {
        var repository = CreateRepository();
        var category = new Category { Name = "Old" };
        await _fixture.Context.Categories.AddAsync(category);
        await _fixture.Context.SaveChangesAsync();

        category.Name = "New";
        await repository.UpdateAsync(category);
        await _fixture.Context.SaveChangesAsync();

        _fixture.Context.Entry(category).State.Should().Be(Microsoft.EntityFrameworkCore.EntityState.Modified);
        var reloaded = await _fixture.Context.Categories.FirstAsync(c => c.Id == category.Id);
        reloaded.Name.Should().Be("New");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEntity()
    {
        var repository = CreateRepository();
        var category = new Category { Name = "ToDelete" };
        await _fixture.Context.Categories.AddAsync(category);
        await _fixture.Context.SaveChangesAsync();

        await repository.DeleteAsync(category);
        await _fixture.Context.SaveChangesAsync();

        var exists = await _fixture.Context.Categories.AnyAsync(c => c.Id == category.Id);
        exists.Should().BeFalse();
    }
}
