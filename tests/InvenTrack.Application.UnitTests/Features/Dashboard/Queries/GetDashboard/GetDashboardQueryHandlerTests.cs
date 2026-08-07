namespace InvenTrack.Application.UnitTests.Features.Dashboard.Queries.GetDashboard;

using FluentAssertions;
using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Dashboard.DTOs;
using InvenTrack.Application.Features.Dashboard.Queries.GetDashboard;
using Moq;
using Xunit;

public class GetDashboardQueryHandlerTests
{
    private readonly Mock<IDashboardRepository> _dashboardRepository = new();

    private GetDashboardQueryHandler CreateHandler() => new(_dashboardRepository.Object);

    [Fact]
    public async Task Handle_Should_ReturnDashboardStats()
    {
        var expected = new DashboardDto { Overview = new OverviewDto { TotalProducts = 42 } };
        _dashboardRepository
            .Setup(r => r.GetDashboardStatsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = CreateHandler();

        var result = await handler.Handle(new GetDashboardQuery(), CancellationToken.None);

        result.Should().Be(expected);
        result.Overview.TotalProducts.Should().Be(42);
        _dashboardRepository.Verify(r => r.GetDashboardStatsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
