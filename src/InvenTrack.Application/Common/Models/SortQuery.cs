namespace InvenTrack.Application.Common.Models;

public abstract class SortQuery : PaginationQuery
{
    /// <summary>
    /// The field to sort by. Supported values: "price", "name", "quantity". Default is sort by created date.
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Set to true to sort in descending order (highest to lowest). Default is false (ascending).
    /// </summary>
    public bool SortDescending { get; set; }
}
