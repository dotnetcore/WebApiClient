using System;
using System.Linq;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Contexts;
using Xunit;

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


        private class Filter : WebApiClient.IApiActionFilter
        {
            public Task OnBeginRequestAsync(ApiActionContext context)
            {
                throw new NotImplementedException();
            }

            public Task OnEndRequestAsync(ApiActionContext context)
            {
                throw new NotImplementedException();
            }

            public Task<bool> OnExceptionAsync(ApiActionContext context)
            {
                throw new NotImplementedException();
            }
        }
    }
}
