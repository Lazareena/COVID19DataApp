using System;
namespace DataLayer
{
    public class NetworkStatus<T>
    {
        public T Result { get; set; }
        public bool Succeeded { get; set; }

        public string ErrorMessage { get; set; }
        public Exception Ex { get; set; }
        public NetworkErrorCode ErrorCode { get; set; }
    }

    public enum NetworkErrorCode
    {
        Ok,
        Failure,
        NetworkTimeout,
        Conflict,
        BadServerRequest,
        Unauthorized,
        ObsoleteApi,
    }

    public static class NetworkReturnUtils
    {
        public static NetworkErrorCode ExceptionToReturnCode(Exception ex)
        {
            var result = NetworkErrorCode.Failure;

            if (ex is System.Net.Http.HttpRequestException && ex.Message.Substring(0, 3) == "409")
                result = NetworkErrorCode.Conflict;
            if (ex is System.Net.Http.HttpRequestException && ex.Message.Substring(0, 3) == "400")
                result = NetworkErrorCode.BadServerRequest;
            if (ex is System.Net.Http.HttpRequestException && ex.Message.Substring(0, 3) == "401")
                result = NetworkErrorCode.Unauthorized;
            if ((ex is System.Net.Http.HttpRequestException && ex.InnerException != null && ex.InnerException.Message == "Error: NameResolutionFailure")
                || (ex is System.Net.Http.HttpRequestException && ex.Message.StartsWith("502")))
                result = NetworkErrorCode.NetworkTimeout;

            if (ex is System.Threading.Tasks.TaskCanceledException || ex is System.Net.WebException)
                result = NetworkErrorCode.NetworkTimeout;

            return result;
        }
    }
}
