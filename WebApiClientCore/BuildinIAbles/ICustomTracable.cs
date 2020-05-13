using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 定义自定义输出内容的接口
    /// </summary>
    interface ICustomTracable
    {
        /// <summary>
        /// 读取为文本信息
        /// </summary>
        /// <returns></returns>
        Task<string> ReadAsStringAsync();
    }
}
