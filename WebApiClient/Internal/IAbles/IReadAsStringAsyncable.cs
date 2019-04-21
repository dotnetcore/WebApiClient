using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 定义可读取为文本的接口
    /// </summary>
    interface IReadAsStringAsyncable
    {
        /// <summary>
        /// 读取为文本信息
        /// </summary>
        /// <returns></returns>
        Task<string> ReadAsStringAsync();
    }
}
