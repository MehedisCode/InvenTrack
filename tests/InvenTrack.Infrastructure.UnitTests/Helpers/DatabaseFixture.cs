namespace InvenTrack.Infrastructure.UnitTests.Helpers;

using InvenTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

/// <summary>
/// Shared Testcontainers Postgres fixture for the Infrastructure test project.
/// One container is started for the lifetime of the test run; each test gets a fresh
/// database schema (created in the constructor) so tests stay isolated.
/// </summary>
public class DatabaseFixture : IAsyncLifetime
{
#pragma warning disable CS0618 // PostgreSqlBuilder parameterless ctor is obsolete but functional in 4.13.0
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithCleanUp(true)
        .Build();
#pragma warning restore CS0618

    public ApplicationDbContext Context { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .Options;

        Context = new ApplicationDbContext(options);
        await Context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        if (Context is not null)
        {
            await Context.DisposeAsync();
        }

        await _container.DisposeAsync();
    }
}

[CollectionDefinition("Database")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
    // xUnit uses this class only as an anchor for the collection definition.
}
