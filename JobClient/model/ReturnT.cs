using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobClient.model
{
    public class ReturnT<T>
    {
         
        public static readonly int SUCCESS_CODE = 200;
        public static readonly int FAIL_CODE = 500;
        public static readonly ReturnT<String> SUCCESS = new ReturnT<String>(null);
        public static readonly ReturnT<String> FAIL = new ReturnT<String>(FAIL_CODE, null);

        public int code { get; set; }
        public String msg { get; set; }
        public T content { get; set; }

        public ReturnT() { }
        public ReturnT(int code, String msg)
        {
            this.code = code;
            this.msg = msg;
        }
        public ReturnT(T content)
        {
            this.code = SUCCESS_CODE;
            this.content = content;
        }

        public override string ToString()
        {
            return "ReturnT [code=" + code + ", msg=" + msg + ", content=" + content + "]";
        }



    }
}
