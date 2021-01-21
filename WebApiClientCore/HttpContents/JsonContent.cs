using System.Text.Json;
using WebApiClientCore.Serialization;

namespace WebApiClientCore.HttpContents
{
    /// <summary>
    /// 表示uft8的json内容
    /// </summary>
    public class JsonContent : BufferContent
    {
        /// <summary>
        /// 获取对应的ContentType
        /// </summary>
        public static string MediaType => "application/json";

        /// <summary>
        /// uft8的json内容
        /// </summary> 
        public JsonContent()
            : base(MediaType)
        {
        }

        /// <summary>
        /// uft8的json内容
        /// </summary>
        /// <param name="value">patch操作项</param>
        /// <param name="jsonSerializerOptions">json序列化选项</param> 
        public JsonContent(object value, JsonSerializerOptions? jsonSerializerOptions)
            : base(MediaType)
        {
            JsonBufferSerializer.Serialize(this, value, jsonSerializerOptions);
        }
    }
}
