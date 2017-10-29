using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    public interface IRetryHandler<TResult>
    {
        Task<TResult> When<TException>() where TException : Exception;
    }
}
