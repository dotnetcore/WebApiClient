using System;
using System.Threading.Tasks;

namespace App.Clients
{
    public class DynamicHostDemoService
    {
        private readonly IDynamicHostDemo dynamicHostDemo;

        public DynamicHostDemoService(IDynamicHostDemo dynamicHostDemo)
        {
            this.dynamicHostDemo = dynamicHostDemo;
        }

        internal async Task RunRequestAsync()
        {
            var r1 = await dynamicHostDemo.ByUrlString("http://www.soso.com");
            var r2 = await dynamicHostDemo.ByAttribute();
            var r3 = await dynamicHostDemo.ByFilter();
        }
    }
}
