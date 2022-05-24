namespace GameOfRevenge.Common.Net
{
    public interface IPagedView
    {
        int CountPerPage { get; set; }
        int CurrentPage { get; set; }
        int EndIndex { get; }
        int MaxPages { get; }
        int MaxRows { get; }
        int StartIndex { get; }
    }
}