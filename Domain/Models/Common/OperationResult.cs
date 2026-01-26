namespace Domain.Models.Common
{
    // <summary>
    /// Generic operation result with data
    /// </summary>
    public class OperationResult<T>
    {
        public bool IsSuccess { get; set; }
        public List<string> Errors { get; set; } = new();
        public T? Data { get; set; }

        public static OperationResult<T> Success(T data)
            => new() { IsSuccess = true, Data = data };

        public static OperationResult<T> Failure(params string[] errors)
            => new() { IsSuccess = false, Errors = errors.ToList() };
    }

    /// <summary>
    /// Non-generic operation result (for operations without return data)
    /// </summary>
    public class OperationResult
    {
        public bool IsSuccess { get; set; }
        public List<string> Errors { get; set; } = new();

        public static OperationResult Success()
            => new() { IsSuccess = true };

        public static OperationResult Failure(params string[] errors)
            => new() { IsSuccess = false, Errors = errors.ToList() };
    }
}
