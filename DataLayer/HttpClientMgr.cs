using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DataLayer
{
    public class HttpClientMgr
    {
        private const int _maxRetryIfNetworkTimeout = 2;

        private HttpClient _client;
        private HttpClient Client
        {
            get
            {
                if (_client == null)
                    _client = new HttpClient();
                return _client;
            }
        }

        public async Task<NetworkStatus<string>> GetAsync(string url)
        {
            var result = new NetworkStatus<string>() { Succeeded = false };
            bool done = false;
            int retryCounts = 0;
            while (!done)
            {
                done = true;
                try
                {
                    var response = await Client.GetAsync(url).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();
                    result.Succeeded = true;
                    result.Result = await response.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    result.ErrorCode = NetworkReturnUtils.ExceptionToReturnCode(ex);
                    result.ErrorMessage = ex.Message;
                    result.Ex = ex;
                    if (result.ErrorCode == NetworkErrorCode.NetworkTimeout)
                    {
                        retryCounts++;
                        if (retryCounts < _maxRetryIfNetworkTimeout)
                            done = false;
                    }
                }
            }
            return result;
        }
    }
}
