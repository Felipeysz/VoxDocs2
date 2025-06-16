using System.Net;

namespace VoxDocs.BusinessRules
{
    public class ValidationResult
    {
        public bool IsValid { get; }
        public string ErrorMessage { get; }
        public HttpStatusCode StatusCode { get; }
        public object Data { get; }

        protected ValidationResult(bool isValid, string errorMessage, HttpStatusCode statusCode, object data = null)
        {
            IsValid = isValid;
            ErrorMessage = errorMessage;
            StatusCode = statusCode;
            Data = data;
        }

        public static ValidationResult Success(object data = null) => new ValidationResult(true, null, HttpStatusCode.OK, data);
        public static ValidationResult Error(string message, HttpStatusCode statusCode) => new ValidationResult(false, message, statusCode);
    }

    public class ValidationResult<T> : ValidationResult
    {
        public T Result => (T)Data;

        private ValidationResult(bool isValid, string errorMessage, HttpStatusCode statusCode, T data) 
            : base(isValid, errorMessage, statusCode, data)
        {
        }

        public static ValidationResult<T> Success(T data) => new ValidationResult<T>(true, null, HttpStatusCode.OK, data);
        public static new ValidationResult<T> Error(string message, HttpStatusCode statusCode) => new ValidationResult<T>(false, message, statusCode, default);
    }
}