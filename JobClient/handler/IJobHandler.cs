using JobClient.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobClient.handler
{
    public abstract class IJobHandler
    {
         
        public abstract ReturnT<String> execute(params string[] param);

    }
}
