using System;
using System.Buffers;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebApiClientCore.Serialization
{
    /// <summary>
    /// Newtonsoft.Json的json序列化工具
    /// </summary>
    public class NewtonsoftJsonSerializer : IJsonSerializer
    {
        private static readonly JsonSerializerSettings defaultOptions = new JsonSerializerSettings();

        /// <summary>
        /// json序列化设置
        /// </summary>
        /// <remarks>
        /// 2020-07-11 因接口限制，接口上面的方法必须使用<see cref="System.Text.Json.JsonSerializerOptions"/>,与Newtonsoft.Json的设置不兼容,所以需要使用这个字段来接收注册服务时配置的json配置，不使用接口传递的
        /// </remarks>
        private readonly JsonSerializerSettings? jsonOptions;

        private NewtonsoftJsonSerializer(JsonSerializerSettings? jsonOptions)
        {
            this.jsonOptions = jsonOptions ?? defaultOptions;
        }

        /// <summary>
        /// 将对象序列化为 utf8编码的Json 到指定的bufferWriter
        /// </summary>
        /// <param name="bufferWriter">buffer写入器</param>
        /// <param name="obj">对象</param>
        /// <param name="options">选项</param>
        public void Serialize(IBufferWriter<byte> bufferWriter, object? obj, JsonSerializerOptions? options)
        {
            if (obj == null || bufferWriter == null)
            {
                return;
            }

            string jsonStr = JsonConvert.SerializeObject(obj, obj.GetType(), this.jsonOptions);
            ReadOnlySpan<byte> bytes = Encoding.UTF8.GetBytes(jsonStr).AsSpan();
            bufferWriter.Write(bytes);
        }

        /// <summary>
        /// 将utf8编码的Json反序列化为对象
        /// </summary>
        /// <param name="utf8Json">json</param>
        /// <param name="objType">对象类型</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        public object? Deserialize(ReadOnlySpan<byte> utf8Json, Type objType, JsonSerializerOptions? options)
        {
            return utf8Json.IsEmpty
                ? objType.DefaultValue()
                : deserialize(utf8Json, objType);

            object? deserialize(ReadOnlySpan<byte> utf8Json2, Type objType2)
            {
                string objStr = Encoding.UTF8.GetString(utf8Json2);
                return JsonConvert.DeserializeObject(new string(objStr), objType2);
            }
        }

        /// <summary>
        /// 将utf8编码的Json流 反序列化为对象
        /// </summary>
        /// <param name="utf8JsonStream">utf8编码的Json流</param>
        /// <param name="objType">对象类型</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        public async Task<object> DeserializeAsync(Stream utf8JsonStream, Type objType, JsonSerializerOptions? options)
        {
#pragma warning disable CS8603 // 可能的 null 引用返回。
            StreamReader streamReader = new StreamReader(utf8JsonStream, Encoding.UTF8);
            string jsonStr = await streamReader.ReadToEndAsync();
            if (string.IsNullOrEmpty(jsonStr))
            {
                return objType.DefaultValue();
            }
            else
            {
                return JsonConvert.DeserializeObject(jsonStr, objType, this.jsonOptions);
            }

#pragma warning restore CS8603 // 可能的 null 引用返回。
        }

        /// <summary>
        /// 创建 NewtonsoftJsonSerializer
        /// </summary>
        /// <param name="optionAction"></param>
        /// <returns></returns>
        public static NewtonsoftJsonSerializer CreateJsonSerializer(Action<JsonSerializerSettings?>? optionAction = null)
        {
            JsonSerializerSettings jsonSerializer = new JsonSerializerSettings();
            optionAction?.Invoke(jsonSerializer);

            return new NewtonsoftJsonSerializer(jsonSerializer);
        }
    }
}