namespace ShoppingCart.Application.Common
{
    /// <summary>
    /// Thrown by Infrastructure when the data store rejects a change because it violates a constraint
    /// or because someone else changed the same data first. The Api maps it to HTTP 409.
    /// </summary>
    public sealed class DataConflictException : Exception
    {
        public DataConflictException(string message, Exception innerException) : base(message, innerException) { }
    }
}
