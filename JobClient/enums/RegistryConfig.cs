using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobClient.enums
{
    public class RegistryConfig
    {

        public static readonly int BEAT_TIMEOUT = 30;
        public static readonly int DEAD_TIMEOUT = BEAT_TIMEOUT * 3;

        public enum RegistType { EXECUTOR, ADMIN }

    }
}
