namespace GameOfRevenge.Common.Net
{
    public interface IResponse
    {
        int Case { get; set; }
        bool IsSuccess { get; }
        string Message { get; set; }

        CaseType GetCaseType();
    }

    public interface IResponse<T>
    {
        T Data { get; set; }
        bool HasData { get; }
    }
}