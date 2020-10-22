namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示请求服务主机
    /// </summary>
    public abstract class HttpHostBaseAttribute : ApiActionAttribute
    {
        /// <summary>
        /// 请求服务主机      
        /// </summary> 
        public HttpHostBaseAttribute()
        {
            this.OrderIndex = int.MinValue;
        }
    }
}
