using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    public class ApiResult<T>
    {
        public int Code { get; set; }

        public string Msg { get; set; }

        public T Data { get; set; }
    }
}
