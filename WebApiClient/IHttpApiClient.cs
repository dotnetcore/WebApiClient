using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 定义HttpApiClient的接口
    /// </summary>
    [Obsolete("该接口已废弃，请使用IHttpApi替代", true)]
    public interface IHttpApiClient : IHttpApi
    {
    }
}
