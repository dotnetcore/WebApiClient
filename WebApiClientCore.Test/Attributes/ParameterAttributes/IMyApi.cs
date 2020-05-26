using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace WebApiClientCore.Test.Attributes.ParameterAttributes
{
    public interface IMyApi
    {
        ITask<HttpResponseMessage> PostAsync(object headers);
    }
}
