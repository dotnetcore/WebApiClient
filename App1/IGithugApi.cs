using System;
using WebApiClient;
using WebApiClient.Attributes;

namespace App1
{
    [JsonReturn]
    public interface IGithugApi : IDisposable
    {
        [HttpGet("https://iot.taichuan.net/v1/Intercom/GetAccount?userId={userId}")]
        ITask<ApiResult> GetApiListAsync(string userId);
    }


    public class ApiResult
    {
        public int code { get; set; }
        public string msg { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public bool isNewCreate { get; set; }
        public string userId { get; set; }
        public string loginToken { get; set; }
        public string createTime { get; set; }
    }


}
