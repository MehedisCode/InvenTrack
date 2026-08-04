namespace InvenTrack.Application.Common.Models;

public abstract class PaginationQuery
{
    public int PageNumber { get; set; } = 1;
    private int _pageSize = 10;
    
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > 50) ? 50 : value;
    }
}
