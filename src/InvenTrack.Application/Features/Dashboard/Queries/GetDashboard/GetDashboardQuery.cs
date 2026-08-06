namespace InvenTrack.Application.Features.Dashboard.Queries.GetDashboard;

using InvenTrack.Application.Features.Dashboard.DTOs;
using MediatR;

public record GetDashboardQuery : IRequest<DashboardDto>;
