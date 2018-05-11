using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobClient.utils
{
    namespace log4net.Core
    {
        public static class Log4NetExtensions
        {
            public static void ErrorFormatEx(this ILog logger, string format, Exception exception, params object[] args)
            {
                logger.Error(string.Format(format, args), exception);
            }
        }
    }
}
