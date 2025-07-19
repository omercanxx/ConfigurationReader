using ConfigurationReader.Common.Enums;

namespace ConfigurationReader.Common
{
    public class ServiceResponse<T> : ServiceResponse
    {
        /// Default constructor for success response
        public ServiceResponse(T result)
        {
            this.IsSuccess = true;
            this.Result = result;
        }

        public T Result { get; set; }
    }

    public class ServiceResponse
    {
        /// Default constructor for success response
        public ServiceResponse()
        {
            this.IsSuccess = true;
        }

        /// Default constructor for fail response
        public ServiceResponse(ErrorCode errorCode)
        {
            this.IsSuccess = false;
            this.ErrorCode = (int)errorCode;
            this.ErrorMessage = errorCode.ToString();
        }

        public bool IsSuccess { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;

        public int ErrorCode { get; set; }
    }
}
