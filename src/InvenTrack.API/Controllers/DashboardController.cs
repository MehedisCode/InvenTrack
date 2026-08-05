namespace InvenTrack.API.Controllers;

using System.Threading.Tasks;
using InvenTrack.Application.Features.Dashboard.Queries.GetDashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly ISender _sender;

    public DashboardController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Returns inventory statistics for the dashboard (all roles).</summary>
    [HttpGet]
    public async Task<IActionResult> GetDashboard()
    {
        var result = await _sender.Send(new GetDashboardQuery());
        return Ok(result);
    }
}
