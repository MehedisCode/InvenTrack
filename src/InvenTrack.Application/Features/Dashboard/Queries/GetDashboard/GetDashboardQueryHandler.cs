namespace InvenTrack.Application.Features.Dashboard.Queries.GetDashboard;

using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Application.Features.Dashboard.DTOs;
using MediatR;

public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, DashboardDto>
{
    private readonly IDashboardRepository _dashboardRepository;

    public GetDashboardQueryHandler(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<DashboardDto> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        return await _dashboardRepository.GetDashboardStatsAsync(cancellationToken);
    }
}
