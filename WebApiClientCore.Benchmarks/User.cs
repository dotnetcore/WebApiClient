using System.IO;
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

        static User()
        {
            Utf8Json = File.ReadAllBytes("user.json");
            Instance = JsonSerializer.Deserialize<User>(Utf8Json);
            XmlString = XmlSerializer.Serialize(Instance, null);
        }
    }
}
