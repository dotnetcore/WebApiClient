using System;

namespace WebApiClientCore
{
    /// <summary>
    /// 定义HttpApiOptions变化通知的接口
    /// </summary> 
    public interface IHttpApiOptionsChangeNotifier
    {
        /// <summary>
        /// 通知HttpApiOptions变化
        /// </summary>
        /// <param name="httpApiType">接口类型</param>
        void NotifyChanged(Type httpApiType);

        /// <summary>
        /// 通知HttpApiOptions变化
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        void NotifyChanged<THttpApi>()
        {
            this.NotifyChanged(typeof(THttpApi));
        }
    }
}
