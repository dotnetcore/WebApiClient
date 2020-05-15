using System.Threading.Tasks;
using WebApiClientCore.Parameterables;
using Xunit;

namespace WebApiClientCore.Test.Parameterables
{
    public class JsonPatchDocumentTest
    {
        [Fact]
        public async Task BeforeRequestAsync()
        {
            var doc = new JsonPatchDocument<Model>();
            doc.Replace(item => item.Name, "33");
            doc.Replace(item => item.Age, 10);

            var context = new TestActionContext(
               apiActionDescriptor: new ApiActionDescriptor(typeof(IMyApi).GetMethod("PostAsync")));

            context.HttpContext.RequestMessage.Method = new System.Net.Http.HttpMethod("Patch");  
            await ((IApiParameterable)doc).OnRequestAsync(new ApiParameterContext(context, 0));

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
