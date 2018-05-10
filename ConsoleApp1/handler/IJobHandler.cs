using ConsoleApp1.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.handler
{
    public abstract class IJobHandler
    {
         
        public abstract ReturnT<String> execute(params string[] param);

    }
}
