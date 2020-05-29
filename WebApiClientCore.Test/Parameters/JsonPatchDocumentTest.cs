using System.Threading.Tasks;
using WebApiClientCore.Parameters;
using Xunit;

namespace WebApiClientCore.Test.Parameters
{
    public class JsonPatchDocumentTest
    {
        [Fact]
        public async Task OnRequestAsync()
        {
            var doc = new JsonPatchDocument<Model>();
            doc.Replace(item => item.Name, "33");
            doc.Replace(item => item.Age, 10);

            var apiAction = new ApiActionDescriptor(typeof(ITestApi).GetMethod("PostAsync"));
            var context = new TestRequestContext(apiAction, string.Empty);

            context.HttpContext.RequestMessage.Method = new System.Net.Http.HttpMethod("Patch");
            await doc.OnRequestAsync(new ApiParameterContext(context, 0));

            var body = await context.HttpContext.RequestMessage.Content.ReadAsStringAsync();
            var ops = System.Text.Json.JsonSerializer.Deserialize<Op[]>(body);
            Assert.Equal(2, ops.Length);

            Assert.Equal("replace", ops[0].op);
            Assert.Equal("/name", ops[0].path);
            Assert.Equal("33", ops[0].value.ToString());
        }
    }

    class Op
    {
        public string op { get; set; }

        public string path { get; set; }

        public object value { get; set; }
    }
    class Model
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
