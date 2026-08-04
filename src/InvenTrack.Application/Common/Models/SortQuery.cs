namespace InvenTrack.Application.Common.Models;

public abstract class SortQuery : PaginationQuery
{
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
}
