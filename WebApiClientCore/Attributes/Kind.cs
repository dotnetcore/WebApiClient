namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示参数类型
    /// </summary>
    public enum Kind
    {
        /// <summary>
        /// Uri路径参数
        /// 等效PathQueryAttribute
        /// </summary>
        Path = 0,

        /// <summary>
        /// Uri Query
        /// 等效PathQueryAttribute
        /// </summary>
        Query = 0,

        /// <summary>
        /// Header
        /// 等效HeaderAttribute
        /// </summary>
        Header = 1,

        /// <summary>
        /// x-www-form-urlencoded
        /// 等效FormContentAttribute
        /// </summary>
        Form = 2,

        /// <summary>
        /// multipart/form-data
        /// 等效FormDataContentAttribute
        /// </summary>
        FormData = 3,

        /// <summary>
        /// application/json
        /// 等效JsonContentAttribute
        /// </summary>
        JsonBody = 4,

        /// <summary>
        /// application/xml
        /// 等效XmlContentAttribute
        /// </summary>
        XmlBody = 5
    }
}
