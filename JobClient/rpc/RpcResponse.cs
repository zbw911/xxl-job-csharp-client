using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobClient.model
{

    public class RpcResponse
    {
        
        public String error { get; set; }
        public Object result { get; set; }


        public bool isError()
        {
            return error != null;
        }


        public override string ToString()
        {
            return "NettyResponse [error=" + error
              + ", result=" + result + "]";
        }

    }
}
