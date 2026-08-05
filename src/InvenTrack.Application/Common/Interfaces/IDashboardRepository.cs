namespace InvenTrack.Application.Common.Interfaces;

using System.Threading;
using System.Threading.Tasks;
using InvenTrack.Application.Features.Dashboard.DTOs;

public interface IDashboardRepository
{
    Task<DashboardDto> GetDashboardStatsAsync(CancellationToken cancellationToken = default);
}
