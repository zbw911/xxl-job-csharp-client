using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobClient.handler.attribute
{

    public class JobHanderAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
