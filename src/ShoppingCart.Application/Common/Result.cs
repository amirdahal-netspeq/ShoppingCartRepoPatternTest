namespace ShoppingCart.Application.Common
{
    public enum ErrorType { Validation, NotFound, Conflict }

    public sealed record Error(ErrorType Type, string Message)
    {
        public static Error Validation(string message) => new(ErrorType.Validation, message);
        public static Error NotFound(string message) => new(ErrorType.NotFound, message);
        public static Error Conflict(string message) => new(ErrorType.Conflict, message);
    }

    public class Result
    {
        protected Result(bool isSuccess, Error? error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error? Error { get; }

        public static Result Success() => new(true, null);
        public static Result Failure(Error error) => new(false, error);
        public static Result<T> Success<T>(T value) => new(value, true, null);
        public static Result<T> Failure<T>(Error error) => new(default, false, error);

        public static implicit operator Result(Error error) => Failure(error);
    }

    public sealed class Result<T> : Result
    {
        private readonly T? _value;

        internal Result(T? value, bool isSuccess, Error? error) : base(isSuccess, error) => _value = value;

        public T Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException("Cannot read the value of a failed result.");

        public static implicit operator Result<T>(Error error) => Failure<T>(error);
    }
}
