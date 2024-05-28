using System.Text.Json.Serialization;

namespace AppAot
{
    [JsonSerializable(typeof(AppData[]))]
    partial class AppJsonSerializerContext : JsonSerializerContext
    {
    }
}
