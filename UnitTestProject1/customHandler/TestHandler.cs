using JobClient.handler;
using JobClient.handler.attribute;
using JobClient.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    class TestHandler : IJobHandler
    {
        public override ReturnT<string> execute(params string[] param)
        {
            throw new NotImplementedException();
        }
    }
    [JobHander(Name = "hehehehehe")]
    class TestHandler2 : IJobHandler
    {
        public override ReturnT<string> execute(params string[] param)
        {
            throw new NotImplementedException();
        }
    }
}
