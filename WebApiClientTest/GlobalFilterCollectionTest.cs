using System;
using Xunit;
using WebApiClient;
using System.Threading.Tasks;
using WebApiClient.Contexts;
using System.Linq;

namespace WebApiClientTest
{
    public class GlobalFilterCollectionTest
    {
        [Fact]
        public void Test_All_Methods()
        {
            var global = new GlobalFilterCollection();
            Assert.True(global.Count == 0);
            Assert.Throws<ArgumentNullException>(() => global.Add(null));

            var filter = new Filter();
            global.Add(filter);
            Assert.True(global.Count == 1 && global.First() == filter);
            Assert.Contains(filter, global);

            global.Remove(filter);
            Assert.True(global.Count == 0);


            global.Add(filter);
            global.Clear();
            Assert.True(global.Count == 0);
        }


        private class Filter : WebApiClient.Interfaces.IApiActionFilter
        {
            public Task OnBeginRequestAsync(ApiActionContext context)
            {
                throw new NotImplementedException();
            }

            public Task OnEndRequestAsync(ApiActionContext context)
            {
                throw new NotImplementedException();
            }
        }
    }
}
