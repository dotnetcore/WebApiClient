using System;

namespace WebApiClientCore
{
    /// <summary>
    /// 定义HttpApiOptions的Action配置触发器
    /// </summary> 
    public interface IHttpApiOptionsConfigureTrigger
    {
        /// <summary>
        /// 触发HttpApiOptions的Action配置
        /// </summary>
        /// <param name="httpApiType">接口类型</param>
        void Raise(Type httpApiType);

        /// <summary>
        /// 触发HttpApiOptions的Action配置
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        void Raise<THttpApi>()
        {
            this.Raise(typeof(THttpApi));
        }
    }
}
