namespace ShoppingCart.Application.Common
{
    public sealed class ApiResponseDto<T>
    {
        public bool Success { get; init; }
        public string? Message { get; init; }
        public T? Data { get; init; }
        public IReadOnlyDictionary<string, string[]>? Errors { get; init; }

        public static ApiResponseDto<T> Ok(T? data, string? message = null) => new()
        {
            Success = true,
            Data = data,
            Message = message
        };

        public static ApiResponseDto<T> Error(string message, IReadOnlyDictionary<string, string[]>? errors = null) => new()
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}
