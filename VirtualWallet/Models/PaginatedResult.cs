namespace VirtualWallet.Models;

public class PaginatedResult<T>
{
    public T[] Items { get; set; } = [];
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
}