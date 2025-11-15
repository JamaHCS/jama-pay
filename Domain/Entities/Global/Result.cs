namespace Domain.Entities.Global
{
    public class Result
    {
        public bool Success { get; set; }
        public object? Errors { get; set; }
        public int Status { get; set; }

        public Result(bool success, string? error, int code)
        {
            Success = success;
            Errors = error;
            Status = code;
        }

        public static Result Ok(int httpCode = 200) => new Result(true, null, httpCode);
        public static Result Failure(string error, int httpCode = 400) => new Result(false, error, httpCode);
        public static Result<T> Ok<T>(T value, int httpCode = 200) => new Result<T>(value, true, null, httpCode);
        public static Result<T> Failure<T>(string error, int httpCode = 400) => new Result<T>(default, false, error, httpCode);
        public static Result<T> Failure<T>(string error, T value, int httpCode = 400) => new Result<T>(value, false, error, httpCode);
    }

    public class Result<T> : Result
    {
        public T Value { get; set; }
        public Result(T value, bool isSuccess, string? error, int httpCode) : base(isSuccess, error, httpCode)
        {
            Value = value;
        }
    }
}
