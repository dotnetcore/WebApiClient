using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebApiClientCore.Serialization;

namespace WebApiClientCore.Benchmarks
{
    public class User
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("bio")]
        public string Bio { get; set; }

        [JsonPropertyName("followers")]
        public int Followers { get; set; }

        [JsonPropertyName("following")]
        public int Following { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }


        public static User Instance { get; }
        public static byte[] Utf8Json { get; }
        public static string XmlString { get; set; }

        // 硬编码测试数据，避免依赖外部文件
        private const string UserJsonString = @"{
  ""id"": 253,
  ""name"": ""Namee3a23814-bfe9-4d4b-96db-8fc95d209ea8"",
  ""bio"": ""Biof413d158-7ca7-4b1b-9073-565b3621bb83"",
  ""followers"": 154,
  ""following"": 136,
  ""url"": ""Url70f46596-f86f-4e82-900d-0f07d7dc468c""
}";

        static User()
        {
            Utf8Json = Encoding.UTF8.GetBytes(UserJsonString);
            Instance = JsonSerializer.Deserialize<User>(Utf8Json);
            XmlString = XmlSerializer.Serialize(Instance, null);
        }
    }
}
